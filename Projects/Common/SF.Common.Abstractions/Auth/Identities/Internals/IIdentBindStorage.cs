using SF.Auth.Identities.Models;
using System.Threading.Tasks;

namespace SF.Auth.Identities.Internals
{
	public interface IIdentityCredentialStorage
	{
		Task<IdentityCredential> FindOrBind(
			string ProviderId,
			string Ident,
			string UnionIdent,
			bool Confirmed,
			long UserId
			);
		Task<IdentityCredential> Find(
			string ProviderId,
			string Ident,
			string UnionIdent
			);

		Task Bind(
			string ProviderId,
			string Ident,
			string UnionIdent,
			bool Confirmed,
			long UserId
			);
		Task Unbind(
			string ProviderId,
			string Ident,
			long UserId
			);

		Task SetConfirmed(
			string ProviderId,
			string Ident,
			bool Confirmed
			);

		Task<IdentityCredential[]> GetIdents(
			string ProviderId,
			long UserId
			);
	}
}
