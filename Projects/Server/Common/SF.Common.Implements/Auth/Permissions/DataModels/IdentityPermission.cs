using SF.Data;
using SF.Metadata;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Auth.Permissions.DataModels
{
	public class IdentityPermission : IdentityPermission<Role, IdentityRole, RolePermission, IdentityPermission>
	{
	}
	[Table("AuthIdentityPermission")]
    [Comment(GroupName = "认证服务", Name = "用户授权")]
    public class IdentityPermission<TRole, TIdentityRole, TRolePermission, TIdentityPermission> :
		IPermission
		where TRole : Role<TRole, TIdentityRole, TRolePermission, TIdentityPermission>
		where TRolePermission : RolePermission<TRole, TIdentityRole, TRolePermission, TIdentityPermission>
		where TIdentityRole : IdentityRole<TRole, TIdentityRole, TRolePermission, TIdentityPermission>
		where TIdentityPermission : IdentityPermission<TRole, TIdentityRole, TRolePermission, TIdentityPermission>
	{
        [Key]
		[Column(Order =1)]
        [Display(Name ="用户ID")]
		public long UserId { get; set; }

		[Key]
		[Column(Order = 2)]
		[MaxLength(100)]
		[Index]
		[Required]
        [Display(Name = "操作ID")]
        public string OperationId { get; set; }

		[Key]
		[Column(Order = 3)]
		[MaxLength(100)]
		[Index]
		[Required]
        [Display(Name = "资源ID")]
        public string ResourceId { get; set; }

	}
	
}
