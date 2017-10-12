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
using SF.Management.BizAdmins;
using SF.Management.BizAdmins.Entity;
using SF.Auth.Identities;

namespace SF.Core.ServiceManagement
{
	public static class BizAdminServiceDIExtension
	{
	
		public static IServiceCollection AddBizAdminManagementService<TBizAdmin>(
			this IServiceCollection sc,
			//Func<MenuItem[]> DefaultMenu=null,
			string TablePrefix = null
			)
			where TBizAdmin : SF.Management.BizAdmins.Entity.DataModels.BizAdmin<TBizAdmin>,new()
		{
			sc.AddDataModules<TBizAdmin>(TablePrefix);
			sc.EntityServices(
				"BizAdmin",
				"业务管理员",
				d => d.Add<IBizAdminManagementService, EntityBizAdminManagementService<TBizAdmin>>()
				);
			return sc;
		}
		public static IServiceCollection AddBizAdminManagementService(
			this IServiceCollection sc,
			string TablePrefix = null
			) =>
			sc.AddBizAdminManagementService<SF.Management.BizAdmins.Entity.DataModels.BizAdmin>(TablePrefix);

		public static IServiceCollection AddBizAdminService(this IServiceCollection sc)=>
			sc.AddManagedScoped<IBizAdminService,BizAdminService>();

		public static IServiceCollection AddBizAdminServices(this IServiceCollection sc)
		{
			sc.AddBizAdminManagementService();
			sc.AddManagedScoped<IBizAdminService, BizAdminService>();
			return sc;
		}


		public static IServiceInstanceInitializer<IBizAdminManagementService> NewBizAdminMangementService<TBizAdmin>(
			this IServiceInstanceManager sim
			)
			where TBizAdmin: SF.Management.BizAdmins.Entity.DataModels.BizAdmin<TBizAdmin>, new()
			=> sim.DefaultService<IBizAdminManagementService, EntityBizAdminManagementService<TBizAdmin>>(
				new { }
				);

		public static IServiceInstanceInitializer<IBizAdminService> NewBizAdminService<TBizAdmin>(
			this IServiceInstanceManager sim,
			IServiceInstanceInitializer<IBizAdminManagementService> BizAdminManagementService=null,
			IServiceInstanceInitializer<IIdentityService> IdentityService = null
			)
			where TBizAdmin : SF.Management.BizAdmins.Entity.DataModels.BizAdmin<TBizAdmin>, new()
			=> sim.DefaultService<IBizAdminService, BizAdminService>(
				new { },
				BizAdminManagementService??sim.NewBizAdminMangementService<TBizAdmin>(),
				IdentityService ?? sim.NewAuthIdentityServive()
				);
		public static IServiceInstanceInitializer<IBizAdminService> NewBizAdminService(
			this IServiceInstanceManager sim,
			IServiceInstanceInitializer<IBizAdminManagementService> BizAdminManagementService = null,
			IServiceInstanceInitializer<IIdentityService> IdentityService = null
			) =>
			sim.NewBizAdminService<SF.Management.BizAdmins.Entity.DataModels.BizAdmin>(BizAdminManagementService, IdentityService);


	}
}