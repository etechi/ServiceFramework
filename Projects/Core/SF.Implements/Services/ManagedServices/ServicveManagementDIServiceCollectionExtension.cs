using SF.Core.DI;
using SF.Services.ManagedServices.Runtime;
using SF.Services.ManagedServices.Storages;
using System.Linq;
using SF.Metadata;
using System;

namespace SF.Services.ManagedServices
{
	public static class ServicveManagementDIServiceCollectionExtension
	{
		
		public static IManagedServiceCollection UseManagedService(
			this IDIServiceCollection sc
			)
		{
			var msc = new ManagedServiceCollection(sc);

			sc.AddSingleton<IServiceMetadata>(sp =>
				new Runtime.ServiceMetadata(
					sc.GetServiceTypes(),
					msc
					)
					);

			if (!sc.GetServiceTypes().Any(t=>t== typeof(IServiceImplementTypeResolver)))
				sc.AddSingleton<IServiceImplementTypeResolver, DefaultServiceImplementTypeResolver>();

			sc.AddSingleton<IManagedServiceFactoryManager, Runtime.ManagedServiceFactoryManager>();

			sc.AddScoped<IManagedServiceScope,Runtime.ManagedServiceScope>();
			sc.AddSingleton<IManagedServiceConfigChangedNotifier>(sp=>(IManagedServiceConfigChangedNotifier)sp.Resolve<IManagedServiceFactoryManager>());

			sc.AddSingleton<NetworkService.IExtraServiceTypeSource, ImplementConfigTypeSource>();
			return msc;
		}
		public static void UseMemoryManagedServiceSource(this IDIServiceCollection sc)
		{
			sc.AddScoped<IServiceConfigLoader, MemoryServiceSource>();
			sc.AddScoped<IDefaultServiceLocator>(isp => (IDefaultServiceLocator)isp.Resolve<IServiceConfigLoader>());
		}
		public static void UseManagedServiceAdminServices(this IManagedServiceCollection ManagedServiceCollection, string TablePrefix = null)
		{
			var sc = ManagedServiceCollection.NormalServiceCollection;
			sc.UseDataModules<Admin.DataModels.ServiceInstance>(TablePrefix);

			sc.AddEntityService<Admin.IServiceDeclarationManager, Admin.ServiceDeclarationManager,string,Models.ServiceDeclaration>();
			sc.AddEntityService<Admin.IServiceImplementManager, Admin.ServiceImplementManager, string, Models.ServiceImplement>();
			sc.AddEntityService<Admin.IServiceInstanceManager, Admin.ServiceInstanceManager, string, Models.ServiceInstance>();

			sc.AddScoped<IServiceConfigLoader, Admin.ServiceInstanceManager>();
			sc.AddScoped<IDefaultServiceLocator, Admin.ServiceInstanceManager>();
		}
	}

}
