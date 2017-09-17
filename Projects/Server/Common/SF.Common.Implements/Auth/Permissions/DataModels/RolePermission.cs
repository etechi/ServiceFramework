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
	public class RolePermission : RolePermission<Role, IdentityRole, RolePermission, IdentityPermission>
	{
	}
	[Table("AuthRolePermission")]
    [Comment(GroupName = "认证服务", Name = "角色权限")]
    public class RolePermission<TRole, TIdentityRole, TRolePermission, TIdentityPermission>:
		IPermission
		where TRole : Role<TRole, TIdentityRole, TRolePermission, TIdentityPermission>
		where TRolePermission : RolePermission<TRole, TIdentityRole, TRolePermission, TIdentityPermission>
		where TIdentityRole : IdentityRole<TRole, TIdentityRole, TRolePermission, TIdentityPermission>
		where TIdentityPermission : IdentityPermission<TRole, TIdentityRole, TRolePermission, TIdentityPermission>
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
