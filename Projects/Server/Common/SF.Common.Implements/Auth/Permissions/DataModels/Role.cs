using SF.Data;
using SF.Data.Models;
using SF.Entities;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Auth.Permissions.DataModels
{
	public class Role : Role<Role, IdentityRole, RolePermission, IdentityPermission>
	{
	}

	[Table("AuthRole")]
    [Comment(GroupName = "认证服务", Name = "角色")]
    public class Role<TRole, TIdentityRole, TRolePermission, TIdentityPermission> :
		ObjectEntityBase<string>
		where TRole : Role<TRole, TIdentityRole, TRolePermission, TIdentityPermission>
		where TRolePermission : RolePermission<TRole, TIdentityRole, TRolePermission, TIdentityPermission>
		where TIdentityRole : IdentityRole<TRole, TIdentityRole, TRolePermission, TIdentityPermission>
		where TIdentityPermission : IdentityPermission<TRole, TIdentityRole, TRolePermission, TIdentityPermission>
	{

		[InverseProperty(nameof(RolePermission<TRole, TIdentityRole, TRolePermission, TIdentityPermission>.Role))]
		public ICollection<TRolePermission> Permissions { get; set; }

		[InverseProperty(nameof(IdentityRole<TRole, TIdentityRole, TRolePermission, TIdentityPermission>.Role))]
		public ICollection<TIdentityRole> Identities { get; set; }

	}


}
