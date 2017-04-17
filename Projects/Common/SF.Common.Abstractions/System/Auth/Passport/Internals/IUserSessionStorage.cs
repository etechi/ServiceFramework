using System;
using System.Threading.Tasks;

namespace SF.System.Auth.Passport.Internals
{
	public interface IUserSessionStorage
	{
		Task<long> Create(long UserId,DateTime? Expires,Clients.AccessInfo AccessInfo);
		Task Update(long Id);
	}

}

