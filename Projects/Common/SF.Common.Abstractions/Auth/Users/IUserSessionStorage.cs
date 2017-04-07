using System;
using System.Threading.Tasks;

namespace SF.Auth.Users
{
	public interface IUserSessionStorage
	{
		Task<long> Create(UserType UserType, long UserId,Clients.AccessInfo AccessInfo);
		Task Update(long Id);
	}

}

