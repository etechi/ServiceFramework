
using SF.Core.ServiceManagement.Management;
using SF.Management.MenuServices;
using System.Linq;
using SF.Metadata;
using SF.Core.NetworkService.Metadata;
using SF.Data.Entity;
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
			sc.AddManagedScoped<ISysAdminManagementService, EntitySysAdminManagementService<TSysAdmin>>();
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
			sc.AddManagedScoped<ISysAdminService, SysAdminService>();
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
				);
		public static IServiceInstanceInitializer<ISysAdminService> NewSysAdminService(
			this IServiceInstanceManager sim,
			IServiceInstanceInitializer<ISysAdminManagementService> SysAdminManagementService = null,
			IServiceInstanceInitializer<IIdentityService> IdentityService = null
			) =>
			sim.NewSysAdminService<SF.Management.SysAdmins.Entity.DataModels.SysAdmin>(SysAdminManagementService, IdentityService);


	}
}