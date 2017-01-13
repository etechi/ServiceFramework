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
		public string Password { get; set; }
		public UserIdent[] Idents { get; set; }
		public ClientAccessInfo AccessInfo { get; set; }
	}
	public interface IUserProvider
	{
		Task<UserInfo> FindById(long UserId);
		Task Update(UserInfo User);

		Task<UserInfo> Create(UserCreateArgument Arg);

		Task<string> GetPasswordHash(long UserId,bool ForSignin);
		Task SetPasswordHash(long UserId, string PasswordHash);
		Task<UserInfo> Signin(long UserId,bool Success, ClientAccessInfo AccessInfo);

		Task BindUserIdent(string IdentProviderId, string Ident, long UserId, string UnionIdent);
		Task UnbindUserIdent(string IdentProviderId, string Ident, long UserId);

		Task<UserIdent[]> GetUserIdents(long UserId);
		Task<UserIdent[]> GetUserIdentsByUnionIdent(string IdentProviderId, string UnionIdent);

		Task<long?> FindUserIdByIdent(string IdentProviderId, string Ident);


	}

}

