using SF.Data;
using SF.Data.Models;
using SF.KB;
using SF.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SF.Management.BizAdmins.Models
{
	public class BizAdminInternal : ObjectEntityBase
	{
		[Comment("账号")]
		[Required]
		[MaxLength(100)]
		public string Account { get; set; }

	}
}

