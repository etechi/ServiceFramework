using SF.Data;
using SF.Metadata;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Auth.Permissions.DataModels
{
	public class GrantRole : GrantRole<Grant, Role, GrantRole, RolePermission, GrantPermission>
	{
	}
	[Table("AuthGrantRole")]
    [Comment(GroupName = "授权服务", Name = "授权角色")]
    public class GrantRole<TGrant,TRole, TGrantRole, TRolePermission, TGrantPermission>
		where TGrant :Grant<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>
		where TRole : Role<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>
		where TRolePermission : RolePermission<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>
		where TGrantRole : GrantRole<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>
		where TGrantPermission : GrantPermission<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>
	{
        [Key]
		[Column(Order = 1)]
        [Display(Name = "用户ID")]
        public long GrantId { get; set; }

		[Key]
		[Column(Order =2)]
		[Index]
		[MaxLength(100)]
		[Required]
        [Display(Name = "角色ID")]

        public string RoleId { get; set; }

		[ForeignKey(nameof(RoleId))]
		public TRole Role { get; set; }

		[ForeignKey(nameof(GrantId))]
		public TGrant Grant { get; set; }


	}


}
