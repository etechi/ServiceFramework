using SF.Core.DI;
using System.Linq;
namespace SF.Services.Management
{
	public static class ServicveManagementDIServiceCollectionExtension
	{
		public static void SetupServices(
			this IManagedServiceCollection ManagedServiceCollection
			)
		{
			var sc = ManagedServiceCollection.NormalServiceCollection;
			var mc = new Internal.ServiceMetadata(
				sc.GetServiceTypes(),
				ManagedServiceCollection
				);

			if (!sc.GetServiceTypes().Any(t=>t== typeof(IServiceImplementTypeResolver)))
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
