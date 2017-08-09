using System;
using SF.Data;
using SF.Metadata;
using System.Collections.Generic;

namespace SF.Auth.Identities.Models
{
	public class IdentityInternal : Identity
	{
		[Comment("创建标识")]
		public string CreateCredential  { get; set; }

		[Comment("创建标识提供者")]
		public long CreateCredentialProviderId { get; set; }

		[Comment("创建时间")]
		public DateTime CreateTime { get; set; }
		[Comment("更新时间")]
		public DateTime UpdateTime { get; set; }

		[Comment("对象状态")]
		public LogicObjectState ObjectState { get; set; }
	}
	public class IdentityEditable : IdentityInternal
	{
		public string PasswordHash { get; set; }
		public string SecurityStamp { get; set; }
		public ICollection<IdentityCredential> Credentials { get; set; }
	}
}

