
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Auth.Users
{
    public interface IUserIdentProvider
    {
		string Name { get; }
		Task<string> Verify(string Ident);

		Task<bool> CanSendMessage();
		Task<string> SendMessage(string Ident, string Title, string Message,string TrackIdent);

		Task<UserIdent> FindOrBind(string Ident, string UnionIdent,bool Confirmed,long UserId);
		Task<UserIdent> Find(string Ident,string UnionIdent);

		Task Bind(string Ident, string UnionIdent, bool Confirmed, long UserId);
		Task Unbind(string Ident, long UserId);

		Task SetConfirmed(string Ident, bool Confirmed);

		Task<UserIdent[]> GetIdents(long UserId);
	}
}
