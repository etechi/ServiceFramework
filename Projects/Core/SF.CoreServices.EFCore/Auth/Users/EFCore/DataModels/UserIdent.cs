using ServiceProtocol.Data.Entity;
using SF.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Auth.Users.EFCore.DataModels
{
	[Table("AuthUserIdent")]
	public class UserIdent 
	{
		[MaxLength(100)]
		[Display(Name = "标识")]
		[Key]
		[Column(Order =1)]
		public virtual string Value { get; set; }

		[Key]
		[Display(Name ="类型")]
		[MaxLength(100)]
		[Column(Order = 1)]
		[Index("unionIndex", Order = 1)]
		public virtual string Provider { get; set; }

		[MaxLength(100)]
		[Display(Name = "统一标识")]
		[Index("unionIndex", Order = 2)]
		public virtual string UnionIdent{ get; set; }

		[Display(Name = "创建时间")]
		public virtual DateTime CreatedTime { get; set; }

		[Display(Name = "验证时间")]
		public virtual DateTime? ConfirmedTime { get; set; }
	
	}
}
