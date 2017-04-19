using SF.System.Auth.Identity.Models;
using System;
using System.Threading.Tasks;

namespace SF.System.Auth.Identity.Internals
{
	public class IdentCreateArgument
	{
		public int ScopeId { get; set; }
		public UserDesc Desc { get; set; }
		public string PasswordHash { get; set; }
		public string SecurityStamp { get; set; }
		public Clients.IAccessSource AccessInfo { get; set; }
	}
	public interface IIdentStorage
	{
		Task<UserDesc> FindById(long Id);
		Task UpdateAsync(UserDesc Desc);

		Task<long> Create(IdentCreateArgument Arg);

		Task<string> GetPasswordHash(long Id);
		Task SetPasswordHash(long Id, string PasswordHash,string SecurityStamp);

	}

}

