using System.Threading.Tasks;

namespace SF.Auth.Users
{
    public interface IUserIdentStorage
    {
		Task<UserIdent> FindOrBind(string Provider,string Ident, string UnionIdent,bool Confirmed,long UserId);
		Task<UserIdent> Find(string Provider, string Ident,string UnionIdent);

		Task Bind(string Provider, string Ident, string UnionIdent, bool Confirmed, long UserId);
		Task Unbind(string Provider, string Ident, long UserId);

		Task SetConfirmed(string Provider, string Ident, bool Confirmed);

		Task<UserIdent[]> GetIdents(string Provider, long UserId);
	}
}
