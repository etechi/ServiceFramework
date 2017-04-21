using SF.Auth.Identity.Models;
using System.Threading.Tasks;

namespace SF.Auth.Identity.Internals
{
	public interface IIdentBindStorage
	{
		Task<IdentBind> FindOrBind(
			int ScopeId,
			string ProviderId,
			string Ident,
			string UnionIdent,
			bool Confirmed,
			long UserId
			);
		Task<IdentBind> Find(
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

		Task<IdentBind[]> GetIdents(
			string ProviderId,
			long UserId
			);
	}
}
