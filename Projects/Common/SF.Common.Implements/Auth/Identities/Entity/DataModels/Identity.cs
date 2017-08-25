using SF.Data;
using SF.Entities;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Auth.Identities.Entity.DataModels
{

	[Table("SysAuthIdentity")]
	public class Identity<TIdentity,TIdentityCredential>: IEntityWithId<long>
		where TIdentity: Identity<TIdentity, TIdentityCredential>
		where TIdentityCredential : IdentityCredential<TIdentity, TIdentityCredential>
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[Comment("ID")]
		public long Id { get; set; }


		[Index]
		[Comment("功能ID")]
		public long ScopeId { get; set; }

		[MaxLength(100)]
		[Comment("名称")]
		[Required]
		public virtual string Name { get; set; }

		[MaxLength(100)]
		[Comment("图标")]
		public virtual string Icon { get; set; }

		[MaxLength(100)]
		[Comment("身份类别")]
		[Required]
		[Index]
		public virtual string Entity { get; set; }

		[MaxLength(100)]
		[Comment("密码哈希")]
		[Required]
		public virtual string PasswordHash { get; set; }

		[Comment("逻辑状态")]
		public virtual EntityLogicState ObjectState { get; set; }

		[MaxLength(100)]
		[Required]
		[Comment("安全标识")]
		public virtual string SecurityStamp { get; set; }

		[Index]
		[Comment("创建时间")]
		public virtual DateTime CreatedTime { get; set; }
		[Comment("更新时间")]
		public virtual DateTime UpdatedTime { get; set; }

		[Index(Order = 1)]
		[Comment("注册标识类型")]
		[Required]
		public long SignupIdentProviderId { get; set; }

		[MaxLength(200)]
		[Index(Order = 2)]
		[Comment("注册标识值")]
		[Required]
		public string SignupIdentValue { get; set; }



		[InverseProperty(nameof(IdentityCredential<TIdentity,TIdentityCredential>.Identity))]
		public ICollection<TIdentityCredential> Credentials { get; set; }
	}
	public class Identity : Identity<Identity, IdentityCredential>
	{ }
}
