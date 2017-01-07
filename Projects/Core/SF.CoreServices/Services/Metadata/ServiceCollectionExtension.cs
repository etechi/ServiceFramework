using SF.DI;
using System.Collections.Generic;
using System.Linq;
using System;
using SF.Reflection;
using SF.Serialization;

namespace SF.Services.Metadata
{
	public static class ServiceCollectionExtension
	{
		public static void UseServiceMetadata(
					this DI.IDIServiceCollection sc,
					IEnumerable<Type> Services=null
					)
		{
			Services = Services ?? sc.ServiceTypes.Where(st => st.GetCustomAttribute<Annotations.NetworkServiceAttribute>() != null);
			sc.AddSingleton<IServiceBuildRuleProvider, DefaultServiceBuildRuleProvider>();
			sc.AddSingleton(sp =>
				new ServiceMetadataBuilder(
					sp.Resolve<IServiceBuildRuleProvider>(),
					sp.Resolve<IJsonSerializer>()
					).BuildLibrary(Services)
					);
			sc.AddScoped<IServiceMetadataService, DefaultServiceMetadataService>();
		}
	}

}
