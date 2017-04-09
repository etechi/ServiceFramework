using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;

namespace SF.Auth.Users.DataModels
{
	[Table("SysUserIdent")]
	public class UserIdent
	{

		[Key]
		[Column(Order = 1)]
		[Comment("功能ID")]
		[Index("union", Order = 1)]
		public long FeatureId { get; set; }

		[Key]
		[Column(Order =2)]
		[Index("union",Order=2)]
		[MaxLength(50)]
		public string Provider { get; set; }

		[Key]
		[Column(Order = 3)]
		[MaxLength(100)]
		public string Ident { get; set; }

		[Index]
		[Comment("应用ID")]
		public long AppId { get; set; }

		[Index]
		[Comment("用户ID")]
		public long UserId { get; set; }

		[Index("union", Order = 3)]
		public string UnionIdent { get; set; }

		public DateTime BindTime { get; set; }
		public DateTime? ConfirmedTime { get; set; }
	}

}
