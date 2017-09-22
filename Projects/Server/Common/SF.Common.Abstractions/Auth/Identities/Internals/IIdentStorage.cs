using SF.Auth.Identities.Models;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SF.Auth.Identities.Internals
{
	public class IdentityCreateArgument
	{
		[Comment("身份表示")]
		[Required]
		public Identity Identity { get; set; }

		[Required]
		[Comment("密码哈希")]
		public string PasswordHash { get; set; }

		[Required]
		[Comment("安全戳")]
		public byte[] SecurityStamp { get; set; }

		[Required]
		[Comment("访问源")]
		public Clients.IAccessSource AccessSource { get; set; }

		[Required]
		[Comment("登录凭证")]
		[MaxLength(100)]
		public string CredentialValue { get; set; }

		[Required]
		[Comment("登录凭证提供者")]
		[MaxLength(100)]
		public long CredentialProvider { get; set; }

		[Comment("注册附加参数")]
		[MaxLength(200)]
		public Dictionary<string,string> ExtraArgument { get; set; }
	}

	[Comment("用户身份数据")]
	public class IdentityData
	{
		public long Id { get; set; }
		public byte[] SecurityStamp { get; set; }
		public string PasswordHash { get; set; }
		public bool IsEnabled { get; set; }
		public string Name { get; set; }
		public string Icon { get; set; }
		public string Entity { get; set; }
	}
	
	public interface IIdentStorage
	{
		Task<long> Create(IdentityCreateArgument Arg);
		Task<IdentityData> Load(long Id);
		Task UpdateDescription(Identity Identity);
		Task UpdateSecurity(long Id, string PasswordHash,byte[] SecurityStamp);
	}

}

