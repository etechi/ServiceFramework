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
using System.Reflection;
using System.Linq.Expressions;
using SF.Sys.Reflection;
using SF.Sys.Serialization;
using SF.Sys.Services;
using SF.Sys.Linq;
using SF.Sys.Linq.Expressions;

namespace SF.Sys.NetworkService
{
	public class ServiceInvoker : IServiceInvoker
	{
		IDynamicTypeBuilder DynamicTypeBuilder { get; }
		IServiceBuildRuleProvider BuildRule { get; }
		IJsonSerializer JsonSerializer { get; }
		Dictionary<string,Type> ServiceTypes { get; }
		System.Collections.Concurrent.ConcurrentDictionary<(Type, string),Lazy< Func<object, IJsonSerializer, string, object>>> Invokers { get; } = 
			new System.Collections.Concurrent.ConcurrentDictionary<(Type, string),Lazy< Func<object, IJsonSerializer, string, object>>>();

		public ServiceInvoker(
			IServiceBuildRuleProvider BuildRule,
			IDynamicTypeBuilder DynamicTypeBuilder,
			IJsonSerializer JsonSerializer,
			IServiceTypeCollection ServiceTypeCollection
			)
		{
			this.BuildRule = BuildRule;
			this.JsonSerializer = JsonSerializer;
			this.ServiceTypes = ServiceTypeCollection.Types.ToDictionary(
				t => BuildRule.FormatServiceName(t), t => t
				);
			this.DynamicTypeBuilder = DynamicTypeBuilder;
		}
		//(Type,object) GetService(IServiceProvider isp,string Service, string Method)
		//{
		//	object svc;
		//	var resolver = isp.Resolver();
		//	IServiceDeclaration decl;
		//	if (long.TryParse(Service, out var sid))
		//	{
		//		decl = resolver.ResolveDescriptorByIdent(sid)?.ServiceDeclaration;
		//		if (decl == null)
		//			return (null,null);
		//		svc = resolver.ResolveServiceByIdent(sid, decl.ServiceType);
		//	}
		//	else
		//	{
		//		decl = ServiceMetadata.ServicesByTypeName.Get(Service);
		//		if (decl == null)
		//			return (null, null);
		//		svc = resolver.ResolveServiceByType(
		//			null,
		//			decl.ServiceType,
		//			null
		//			);
		//	}
		//	if (svc == null)
		//		return (null, null);
		//	return (decl.ServiceType, svc);

		//}
		public object Invoke(IServiceProvider ServiceProvider,long? ScopeId,string Service, string Method, string Argument)
		{
			if (!ServiceTypes.TryGetValue(Service, out var ServiceType))
				throw new ArgumentException($"动态服务请求找不到服务名称{Service}");

			var key = (ServiceType, Method);
			if (!Invokers.TryGetValue(key, out var func))
			{
				func = new Lazy<Func<object, IJsonSerializer, string, object>>(
					() => BuildInvokeFunc(ServiceType, Method)
					);
				func = Invokers.GetOrAdd(key, func);
			}

			var svc = ServiceProvider.Resolver().ResolveServiceByType(ScopeId, ServiceType, null);
			if (svc == null)
				throw new ArgumentException($"找不到服务, 区域:{ScopeId} {ServiceType}");
			return func.Value(svc, JsonSerializer, Argument);
		}

		static volatile int MethodArgumentTypeIdentSeed=1;
		public class BaseMethodArgument
		{

		}

		static MethodInfo MethodDeserialize { get; } = 
			typeof(IJsonSerializer)
			.GetMethod(nameof(IJsonSerializer.Deserialize));


		Func<object, IJsonSerializer, string, object> BuildInvokeFunc(Type Type,string MethodName)
		{
			var methods = (from i in Type.AllInterfaces()
							from m in i.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod)
							where BuildRule.FormatMethodName(m).Equals(MethodName, StringComparison.CurrentCultureIgnoreCase)
							select m).ToArray();
			if (methods.Length == 0)
				throw new ArgumentException($"在服务{Type}上找不到方法{MethodName}");
			if(methods.Length>1)
				throw new ArgumentException($"在服务{Type}上找到多个名为{MethodName}的方法：{methods.Select(m=>m.DeclaringType+"."+m.Name)}");

			var method = methods[0];
			var args = method.GetParameters();
			var idx = System.Threading.Interlocked.Increment(ref MethodArgumentTypeIdentSeed);
			var typeName = (method.DeclaringType.GetFullName() + "." + method.Name + "_" + idx).Replace('.', '_');

			var typeExpr = new TypeExpression(
				typeName,
				new SystemTypeReference(typeof(BaseMethodArgument)),
				TypeAttributes.Public | TypeAttributes.Class
				);
			foreach (var arg in args)
				typeExpr.Properties.Add(
					new PropertyExpression(
						arg.Name, 
						new SystemTypeReference(arg.ParameterType), 
						PropertyAttributes.None
						));
			var argType = DynamicTypeBuilder.Build(EnumerableEx.From(typeExpr)).Single();

			var argSvc = Expression.Parameter(typeof(object));
			var argSerializer = Expression.Parameter(typeof(IJsonSerializer));
			var argParameters= Expression.Parameter(typeof(string));
			var varArg = Expression.Variable(argType);
			var func = Expression.Lambda<Func<object, IJsonSerializer, string, object>>(
				Expression.Block(
					new[] { varArg },
					varArg.Assign(
						argSerializer.CallMethod(
							MethodDeserialize,
							argParameters,
							Expression.Constant(argType),
							Expression.Constant(null, typeof(JsonSetting))
							).To(argType)
						),
					argSvc.To(Type).CallMethod(
						method,
						args.Select(a => {
							var prop = argType.GetProperty(a.Name);
							if (prop == null)
								return Expression.Constant(a.ParameterType.GetDefaultValue(), a.ParameterType);
							else
								return varArg.GetMember(prop); 
							}
							).ToArray()
						)
					),
				argSvc,
				argSerializer,
				argParameters
				).Compile();
			return func;
		}
	}

}
