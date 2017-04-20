using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Auth.Identity.Models;
using SF.Auth.Identity.Internals;
using SF.Metadata;

namespace SF.Auth.Identity
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
