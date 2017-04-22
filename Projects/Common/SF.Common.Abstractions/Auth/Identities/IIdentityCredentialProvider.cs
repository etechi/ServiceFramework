using System.Threading.Tasks;
using SF.Auth.Identities.Models;
namespace SF.Auth.Identities
{
	public enum ConfirmMessageType
	{
		Signin,
		Signup,
		PasswordRecorvery,
		Confirm
	}
    public interface IIdentityCredentialProvider
    {
		string Name { get; }
		bool IsConfirmable();
		Task<string> VerifyFormat(string Ident);

		Task<long> SendConfirmCode(int ScopeId,string Credential, string Code, ConfirmMessageType Type, string TrackIdent);
		Task SetConfirmed(int ScopeId, string Credential, bool Confirmed);

		Task<IdentityCredential> FindOrBind(int ScopeId, string Credential, string UnionIdent,bool Confirmed,long UserId);
		Task<IdentityCredential> Find(int ScopeId, string Credential, string UnionIdent);

		Task Bind(int ScopeId, string Credential, string UnionIdent, bool Confirmed, long UserId);
		Task Unbind(int ScopeId, string Credential, long UserId);

		Task<IdentityCredential[]> GetIdents(long UserId);
	}

}
