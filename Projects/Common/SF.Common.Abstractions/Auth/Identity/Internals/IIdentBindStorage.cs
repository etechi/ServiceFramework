using SF.Auth.Identity.Models;
using System.Threading.Tasks;

namespace SF.Auth.Identity.Internals
{
	public interface IIdentBindStorage
	{
		Task<IdentBind> FindOrBind(
			string ProviderId,
			string Ident,
			string UnionIdent,
			bool Confirmed,
			long UserId
			);
		Task<IdentBind> Find(
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

		Task<IdentBind[]> GetIdents(
			string ProviderId,
			long UserId
			);
	}
}
