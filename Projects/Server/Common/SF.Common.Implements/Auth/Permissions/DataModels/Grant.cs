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
