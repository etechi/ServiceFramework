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
	[Comment("访问令牌加密密钥")]
	public class AccessTokenPassword
	{
		public byte[] Password { get; set; }
	}
}
