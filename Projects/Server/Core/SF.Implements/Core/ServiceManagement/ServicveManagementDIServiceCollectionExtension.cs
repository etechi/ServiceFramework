using System.Linq;
using SF.Metadata;
using System;
using SF.Core.ServiceManagement.Internals;
using SF.Core.ServiceManagement.Storages;
using SF.Core.Hosting;

namespace SF.Core.ServiceManagement
{
	public static class ServicveManagementDIServiceCollectionExtension
	{
		public static IServiceCollection AddManagedService(
			this IServiceCollection sc
			)
		{
			//sc.AddSingleton<IServiceFactoryManager, ServiceFactoryManager>();
			//sc.AddScoped<IServiceInstanceScope,ServiceInstanceScope>();
			sc.AddSingleton<IServiceDeclarationTypeResolver, DefaultServiceDeclarationTypeResolver>();
			sc.AddSingleton<IServiceImplementTypeResolver, DefaultServiceImplementTypeResolver>();
			sc.AddSingleton<NetworkService.IExtraServiceTypeSource, ImplementConfigTypeSource>();
			return sc;
		}
		public static void UseMemoryManagedServiceSource(this IServiceCollection sc)
		{
			sc.AddSingleton<MemoryServiceSource, MemoryServiceSource>();
			sc.AddScoped(sp => (IServiceConfigLoader)sp.Resolve<MemoryServiceSource>());
			sc.AddScoped(sp => (IServiceInstanceLister)sp.Resolve<MemoryServiceSource>());
		}
		public static void AddManagedServiceAdminServices(this IServiceCollection sc, string TablePrefix = null)
		{
			sc.AddDataModules<Management.DataModels.ServiceInstance>(TablePrefix);

			sc.EntityServices(
				"SysService",
				"ϵͳ����",
				d => d.AddUnmanaged<Management.IServiceDeclarationManager, Management.ServiceDeclarationManager>("SysServiceDeclaration","ϵͳ������")
					.AddUnmanaged<Management.IServiceImplementManager, Management.ServiceImplementManager>("SysServiceImplement", "ϵͳ����ʵ��")
					.AddUnmanaged<Management.IServiceInstanceManager, Management.ServiceInstanceManager>("SysServiceInstance", "ϵͳ����ʵ��")
					);

			sc.AddScoped<IServiceConfigLoader, Storages.DBServiceSource>();
			sc.AddScoped<IServiceInstanceLister, Storages.DBServiceSource>();
		}
		public static IServiceProvider BuildServiceResolver(
			this IServiceCollection sc,
			Caching.ILocalCache<IServiceEntry> AppServiceCache=null
			)
		{
			return new ServiceProviderBuilder().Build(
				sc, 
				AppServiceCache
				);
		}

		public static IAppInstance Build(this IAppInstanceBuilder Builder)
		{
			return Builder.Build(sc => sc.BuildServiceResolver());
		}
	}

}
