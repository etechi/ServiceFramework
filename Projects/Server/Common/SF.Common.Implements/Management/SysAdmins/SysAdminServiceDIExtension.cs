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


using SF.Core.ServiceManagement.Management;
using SF.Management.MenuServices;
using System.Linq;
using SF.Metadata;
using SF.Core.NetworkService.Metadata;
using SF.Entities;
using SF.Management.MenuServices.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using SF.Management.SysAdmins;
using SF.Management.SysAdmins.Entity;
using SF.Auth.Identities;

namespace SF.Core.ServiceManagement
{
	public static class SysAdminServiceDIExtension
	{
	
		public static IServiceCollection AddSysAdminManagementService<TSysAdmin>(
			this IServiceCollection sc,
			//Func<MenuItem[]> DefaultMenu=null,
			string TablePrefix = null
			)
			where TSysAdmin : SF.Management.SysAdmins.Entity.DataModels.SysAdmin<TSysAdmin>,new()
		{
			sc.AddDataModules<TSysAdmin>(TablePrefix);
			sc.EntityServices(
				"SysAdmin",
				"系统管理员",
				d => d.Add<ISysAdminManagementService, EntitySysAdminManagementService<TSysAdmin>>()
				);
			return sc;
		}
		public static IServiceCollection AddSysAdminManagementService(
			this IServiceCollection sc,
			string TablePrefix = null
			) =>
			sc.AddSysAdminManagementService<SF.Management.SysAdmins.Entity.DataModels.SysAdmin>(TablePrefix);

		public static IServiceCollection AddSysAdminService(this IServiceCollection sc)=>
			sc.AddManagedScoped<ISysAdminService,SysAdminService>();

		public static IServiceCollection AddSysAdminServices(this IServiceCollection sc)
		{
			sc.AddSysAdminManagementService();
			sc.AddManagedScoped<ISysAdminService, SysAdminService>(IsDataScope: true);
			return sc;
		}


		public static IServiceInstanceInitializer<ISysAdminManagementService> NewSysAdminMangementService<TSysAdmin>(
			this IServiceInstanceManager sim
			)
			where TSysAdmin: SF.Management.SysAdmins.Entity.DataModels.SysAdmin<TSysAdmin>, new()
			=> sim.DefaultService<ISysAdminManagementService, EntitySysAdminManagementService<TSysAdmin>>(
				new { }
				);

		public static IServiceInstanceInitializer<ISysAdminService> NewSysAdminService<TSysAdmin>(
			this IServiceInstanceManager sim,
			IServiceInstanceInitializer<ISysAdminManagementService> SysAdminManagementService=null,
			IServiceInstanceInitializer<IIdentityService> IdentityService = null
			)
			where TSysAdmin : SF.Management.SysAdmins.Entity.DataModels.SysAdmin<TSysAdmin>, new()
			=> sim.DefaultService<ISysAdminService, SysAdminService>(
				new { },
				SysAdminManagementService??sim.NewSysAdminMangementService<TSysAdmin>(),
				IdentityService ?? sim.NewAuthIdentityServive()
				).WithSystemAdminMenuItems("系统管理员");
		public static IServiceInstanceInitializer<ISysAdminService> NewSysAdminService(
			this IServiceInstanceManager sim,
			IServiceInstanceInitializer<ISysAdminManagementService> SysAdminManagementService = null,
			IServiceInstanceInitializer<IIdentityService> IdentityService = null
			) =>
			sim.NewSysAdminService<SF.Management.SysAdmins.Entity.DataModels.SysAdmin>(SysAdminManagementService, IdentityService);


	}
}