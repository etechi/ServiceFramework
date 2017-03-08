using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Auth.Users.IdentProviders
{
	public abstract class BaseIdentProvider :
		IUserIdentProvider
	{
		protected IUserIdentStorage IdentStorage { get; }
		public BaseIdentProvider(IUserIdentStorage IdentStorage)
		{
			this.IdentStorage = IdentStorage;
		}

		public virtual Task Bind(string Ident, string UnionIdent, bool Confirmed, long UserId)
			=> IdentStorage.Bind(Name, Ident, UnionIdent, Confirmed, UserId);

		public virtual Task<bool> CanSendMessage()
			=> Task.FromResult(true);

		public virtual Task<UserIdent> Find(string Ident, string UnionIdent)
			=> IdentStorage.Find(Name, Ident, UnionIdent);

		public virtual Task<UserIdent> FindOrBind(string Ident, string UnionIdent, bool Confirmed, long UserId)
			=> IdentStorage.FindOrBind(Name, Ident, UnionIdent, Confirmed, UserId);

		public virtual Task<UserIdent[]> GetIdents(long UserId)
			=> IdentStorage.GetIdents(Name, UserId);

		public virtual Task SetConfirmed(string Ident, bool Confirmed)
			=> IdentStorage.SetConfirmed(Name, Ident, Confirmed);

		public virtual Task Unbind(string Ident, long UserId)
			=> IdentStorage.Unbind(Name, Ident, UserId);

		public abstract string Name { get; }
		public abstract Task<string> Verify(string Ident);
		public abstract Task<string> SendMessage(string Ident, string Title, string Message, string TrackIdent);
	}
}
