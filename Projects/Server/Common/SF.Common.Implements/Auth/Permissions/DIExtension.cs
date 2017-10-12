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
using SF.Auth.Permissions.Models;
using SF.Auth.Permissions;
using SF.Auth;

namespace SF.Core.ServiceManagement
{
	public static class PermissionsDIExtension
	{
	
		public static IServiceCollection AddAuthPermissions(this IServiceCollection sc)
		{
			sc.AddSingleton<IReadOnlyDictionary<string, ResourceInternal>, ResourceDictionary>();
			sc.AddSingleton<IReadOnlyDictionary<string, OperationInternal>, OperationDictionary>();
			sc.AddScoped<IResourceManager, ResourceManager>();
			sc.AddScoped<IOperationManager, OperationManager>();

			sc.AddManagedScoped<IRoleManager, RoleManager>();
			sc.AddManagedScoped<IGrantManager, GrantManager>();

			sc.AddSingleton<IPermissionProvider, PermissionProvider>();
			return sc;
		}
	}
}