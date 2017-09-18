﻿using SF.Data;
using SF.Data.Models;
using SF.Entities;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Auth.Permissions.DataModels
{
	public class Role : Role<Grant, Role, GrantRole, RolePermission, GrantPermission>
	{
	}

	[Table("AuthRole")]
    [Comment(GroupName = "授权服务", Name = "角色")]
    public class Role<TGrant,TRole, TGrantRole, TRolePermission, TGrantPermission> :
		ObjectEntityBase<string>
		where TGrant : Grant<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>
		where TRole : Role<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>
		where TRolePermission : RolePermission<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>
		where TGrantRole : GrantRole<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>
		where TGrantPermission : GrantPermission<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>
	{

		[InverseProperty(nameof(RolePermission<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>.Role))]
		public ICollection<TRolePermission> Permissions { get; set; }

		[InverseProperty(nameof(GrantRole<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>.Role))]
		public ICollection<TGrantRole> Grants { get; set; }

	}


}
