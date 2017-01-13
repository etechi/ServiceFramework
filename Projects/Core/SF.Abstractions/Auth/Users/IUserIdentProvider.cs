
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Auth.Users
{
  
    public interface IUserIdentProvider
    {
		string Id { get; }
		Task<bool> Verify(string Ident);
		Task<long?> FindUserId(string Ident);
	}
}
