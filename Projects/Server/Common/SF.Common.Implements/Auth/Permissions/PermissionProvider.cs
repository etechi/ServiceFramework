using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Entities;
using SF.Data;
using SF.Auth.Permissions.DataModels;
using SF.Auth.Permissions.Models;
using SF.Core.Caching;
using SF.Core.ServiceManagement;
using SF.Core.Events;

namespace SF.Auth.Permissions
{
	public class PermissionProvider :
		   PermissionProvider<
			   GrantEditable,
			   RoleInternal,
			   DataModels.Grant,
			   DataModels.Role,
			   DataModels.RolePermission,
			   DataModels.GrantRole,
			   DataModels.GrantPermission
			   >
	{
		public PermissionProvider(
			IScoped<(IDataSet<GrantPermission>, IDataSet<GrantRole>)> WithNewScope, 
			ILocalCache<IPermission[]> LocalCache, 
			IEventSubscriber<EntityModified<GrantEditable>> GrantModified, 
			IEventSubscriber<EntityModified<RoleInternal>> RoleModified) : 
			base(WithNewScope, LocalCache, GrantModified, RoleModified)
		{
		}
	}

	public class PermissionProvider<TGrantEditable, TRoleEditable, TGrant, TRole, TRolePermission, TGrantRole, TGrantPermission> :
		IPermissionProvider
		where TGrantEditable:GrantEditable
		where TRoleEditable : RoleInternal
		where TGrant : DataModels.Grant<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>
		where TRole : DataModels.Role<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>, new()
		where TRolePermission : DataModels.RolePermission<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>, new()
		where TGrantRole : DataModels.GrantRole<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>
		where TGrantPermission : DataModels.GrantPermission<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>
	{
		ILocalCache<IPermission[]> LocalCache { get; }
		IScoped<(IDataSet<TGrantPermission>,IDataSet<TGrantRole>)> WithNewScope { get; }
		public PermissionProvider(
			IScoped<(IDataSet<TGrantPermission>, IDataSet<TGrantRole>)> WithNewScope, 
			ILocalCache<IPermission[]> LocalCache,
			IEventSubscriber<EntityModified< TGrantEditable>> GrantModified,
			IEventSubscriber<EntityModified<TRoleEditable>> RoleModified
			)
		{
			this.LocalCache = LocalCache;
			this.WithNewScope = WithNewScope;
			GrantModified.Wait(e =>
			{
				LocalCache.Remove(e.Entity.Id.ToString());
				return Task.CompletedTask;
			});
			RoleModified.Wait(e =>
			{
				//清除所有
				LocalCache.Clear();
				return Task.CompletedTask;
			});
		}

		public async Task<IPermission[]> GetPermissions(long OperatorId)
		{
			var id = OperatorId.ToString();
			var re = LocalCache.Get(id);
			if (re != null)
				return re;

			re = await WithNewScope.Use(async (svcs)=>
			{
				var (Grants, Roles) = svcs;
				 return await Grants
					 .AsQueryable()
					 .Where(p => p.GrantId == OperatorId)
					 .Select(p => new Permission { OperationId = p.OperationId, ResourceId = p.ResourceId })
					 .Union(
						 Roles.AsQueryable().Where(gr => gr.GrantId == OperatorId).SelectMany(gr => gr.Role.Permissions)
						 .Select(p => new Permission { OperationId = p.OperationId, ResourceId = p.ResourceId })
					 ).ToArrayAsync();
			 });
			return LocalCache.AddOrGetExisting(id, re, TimeSpan.FromMinutes(30));
			
		}
		
	}
}
