using SF.Core.DI;
using System.Collections.Generic;
using System.Linq;
using System;
using SF.Metadata;
using System.Reflection;
using SF.Core.Serialization;
using SF.Services.NetworkService;

namespace SF.Core.DI
{
	public static class NetworkServiceDICollectionExtension
	{
		public static void UseNetworkService(
					this IDIServiceCollection sc,
					IEnumerable<Type> Services=null
					)
		{
			Services = Services ?? sc.ServiceTypes.Where(st => st.GetCustomAttribute<NetworkServiceAttribute>() != null);
			sc.AddSingleton<IServiceBuildRuleProvider, DefaultServiceBuildRuleProvider>();
			sc.AddSingleton(sp =>
				new ServiceMetadataBuilder(
					sp.Resolve<IServiceBuildRuleProvider>(),
					sp.Resolve<IJsonSerializer>()
					).BuildLibrary(Services)
					);
			sc.AddScoped<IServiceMetadataService, DefaultServiceMetadataService>();
			sc.AddSingleton<IServiceTypeCollection>(sp =>
			{
				var ServiceTypes = (Services ??
					sc.ServiceTypes
					.Where(st => st.GetCustomAttribute<NetworkServiceAttribute>() != null)
					).ToArray();
				return new ServiceTypeCollection(ServiceTypes);
			});
		}
	}

}
