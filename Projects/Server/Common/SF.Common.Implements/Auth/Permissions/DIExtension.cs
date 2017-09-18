
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

			return sc;
		}
	}
}