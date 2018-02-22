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

using System;
using SF.Sys.Services.Internals;
using SF.Sys.Services.Storages;

namespace SF.Sys.Services
{
	public static class ServicveManagementDIServiceCollectionExtension
	{
		public static void UseMemoryManagedServiceSource(this IServiceCollection sc)
		{
			sc.AddSingleton<MemoryServiceSource, MemoryServiceSource>();
			sc.AddScoped(sp => (IServiceConfigLoader)sp.Resolve<MemoryServiceSource>());
			sc.AddScoped(sp => (IServiceInstanceLister)sp.Resolve<MemoryServiceSource>());
		}
		public static void AddManagedServiceAdminServices(this IServiceCollection sc, string TablePrefix = null)
		{
			sc.AddDataModules<Management.DataModels.DataServiceInstance>(TablePrefix ?? "Sys");

			sc.EntityServices(
				"SysService",
				"ϵͳ����",
				d => d.AddUnmanaged<Management.IServiceDeclarationManager, Management.ServiceDeclarationManager>("SysServiceDeclaration","ϵͳ������")
					.AddUnmanaged<Management.IServiceImplementManager, Management.ServiceImplementManager>("SysServiceImplement", "ϵͳ����ʵ��")
					.AddUnmanaged<Management.IServiceInstanceManager, Management.ServiceInstanceManager>("SysServiceInstance", "ϵͳ����ʵ��")
					);

			sc.AddScoped<IServiceConfigLoader, Storages.DBServiceSource>();
			sc.AddScoped<IServiceInstanceLister, Storages.DBServiceSource>();

			sc.AddDefaultMenuItems(
				"default",
				"1000:ϵͳ����/ϵͳ����",
				new SF.Sys.BackEndConsole.MenuItem {
					Name = "ϵͳ������",
					Action = SF.Sys.BackEndConsole.MenuActionType.EntityManager,
					ActionArgument = "SysServiceDeclaration"
				},
				new SF.Sys.BackEndConsole.MenuItem
				{
					Name = "ϵͳ����ʵ��",
					Action = SF.Sys.BackEndConsole.MenuActionType.EntityManager,
					ActionArgument = "SysServiceImplement"
				},
				new SF.Sys.BackEndConsole.MenuItem
				{
					Name = "ϵͳ����ʵ��",
					Action = SF.Sys.BackEndConsole.MenuActionType.EntityManager,
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

	
	}

}
