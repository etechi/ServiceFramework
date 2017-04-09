using SF.Auth.Passport;
using SF.Auth.Users.Models;
using System;
using System.Threading.Tasks;

namespace SF.Auth.Users.Internals
{
	public class UserCreateArgument
	{
		public UserInfo User { get; set; }
		public string PasswordHash { get; set; }
		public string SecurityStamp { get; set; }
		public Clients.AccessInfo AccessInfo { get; set; }
	}
	public interface IUserStorage
	{
		Task<UserInfo> FindById(long UserId);
		Task UpdateAsync(UserInfo User);

		Task<long> Create(UserCreateArgument Arg);

		Task<string> GetPasswordHash(long UserId,bool ForSignin);
		Task SetPasswordHash(long UserId, string PasswordHash,string SecurityStamp);
		Task SigninSuccess(long UserId,Clients.AccessInfo AccessInfo);
		Task SigninFailed(long UserId, int LockoutFailedCount ,TimeSpan LockoutTime, Clients.AccessInfo AccessInfo);

	}

}

