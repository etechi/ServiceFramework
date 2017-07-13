using SF.Core.DI;
using System.Linq;
using SF.Metadata;
using System;
using SF.Core.ServiceManagement.Internals;
using SF.Core.ServiceManagement.Storages;

namespace SF.Core.ServiceManagement
{
	public static class ServicveManagementDIServiceCollectionExtension
	{
		public static IServiceCollection UseManagedService(
			this IServiceCollection sc
			)
		{
			//sc.AddSingleton<IServiceFactoryManager, ServiceFactoryManager>();
			//sc.AddScoped<IServiceInstanceScope,ServiceInstanceScope>();
			sc.AddSingleton<NetworkService.IExtraServiceTypeSource, ImplementConfigTypeSource>();
			return sc;
		}
		public static void UseMemoryManagedServiceSource(this IServiceCollection sc)
		{
			sc.AddScoped<IServiceConfigLoader, MemoryServiceSource>();
			sc.AddScoped<IDefaultServiceLocator>(isp => (IDefaultServiceLocator)isp.Resolve<IServiceConfigLoader>());
		}
		public static void UseManagedServiceAdminServices(this IServiceCollection sc, string TablePrefix = null)
		{
			sc.UseDataModules<Management.DataModels.ServiceInstance>(TablePrefix);

			sc.AddEntityService<Management.IServiceDeclarationManager, Management.ServiceDeclarationManager,string,Models.ServiceDeclaration>();
			sc.AddEntityService<Management.IServiceImplementManager, Management.ServiceImplementManager, string, Models.ServiceImplement>();
			sc.AddEntityService<Management.IServiceInstanceManager, Management.ServiceInstanceManager, string, Models.ServiceInstanceInternal>();

			sc.AddScoped<IServiceConfigLoader, Management.ServiceInstanceManager>();
			sc.AddScoped<IDefaultServiceLocator, Management.ServiceInstanceManager>();
		}
		public static IServiceProvider BuildServiceResolver(
			this IServiceCollection sc,
			Caching.ILocalCache<IServiceEntry> AppServiceCache=null,
			IServiceDeclarationTypeResolver ServiceDeclarationTypeResolver=null,
			IServiceImplementTypeResolver ServiceImplementTypeResolver = null
			)
		{
			return new ServiceProviderBuilder().Build(
				sc, 
				AppServiceCache, 
				ServiceDeclarationTypeResolver, 
				ServiceImplementTypeResolver
				);
		}
	}

}
