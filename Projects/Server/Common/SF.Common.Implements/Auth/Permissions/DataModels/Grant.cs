using SF.Data;
using SF.Data.Models;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Auth.Permissions.DataModels
{
	public class Grant : Grant<Grant, Role, GrantRole, RolePermission, GrantPermission>
	{
	}
	[Table("AuthGrant")]
    [Comment(GroupName = "授权服务", Name = "授权对象")]
    public class Grant<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission> :
		ObjectEntityBase<long>
		where TGrant : Grant<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>
		where TRole : Role<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>
		where TRolePermission : RolePermission<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>
		where TGrantRole : GrantRole<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>
		where TGrantPermission : GrantPermission<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>
	{
		[InverseProperty(nameof(GrantPermission<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>.Grant))]
		public ICollection<TGrantPermission> Permissions { get; set; }

		[InverseProperty(nameof(GrantRole<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>.Role))]
		public ICollection<TGrantRole> Roles { get; set; }

	}

}
