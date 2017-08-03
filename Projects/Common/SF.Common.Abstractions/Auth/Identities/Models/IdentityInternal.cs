using System;
using SF.Data;
using SF.Metadata;

namespace SF.Auth.Identities.Models
{
	public class IdentityInternal : Identity
	{
		[Comment("创建标识")]
		public string CreateIdent { get; set; }
		[Comment("创建时间")]
		public DateTime CreateTime { get; set; }
		[Comment("更新时间")]
		public DateTime UpdateTime { get; set; }
	}

}

