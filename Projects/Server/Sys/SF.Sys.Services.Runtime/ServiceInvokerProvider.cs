#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using System;
using System.Collections.Generic;

using System.Linq;
using System.Collections;
using SF.Sys.Services.Internals;
using System.Collections.Concurrent;
using SF.Sys.Collections.Generic;
using SF.Sys.Reflection;
using System.Linq.Expressions;
using System.Reflection;

namespace SF.Sys.Services
{
	public class ServiceInvokeArgumentBase
	{

	}
	class ServiceInvokerProvider : IServiceInvokerProvider
	{
		IServiceMetadata ServiceMetadata { get; }
		IDynamicTypeBuilder DynamicTypeBuilder { get; }
		
		class Invoker: IServiceInvoker
		{
			public IServiceDeclaration ServiceDeclaration { get; }
			public MethodInfo Method { get; }
			static MethodInfo JsonParse { get; } =
				typeof(Json).GetMethodExt(nameof(Json.Parse), typeof(string)).IsNotNull();

			Lazy<Func<object, string, object>> LazyFuncByStrArgument { get; }
			Lazy<Func<object, object[], object>> LazyFuncByObjArguments { get; }

			Func<object, string, object> CompileStrArg(IDynamicTypeBuilder DynamicTypeBuilder)
			{
				var typeExpr = new TypeExpression(
							  ServiceDeclaration.ServiceType.Name.Replace('.', '_') + "_" + Method + "_Argument",
							  new SystemTypeReference(typeof(ServiceInvokeArgumentBase)),
							  System.Reflection.TypeAttributes.Class | System.Reflection.TypeAttributes.Public
						  );
				var parameters = Method.GetParameters();
				typeExpr.Properties.AddRange(
					parameters.Select(p => new PropertyExpression(
						p.Name,
						new SystemTypeReference(p.ParameterType),
						System.Reflection.PropertyAttributes.None
						)
					));
				var argType = DynamicTypeBuilder.Build(new[] { typeExpr })[0];

				var argSvc = Expression.Parameter(typeof(object), "svc");
				var argArgs = Expression.Parameter(typeof(string), "arg");

				var varArg = Expression.Variable(argType, "v");

				return Expression.Lambda<Func<object, string, object>>(
					Expression.Block(
						new[] { varArg },
						Expression.Assign(
							varArg,
							Expression.Call(null,
								JsonParse.MakeGenericMethod(argType),
								argArgs
							)
						),
						Expression.Convert(
							Expression.Call(
								Expression.Convert(argSvc,Method.DeclaringType),
								Method,
								parameters.Select(p => Expression.Property(varArg, p.Name))
							),
							typeof(object)
							)
						),
					argSvc,
					argArgs
					).Compile();
			}

			Func<object, object[], object> CompileObjArg()
			{
				
				var parameters = Method.GetParameters();
				
				var argSvc = Expression.Parameter(typeof(object), "svc");
				var argArgs = Expression.Parameter(typeof(object[]), "arg");

				return Expression.Lambda<Func<object, object[], object>>(
						Expression.Convert(
							Expression.Call(
								Expression.Convert(argSvc, (Type)Method.DeclaringType),
								(MethodInfo)Method,
								parameters.Select(
									(p,i)=>
										p.ParameterType.IsValueType?
										(Expression)Expression.Condition(
											Expression.Equal(
												Expression.ArrayAccess(argArgs, Expression.Constant(i)),
												Expression.Constant(null,typeof(object))
												),
											Expression.Constant(p.ParameterType.GetDefaultValue(),p.ParameterType),
											Expression.Unbox(
												Expression.ArrayAccess(argArgs,Expression.Constant(i)),
												p.ParameterType
											)
										):
										(Expression)Expression.Convert(
											Expression.ArrayAccess(argArgs, Expression.Constant(i)),
											p.ParameterType
										)
									)
							),
							typeof(object)
						),
					argSvc,
					argArgs
					).Compile();
			}
			public Invoker(IDynamicTypeBuilder DynamicTypeBuilder,IServiceDeclaration decl,MethodInfo method)
			{
				ServiceDeclaration = decl;
				Method = method;

				LazyFuncByStrArgument = new Lazy<Func<object, string, object>>(() => CompileStrArg(DynamicTypeBuilder));
				LazyFuncByObjArguments= new Lazy<Func<object, object[], object>>(() => CompileObjArg());
			}
			public object Invoke(IServiceProvider ServiceProvider, string Arguments)
			{
				var svc = ServiceProvider.GetService(ServiceDeclaration.ServiceType);
				return LazyFuncByStrArgument.Value.Invoke(svc, Arguments);
			}
			public object Invoke(IServiceProvider ServiceProvider, object[] Arguments)
			{
				var svc = ServiceProvider.GetService(ServiceDeclaration.ServiceType);
				return LazyFuncByObjArguments.Value.Invoke(svc, Arguments);
			}
		}
		ConcurrentDictionary<(Type Service, MethodInfo Method), Lazy<Invoker>> Invokers { get; } 
			= new ConcurrentDictionary<(Type Service, MethodInfo Method), Lazy<Invoker>>();

		public ServiceInvokerProvider(IServiceMetadata ServiceMetadata, IDynamicTypeBuilder DynamicTypeBuilder)
		{
			this.ServiceMetadata = ServiceMetadata;
			this.DynamicTypeBuilder = DynamicTypeBuilder;
		}
		
		public IServiceInvoker Resolve(Type Service, MethodInfo Method)
		{
			IServiceDeclaration decl=null;
			
			var invokerKey = (Service, Method);
			if (!Invokers.TryGetValue(invokerKey,out var invoker))
			{
				if (decl == null)
				{
					decl = ServiceMetadata.Services.Get(Service);
					if (decl == null)
						throw new PublicArgumentException("找不到指定的服务:" + Service);
				}

				invoker = Invokers.GetOrAdd(
					invokerKey, 
					new Lazy<Invoker>(()=>
						new Invoker(DynamicTypeBuilder, decl, Method)
					)
				);
			}
			return invoker.Value;
		}
	}

}
