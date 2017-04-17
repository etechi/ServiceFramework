using SF.System.Auth.Passport;
using SF.System.Auth.Identity.Models;
using System;
using System.Threading.Tasks;

namespace SF.System.Auth.Identity.Internals
{
	public class IdentCreateArgument
	{
		public int ScopeId { get; set; }
		public IdentDesc Desc { get; set; }
		public string PasswordHash { get; set; }
		public string SecurityStamp { get; set; }
		public Clients.AccessInfo AccessInfo { get; set; }
	}
	public interface IIdentStorage
	{
		Task<IdentDesc> FindById(long Id);
		Task UpdateAsync(IdentDesc Desc);

		Task<long> Create(IdentCreateArgument Arg);

		Task<string> GetPasswordHash(long Id);
		Task SetPasswordHash(long Id, string PasswordHash,string SecurityStamp);

	}

}

