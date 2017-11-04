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
using SF.Management.Admins;

using SF.Management.Admins.Models;

namespace SF.Core.ServiceManagement
{
	public static class AdminServiceDIExtension
	{
		public static IServiceCollection AddAdminManager<TService,TImplement,TInternal,TEditable,TQueryArgument,TAdmin>(
			this IServiceCollection sc,
			//Func<MenuItem[]> DefaultMenu=null,
			string TablePrefix = null
			)
			where TInternal: SF.Management.Admins.Models.AdminInternal, new()
			where TEditable : SF.Management.Admins.Models.AdminEditable, new()
			where TQueryArgument : SF.Management.Admins.AdminQueryArgument, new()
			where TAdmin : SF.Management.Admins.DataModels.Admin<TAdmin>,new()
			where TService: class,IAdminManager<TInternal, TEditable, TQueryArgument>
			where TImplement : AdminManager<TInternal, TEditable, TQueryArgument, TAdmin>, TService
		{
			sc.AddDataModules<TAdmin>(TablePrefix);
			sc.EntityServices(
				"Admin",
				"管理员",
				d => d.Add<TService, TImplement>("Admin","管理员")
				);
			return sc;
		}

		public static IServiceCollection AddAdminServices(
			this IServiceCollection sc,
			//Func<MenuItem[]> DefaultMenu=null,
			string TablePrefix = null
			)
			=> sc.AddAdminManager<
				IAdminManager, 
				AdminManager, 
				AdminInternal, 
				AdminEditable, 
				AdminQueryArgument, 
				SF.Management.Admins.DataModels.Admin
				>(TablePrefix);

		public static IServiceInstanceInitializer<TService> NewAdminService<TService, TImplement, TInternal, TEditable, TQueryArgument, TAdmin>(
			this IServiceInstanceManager sim
			)
			where TInternal : SF.Management.Admins.Models.AdminInternal, new()
			where TEditable : SF.Management.Admins.Models.AdminEditable, new()
			where TQueryArgument : SF.Management.Admins.AdminQueryArgument, new()
			where TAdmin : SF.Management.Admins.DataModels.Admin<TAdmin>, new()
			where TService : class, IAdminManager<TInternal, TEditable, TQueryArgument>
			where TImplement : AdminManager<TInternal, TEditable, TQueryArgument, TAdmin>, TService
			=> sim.DefaultService<TService, TImplement>(
				new { }
				).WithMenuItems("系统管理/管理员");

		public static IServiceInstanceInitializer<IAdminManager> NewAdminService(
			this IServiceInstanceManager sim
			)
			=> sim.DefaultService<IAdminManager, AdminManager>(
				new { }
				).WithMenuItems("系统管理/管理员");
	}
}