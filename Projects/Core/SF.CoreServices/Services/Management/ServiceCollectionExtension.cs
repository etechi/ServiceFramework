using SF.DI;
using System.Linq;
namespace SF.Services.Management
{
	public static class ServiceCollectionExtension
	{
		public static void UseManagedService(
			this IDIServiceCollection sc, 
			IManagedServiceCollection ManagedServiceCollection
			)
		{
			var mc = new Internal.ServiceMetadata(
				sc.ServiceTypes,
				ManagedServiceCollection
				);

			if (!sc.ServiceTypes.Any(t=>t== typeof(IServiceImplementTypeResolver)))
				sc.AddSingleton<IServiceImplementTypeResolver, DefaultServiceImplementTypeResolver>();

			var factory = new Internal.ManagedServiceFactory(mc);

			sc.AddScoped<IManagedServiceScope>(sp => new Internal.ManagedServiceScope(factory));
			sc.AddSingleton<IManagedServiceConfigChangedNotifier>(factory);

			foreach (var type in mc.ManagedServices.Select(p=>p.Key))
				sc.Add(
					new ServiceDescriptor(
						type,
						isp => isp.Resolve<IManagedServiceScope>().Resolve(isp, type, null),
						ServiceLifetime.Scoped
					)
				);
		}
		public static void UseMemoryManagedServiceSource(this IDIServiceCollection sc)
		{
			sc.AddScoped<IServiceConfigLoader, MemoryServiceSource>();
			sc.AddScoped<IDefaultServiceLocator>(isp => (IDefaultServiceLocator)isp.Resolve<IServiceConfigLoader>());
		}
	}

}
