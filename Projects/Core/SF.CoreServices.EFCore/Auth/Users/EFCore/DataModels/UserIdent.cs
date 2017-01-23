using ServiceProtocol.Data.Entity;
using SF.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Auth.Users.EFCore.DataModels
{
	[Table("AuthUserPhoneNumberIdent")]
	public class UserPhoneNumberIdent
	{
		[MaxLength(100)]
		[Display(Name = "标识")]
		[Key]
		[Column(Order =1)]
		public virtual string Ident { get; set; }

		[Index]
		[Key]
		[Column(Order = 2)]
		public virtual long UserId { get; set; }

		[Display(Name = "创建时间")]
		public virtual DateTime CreatedTime { get; set; }

		[Display(Name = "验证时间")]
		public virtual DateTime? ConfirmedTime { get; set; }
	
	}
}
