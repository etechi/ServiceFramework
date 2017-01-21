using ServiceProtocol;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Auth.Users
{
	public class UserCreateArgument
	{
		public UserInfo User { get; set; }
		public string PasswordHash { get; set; }
		public string SecurityStamp { get; set; }
		public ClientAccessInfo AccessInfo { get; set; }
	}
	public interface IUserProvider
	{
		Task<UserInfo> FindById(long UserId);
		Task UpdateAsync(UserInfo User);

		Task<UserInfo> Create(UserCreateArgument Arg);

		Task<string> GetPasswordHash(long UserId,bool ForSignin);
		Task SetPasswordHash(long UserId, string PasswordHash,string SecurityStamp);
		Task<UserInfo> Signin(long UserId,bool Success, ClientAccessInfo AccessInfo);

	}

}

