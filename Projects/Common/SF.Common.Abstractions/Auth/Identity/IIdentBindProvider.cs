using System.Threading.Tasks;
using SF.Auth.Identity.Models;
namespace SF.Auth.Identity
{
	public enum ConfirmMessageType
	{
		Signin,
		Signup,
		PasswordRecorvery,
		Confirm
	}
    public interface IIdentBindProvider
    {
		string Name { get; }
		bool IsConfirmable();
		Task<string> VerifyFormat(string Ident);

		Task<long> SendConfirmCode(int ScopeId,string Ident, string Code, ConfirmMessageType Type, string TrackIdent);
		Task SetConfirmed(int ScopeId, string Ident, bool Confirmed);

		Task<IdentBind> FindOrBind(int ScopeId, string Ident, string UnionIdent,bool Confirmed,long UserId);
		Task<IdentBind> Find(int ScopeId, string Ident,string UnionIdent);

		Task Bind(int ScopeId, string Ident, string UnionIdent, bool Confirmed, long UserId);
		Task Unbind(int ScopeId, string Ident, long UserId);

		Task<IdentBind[]> GetIdents(long UserId);
	}

}
