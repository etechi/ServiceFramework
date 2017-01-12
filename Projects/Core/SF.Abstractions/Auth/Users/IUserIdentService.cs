
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Auth.Users
{
  
    public interface IUserIdentService
    {
		Task BindUserIdent(string IdentProviderId, string Ident, long UserId, string UnionIdent);
		Task UnbindUserIdent(string IdentProviderId, string Ident, long UserId);
		Task<UserIdent[]> QueryUserIdents(long UserId);

		Task<UserIdent> GetSignupIdent(long UserId);

		Task<long?> FindUserIdByUnionIdent(string IdentProviderId, string UnionIdent);
		Task<long?> FindUserIdByIdent(string IdentProviderId, string Ident);

	}
}
