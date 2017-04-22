using SF.Auth.Identities.Models;
using SF.Metadata;
using System;
using System.Threading.Tasks;

namespace SF.Auth.Identities.Internals
{
	public class IdentCreateArgument
	{
		public long AppId { get; set; }
		public int ScopeId { get; set; }
		public long Id { get; set; }
		public string PasswordHash { get; set; }
		public byte[] SecurityStamp { get; set; }
		public Clients.IAccessSource AccessInfo { get; set; }
		public string IdentValue { get; set; }
		public string IdentProvider { get; set; }
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
	}
	
	public interface IIdentStorage
	{
		Task<long> Create(IdentCreateArgument Arg);
		Task<IdentityData> Load(long Id);
		Task UpdateDescription(Identity Identity);
		Task UpdateSecurity(long Id, string PasswordHash,byte[] SecurityStamp);
	}

}

