#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

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
				"系统服务",
				d => d.AddUnmanaged<Management.IServiceDeclarationManager, Management.ServiceDeclarationManager>("SysServiceDeclaration","系统服务定义")
					.AddUnmanaged<Management.IServiceImplementManager, Management.ServiceImplementManager>("SysServiceImplement", "系统服务实现")
					.AddUnmanaged<Management.IServiceInstanceManager, Management.ServiceInstanceManager>("SysServiceInstance", "系统服务实例")
					);

			sc.AddScoped<IServiceConfigLoader, Storages.DBServiceSource>();
			sc.AddScoped<IServiceInstanceLister, Storages.DBServiceSource>();

			sc.AddDefaultMenuItems(
				"default",
				"系统管理/系统服务",
				new SF.Management.MenuServices.MenuItem {
					Name = "系统服务定义",
					Action = SF.Management.MenuServices.MenuActionType.EntityManager,
					ActionArgument = "SysServiceDeclaration"
				},
				new SF.Management.MenuServices.MenuItem
				{
					Name = "系统服务实现",
					Action = SF.Management.MenuServices.MenuActionType.EntityManager,
					ActionArgument = "SysServiceImplement"
				},
				new SF.Management.MenuServices.MenuItem
				{
					Name = "系统服务实例",
					Action = SF.Management.MenuServices.MenuActionType.EntityManager,
					ActionArgument = "SysServiceInstance"
				}
				);
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
