using SF.Auth.Identities.Models;
using System.Threading.Tasks;

namespace SF.Auth.Identities.Internals
{
	public interface IIdentBindStorage
	{
		Task<IdentityCredential> FindOrBind(
			int ScopeId,
			string ProviderId,
			string Ident,
			string UnionIdent,
			bool Confirmed,
			long UserId
			);
		Task<IdentityCredential> Find(
			int ScopeId,
			string ProviderId,
			string Ident,
			string UnionIdent
			);

		Task Bind(
			int ScopeId,
			string ProviderId,
			string Ident,
			string UnionIdent,
			bool Confirmed,
			long UserId
			);
		Task Unbind(
			int ScopeId,
			string ProviderId,
			string Ident,
			long UserId
			);

		Task SetConfirmed(
			int ScopeId,
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
