using SF.Data;
using SF.Metadata;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Auth.Permissions.DataModels
{
	public class IdentityRole : IdentityRole<Role, IdentityRole, RolePermission, IdentityPermission>
	{
	}
	[Table("AuthIdentityRole")]
    [Comment(GroupName = "认证服务", Name = "用户角色")]
    public class IdentityRole<TRole, TIdentityRole, TRolePermission, TIdentityPermission> 
		where TRole : Role<TRole, TIdentityRole, TRolePermission, TIdentityPermission>
		where TRolePermission : RolePermission<TRole, TIdentityRole, TRolePermission, TIdentityPermission>
		where TIdentityRole : IdentityRole<TRole, TIdentityRole, TRolePermission, TIdentityPermission>
		where TIdentityPermission : IdentityPermission<TRole, TIdentityRole, TRolePermission, TIdentityPermission>
	{
        [Key]
		[Column(Order = 1)]
        [Display(Name = "用户ID")]
        public long IdentityId { get; set; }

		[Key]
		[Column(Order =2)]
		[Index]
		[MaxLength(100)]
		[Required]
        [Display(Name = "角色ID")]

        public string RoleId { get; set; }

		[ForeignKey(nameof(RoleId))]
		public TRole Role { get; set; }

	}


}
