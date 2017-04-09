using System;
using System.Threading.Tasks;

namespace SF.Auth.Passport.Internals
{
	public interface IUserSessionStorage
	{
		Task<long> Create(long UserId,DateTime? Expires,Clients.AccessInfo AccessInfo);
		Task Update(long Id);
	}

}

