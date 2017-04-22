using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;
using SF.KB;
using SF.Auth.Identities.Models;
using System.Collections.Generic;

namespace SF.Auth.Identities.DataModels
{
	[Table("SysAuthIdentity")]
	public class Identity: IObjectWithId<long>
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[Comment("ID")]
		public long Id { get; set; }

		[Index]
		[Comment("应用ID")]
		public long AppId { get; set; }

		[Index]
		[Comment("功能ID")]
		public long ScopeId { get; set; }

		[MaxLength(100)]
		[Comment("名称")]
		public virtual string Name { get; set; }

		[MaxLength(100)]
		[Comment("图标")]
		public virtual string Icon { get; set; }

		[MaxLength(100)]
		[Comment("密码哈希")]
		public virtual string PasswordHash { get; set; }

		[Comment("逻辑状态")]
		public virtual LogicObjectState ObjectState { get; set; }

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
		[MaxLength(50)]
		public string SignupIdentProvider { get; set; }

		[MaxLength(200)]
		[Index(Order = 2)]
		[Comment("注册标识值")]
		public string SignupIdentValue { get; set; }



		[InverseProperty(nameof(IdentityCredential.Identity))]
		public ICollection<IdentityCredential> IdentBinds { get; set; }
	}
}
