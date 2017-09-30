using SF.Metadata;
using System.Threading.Tasks;
using SF.Auth;
using System;
using System.Linq;
using System.Collections.Generic;
using SF.Entities;

namespace SF.Auth.Permissions
{
	public static class RoleManagerExtension
	{
		public static Task<TRole> RoleEnsure<TRole, TRoleQueryArgument>(
			this IRoleManager<TRole, TRoleQueryArgument> RoleManager,
			string id,
			string name,
			string desc
			)
			where TRole : Models.RoleInternal, new()
			where TRoleQueryArgument : RoleQueryArgument
			=> RoleManager.RoleEnsure(id, name, desc, null);

		public static async Task<TRole> RoleEnsure<TRole,TRoleQueryArgument>(
			this IRoleManager<TRole, TRoleQueryArgument> RoleManager, 
			string id, 
			string name, 
			string desc,
			params (string, string[])[] grants
			)
			where TRole:Models.RoleInternal,new()
			where TRoleQueryArgument:RoleQueryArgument
		{
			return await RoleManager.EnsureEntity(
				ObjectKey.From(id),
				(TRole r) =>
				{
					r.Id = id;
					r.Name = name;
					r.Description = desc;
					if (grants != null)
					{
						r.Grants = grants.Select(g => new SF.Auth.Permissions.Models.ResourceGrantInternal
						{
							Id = g.Item1,
							OperationIds = g.Item2
						}).ToArray();
					}
				});
		}
		
	}

}

