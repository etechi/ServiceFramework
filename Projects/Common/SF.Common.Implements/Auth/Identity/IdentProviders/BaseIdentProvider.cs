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
		public abstract string Name { get; }

		public BaseIdentProvider(IIdentBindStorage IdentStorage)
		{
			this.IdentStorage = IdentStorage;
		}

		public virtual Task Bind(int ScopeId,string Ident, string UnionIdent, bool Confirmed, long UserId)
			=> IdentStorage.Bind(ScopeId, Name, Ident, UnionIdent, Confirmed, UserId);


		public virtual Task<IdentBind> Find(int ScopeId, string Ident, string UnionIdent)
			=> IdentStorage.Find(ScopeId,Name, Ident, UnionIdent);

		public virtual Task<IdentBind> FindOrBind(int ScopeId, string Ident, string UnionIdent, bool Confirmed, long UserId)
			=> IdentStorage.FindOrBind(ScopeId,Name, Ident, UnionIdent, Confirmed, UserId);

		public virtual Task<IdentBind[]> GetIdents(long UserId)
			=> IdentStorage.GetIdents(Name, UserId);

		public virtual Task SetConfirmed(int ScopeId, string Ident, bool Confirmed)
			=> IdentStorage.SetConfirmed(ScopeId,Name, Ident, Confirmed);

		public virtual Task Unbind(int ScopeId, string Ident, long UserId)
			=> IdentStorage.Unbind(ScopeId,Name, Ident, UserId);

		public abstract Task<string> VerifyFormat(string Ident);
		public abstract bool IsConfirmable();
		public abstract Task<long> SendConfirmCode(int ScopeId, string Ident, string Code, ConfirmMessageType Type, string TrackIdent);
	}
}
