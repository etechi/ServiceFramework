using Microsoft.Extensions.DependencyInjection;
using System.Linq;
namespace SF.ServiceManagement
{

	public static class ServiceCollectionExtension
	{
		public static void UseManagedService(
			this IServiceCollection sc, 
			IManagedServiceCollection ManagedServiceCollection
			)
		{
			var mc = new Internal.ServiceMetadata(
				sc.Select(s => s.ServiceType).Distinct(),
				ManagedServiceCollection
				);

			if (!sc.Any(s => s.ServiceType == typeof(IServiceImplementTypeResolver)))
				sc.AddSingleton<IServiceImplementTypeResolver, DefaultServiceImplementTypeResolver>();

			var factory = new Internal.ManagedServiceFactory(mc);

			sc.AddScoped<IManagedServiceScope>(sp => new Internal.ManagedServiceScope(factory));
			sc.AddSingleton<IManagedServiceConfigChangedNotifier>(factory);

			foreach (var type in mc.ManagedServices.Select(p=>p.Key))
				sc.Insert(sc.Count, new ServiceDescriptor(
					type,
					isp => isp.GetRequiredService<IManagedServiceScope>().Resolve(isp, type, null),
					ServiceLifetime.Scoped
					)
				);
		}
	}

}
