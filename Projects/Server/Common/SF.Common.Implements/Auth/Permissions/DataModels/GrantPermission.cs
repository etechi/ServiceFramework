using SF.Data;
using SF.Metadata;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Auth.Permissions.DataModels
{
	public class GrantPermission : GrantPermission<Grant,Role, GrantRole, RolePermission, GrantPermission>
	{
	}
	[Table("AuthGrantPermission")]
    [Comment(GroupName = "授权服务", Name = "授权权限")]
    public class GrantPermission<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission> :
		IPermission
		where TGrant : Grant<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>
		where TRole : Role<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>
		where TRolePermission : RolePermission<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>
		where TGrantRole : GrantRole<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>
		where TGrantPermission : GrantPermission<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>
	{
        [Key]
		[Column(Order =1)]
        [Display(Name ="授权ID")]
		public long GrantId { get; set; }

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

		[ForeignKey(nameof(GrantId))]
		public TGrant Grant { get; set; }
	}
	
}
