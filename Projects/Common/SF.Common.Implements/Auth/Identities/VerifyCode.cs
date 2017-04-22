using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Auth.Identities.Models;
using SF.Auth.Identities.Internals;
using SF.Metadata;

namespace SF.Auth.Identities
{
	[Comment("验证码")]
	public class VerifyCode
	{
		public ConfirmMessageType Type { get; set; }
		public long UserId{ get; set; }
		public string Ident { get; set; }
		public string Code{ get; set; }
	}
}
