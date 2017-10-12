using System.Collections.Generic;
using System.Linq;
using System;
using SF.Metadata;
using System.Reflection;
using SF.Core.Serialization;
using SF.Core.NetworkService;

namespace SF.Core.ServiceManagement
{
	public static class NetworkServiceDICollectionExtension
	{
		public static void AddNetworkService(
					this IServiceCollection sc,
					IEnumerable<Type> Services=null
					)
		{
			sc.AddSingleton<IServiceBuildRuleProvider, DefaultServiceBuildRuleProvider>();
			sc.AddSingleton(sp => {
				var builder = new ServiceMetadataBuilder(
					sp.Resolve<IServiceBuildRuleProvider>(),
					sp.Resolve<IJsonSerializer>()
					);
				foreach (var ests in sp.TryResolve<IEnumerable<IExtraServiceTypeSource>>())
					ests.AddExtraServiceType(builder);
				return builder.BuildLibrary(sp.Resolve<IServiceTypeCollection>().Types);
			});
			sc.AddScoped<IServiceMetadataService, DefaultServiceMetadataService>();
			sc.AddSingleton<IServiceTypeCollection>(sp =>
			{
				var ServiceTypes = (from svc in (Services ??
					sc.GetServiceTypes())
					where svc.AllInterfaces().Any(i=>i.GetCustomAttribute<NetworkServiceAttribute>() != null)
					select svc
					).ToArray();
				return new ServiceTypeCollection(ServiceTypes);
			});

			sc.AddSingleton<IServiceInvoker, ServiceInvoker>();
		}
	}

}
