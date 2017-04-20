using SF.Auth.Identity.Models;
using System;
using System.Threading.Tasks;

namespace SF.Auth.Identity.Internals
{
	public class IdentCreateArgument
	{
		public int ScopeId { get; set; }
		public long Id { get; set; }
		public string PasswordHash { get; set; }
		public byte[] SecurityStamp { get; set; }
		public Clients.IAccessSource AccessInfo { get; set; }
	}
	public interface IIdentStorage
	{
		Task<long> Create(IdentCreateArgument Arg);
		Task<byte[]> GetSecurityStamp(long Id);
		Task<string> GetPasswordHash(long Id);
		Task SetPasswordHash(long Id, string PasswordHash,byte[] SecurityStamp);

	}

}

