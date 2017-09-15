using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Auth.Identity.Internals;
using SF.Auth.Identity.Models;

namespace SF.Auth.Identity.IdentProviders
{
	public abstract class BaseIdentProvider :
		IIdentBindProvider
	{
		protected IIdentBindStorage IdentStorage { get; }
		public BaseIdentProvider(IIdentBindStorage IdentStorage)
		{
			this.IdentStorage = IdentStorage;
		}

		public virtual Task Bind(string Ident, string UnionIdent, bool Confirmed, long UserId)
			=> IdentStorage.Bind(Name, Ident, UnionIdent, Confirmed, UserId);

		public virtual Task<bool> Confirmable()
			=> Task.FromResult(true);

		public virtual Task<IdentBind> Find(string Ident, string UnionIdent)
			=> IdentStorage.Find(Name, Ident, UnionIdent);

		public virtual Task<IdentBind> FindOrBind(string Ident, string UnionIdent, bool Confirmed, long UserId)
			=> IdentStorage.FindOrBind(Name, Ident, UnionIdent, Confirmed, UserId);

		public virtual Task<IdentBind[]> GetIdents(long UserId)
			=> IdentStorage.GetIdents(Name, UserId);

		public virtual Task SetConfirmed(string Ident, bool Confirmed)
			=> IdentStorage.SetConfirmed(Name, Ident, Confirmed);

		public virtual Task Unbind(string Ident, long UserId)
			=> IdentStorage.Unbind(Name, Ident, UserId);

		public abstract string Name { get; }
		public abstract Task<string> VerifyFormat(string Ident);
		public abstract Task<string> SendConfirmCode(string Ident, string Title, string Message, string TrackIdent);
	}
}
