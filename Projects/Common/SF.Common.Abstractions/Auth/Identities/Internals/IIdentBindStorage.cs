using SF.Auth.Identities.Models;
using System.Threading.Tasks;

namespace SF.Auth.Identities.Internals
{
	public interface IIdentityCredentialStorage
	{
		Task<IdentityCredential> FindOrBind(
			long ProviderId,
			string Ident,
			string UnionIdent,
			bool Confirmed,
			long UserId
			);
		Task<IdentityCredential> Find(
			long ProviderId,
			string Ident,
			string UnionIdent
			);

		Task Bind(
			long ProviderId,
			string Ident,
			string UnionIdent,
			bool Confirmed,
			long UserId
			);
		Task Unbind(
			long ProviderId,
			string Ident,
			long UserId
			);

		Task SetConfirmed(
			long ProviderId,
			string Ident,
			bool Confirmed
			);

		Task<IdentityCredential[]> GetIdents(
			long ProviderId,
			long UserId
			);
	}
}
