using System;

using System.Collections.Generic;
using SF.Core.ServiceManagement.Internals;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using SF.Metadata;

namespace SF.Services.Security
{
	public class PasswordHasher : IPasswordHasher
	{
		public byte[] GlobalPassword { get; }
		public PasswordHasher(
			[Comment("全局密钥")]
			string GlobalPassword
			)
		{
			this.GlobalPassword = GlobalPassword.UTF8Bytes();
		}
		public string Hash(string Password,byte[] SecurityStamp)
		{
			return Password.UTF8Bytes().Concat(SecurityStamp, GlobalPassword).Sha1().Hex();
		}
	}
}
