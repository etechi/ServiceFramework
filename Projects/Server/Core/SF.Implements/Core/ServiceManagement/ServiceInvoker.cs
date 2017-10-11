using System;
using System.Collections.Generic;

using System.Linq;
using System.Collections;
using SF.Core.ServiceManagement.Internals;
using System.Linq.TypeExpressions;

namespace SF.Core.ServiceManagement
{
	public class ServiceInvoker : IServiceInvoker
	{
		IServiceProvider ServiceProvider { get; }
		IServiceMetadata ServiceMetadata { get; }
		IDynamicTypeBuilder DynamicTypeBuilder { get; }

		System.Collections.Concurrent.ConcurrentDictionary<(string, string), Func<object, string, object>> Invokers { get; } = new System.Collections.Concurrent.ConcurrentDictionary<(string, string), Func<object, string, object>>();

		public ServiceInvoker(IServiceProvider ServiceProvider, IServiceMetadata ServiceMetadata, IDynamicTypeBuilder DynamicTypeBuilder)
		{
			this.ServiceProvider = ServiceProvider;
			this.ServiceMetadata = ServiceMetadata;
			this.DynamicTypeBuilder = DynamicTypeBuilder;
		}
		(Type,object) GetService(IServiceProvider isp,string Service, string Method)
		{
			object svc;
			var resolver = isp.Resolver();
			IServiceDeclaration decl;
			if (long.TryParse(Service, out var sid))
			{
				decl = resolver.ResolveDescriptorByIdent(sid)?.ServiceDeclaration;
				if (decl == null)
					return (null,null);
				svc = resolver.ResolveServiceByIdent(sid, decl.ServiceType);
			}
			else
			{
				decl = ServiceMetadata.ServicesByTypeName.Get(Service);
				if (decl == null)
					return (null, null);
				svc = resolver.ResolveServiceByType(
					null,
					decl.ServiceType,
					null
					);
			}
			if (svc == null)
				return (null, null);
			return (decl.ServiceType, svc);

		}
		public object Invoke(string Service, string Method, string Argument)
		{
			return ServiceProvider.WithScope((isp) => {
				var (type,svc) = GetService(isp, Service, Method);
				if (svc == null)
					return null;

				var key = (Service, Method);
				if (!Invokers.TryGetValue(key, out var func))
				{
					func = BuildInvokeFunc(type, Method);
					if (func == null)
						return null;
					func = Invokers.GetOrAdd(key, func);
				}
				return func(svc, Argument);
			});
		}

		Func<object,string,object> BuildInvokeFunc(Type Type,string Method)
		{

		}
	}

}
