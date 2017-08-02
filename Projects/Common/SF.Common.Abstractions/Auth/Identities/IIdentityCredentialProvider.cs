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
		long Id { get; }
		string Name { get; }
		bool IsConfirmable();
		Task<string> VerifyFormat(string Ident);

		Task<long> SendConfirmCode(string Credential, string Code, ConfirmMessageType Type, string TrackIdent);
		Task SetConfirmed(string Credential, bool Confirmed);

		Task<IdentityCredential> FindOrBind(string Credential, string UnionIdent,bool Confirmed,long UserId);
		Task<IdentityCredential> Find(string Credential, string UnionIdent);

		Task Bind(string Credential, string UnionIdent, bool Confirmed, long UserId);
		Task Unbind(string Credential, long UserId);

		Task<IdentityCredential[]> GetIdents(long UserId);
	}

}
