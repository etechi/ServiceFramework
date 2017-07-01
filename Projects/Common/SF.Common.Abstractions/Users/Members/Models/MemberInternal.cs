using SF.Data;
using SF.Data.Models;
using SF.KB;
using SF.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SF.Users.Members.Models
{
	[EntityObject("会员")]
	public class MemberInternal : EntityBase
	{
		[MaxLength(20)]
		[Comment("电话号码")]
		[TableVisible]
		[Required]
		public string PhoneNumber { get; set; }

	}
}

