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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Metadata;
using SF.Entities;
using SF.Data;

namespace SF.Auth.Permissions.DataModels
{
	public class RolePermission : RolePermission<Grant, Role, GrantRole, RolePermission, GrantPermission>
	{
	}
	[Table("AuthRolePermission")]
    [Comment(GroupName = "授权服务", Name = "角色权限")]
    public class RolePermission<TGrant, TRole, TIdentityRole, TRolePermission, TIdentityPermission>:
		IPermission
		where TGrant : Grant<TGrant, TRole, TIdentityRole, TRolePermission, TIdentityPermission>
		where TRole : Role<TGrant, TRole, TIdentityRole, TRolePermission, TIdentityPermission>
		where TRolePermission : RolePermission<TGrant, TRole, TIdentityRole, TRolePermission, TIdentityPermission>
		where TIdentityRole : GrantRole<TGrant, TRole, TIdentityRole, TRolePermission, TIdentityPermission>
		where TIdentityPermission : GrantPermission<TGrant, TRole, TIdentityRole, TRolePermission, TIdentityPermission>
	{
        [Key]
		[Column(Order =1)]
		[Required]
		[MaxLength(100)]
        [Display(Name="角色ID")]
		public virtual string RoleId { get; set; }

		[Key]
		[Column(Order = 2)]
		[Required]
		[Index]
		[MaxLength(100)]
        [Display(Name = "操作ID")]
        public virtual string OperationId { get; set; }

		[Key]
		[Column(Order = 3)]
		[Index]
		[Required]
		[MaxLength(100)]
        [Display(Name = "资源ID")]
        public virtual string ResourceId { get; set; }

		[ForeignKey(nameof(RoleId))]
		public virtual TRole Role { get; set; }


	}
	
}
