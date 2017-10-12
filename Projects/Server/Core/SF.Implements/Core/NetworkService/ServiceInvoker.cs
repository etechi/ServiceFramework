using System;
using System.Collections.Generic;

using System.Linq;
using System.Collections;
using SF.Core.ServiceManagement.Internals;
using System.Linq.TypeExpressions;
using System.Reflection;
using System.Linq.Expressions;
using SF.Core.ServiceManagement;

namespace SF.Core.NetworkService
{
	public class ServiceInvoker : IServiceInvoker
	{
		IDynamicTypeBuilder DynamicTypeBuilder { get; }
		IServiceBuildRuleProvider BuildRule { get; }
		SF.Core.Serialization.IJsonSerializer JsonSerializer { get; }
		Dictionary<string,Type> ServiceTypes { get; }
		System.Collections.Concurrent.ConcurrentDictionary<(Type, string),Lazy< Func<object, SF.Core.Serialization.IJsonSerializer, string, object>>> Invokers { get; } = 
			new System.Collections.Concurrent.ConcurrentDictionary<(Type, string),Lazy< Func<object, SF.Core.Serialization.IJsonSerializer, string, object>>>();

		public ServiceInvoker(
			IServiceBuildRuleProvider BuildRule,
			IDynamicTypeBuilder DynamicTypeBuilder,
			SF.Core.Serialization.IJsonSerializer JsonSerializer,
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
				func = new Lazy<Func<object, SF.Core.Serialization.IJsonSerializer, string, object>>(
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
			typeof(SF.Core.Serialization.IJsonSerializer)
			.GetMethod(nameof(SF.Core.Serialization.IJsonSerializer.Deserialize));


		Func<object, SF.Core.Serialization.IJsonSerializer, string, object> BuildInvokeFunc(Type Type,string MethodName)
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
			var argSerializer = Expression.Parameter(typeof(SF.Core.Serialization.IJsonSerializer));
			var argParameters= Expression.Parameter(typeof(string));
			var varArg = Expression.Variable(argType);
			var func = Expression.Lambda<Func<object, SF.Core.Serialization.IJsonSerializer, string, object>>(
				Expression.Block(
					new[] { varArg },
					varArg.Assign(
						argSerializer.CallMethod(
							MethodDeserialize,
							argParameters,
							Expression.Constant(argType),
							Expression.Constant(null, typeof(SF.Core.Serialization.JsonSetting))
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
