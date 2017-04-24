using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Auth.Identities.Internals;
using SF.Auth.Identities.Models;

namespace SF.Auth.Identities.IdentityCredentialProviders
{
	public abstract class BaseIdentityCredentialProvider :
		IIdentityCredentialProvider
	{
		protected IIdentityCredentialStorage CredentialStorage { get; }
		public SF.Core.ManagedServices.IServiceInstanceMeta ServiceInstance { get; }
		public abstract string Name { get; }

		public BaseIdentityCredentialProvider(
			IIdentityCredentialStorage CredentialStorage,
			SF.Core.ManagedServices.IServiceInstanceMeta ServiceInstance
			)
		{
			this.CredentialStorage = CredentialStorage;
			this.ServiceInstance = ServiceInstance;
		}

		public virtual Task Bind(string Ident, string UnionIdent, bool Confirmed, long UserId)
			=> CredentialStorage.Bind(ServiceInstance.Ident, Ident, UnionIdent, Confirmed, UserId);


		public virtual Task<IdentityCredential> Find(string Ident, string UnionIdent)
			=> CredentialStorage.Find(ServiceInstance.Ident, Ident, UnionIdent);

		public virtual Task<IdentityCredential> FindOrBind(string Ident, string UnionIdent, bool Confirmed, long UserId)
			=> CredentialStorage.FindOrBind(ServiceInstance.Ident, Ident, UnionIdent, Confirmed, UserId);

		public virtual Task<IdentityCredential[]> GetIdents(long UserId)
			=> CredentialStorage.GetIdents(ServiceInstance.Ident, UserId);

		public virtual Task SetConfirmed(string Ident, bool Confirmed)
			=> CredentialStorage.SetConfirmed(ServiceInstance.Ident, Ident, Confirmed);

		public virtual Task Unbind(string Ident, long UserId)
			=> CredentialStorage.Unbind(ServiceInstance.Ident, Ident, UserId);

		public abstract Task<string> VerifyFormat(string Ident);
		public abstract bool IsConfirmable();
		public abstract Task<long> SendConfirmCode(string Ident, string Code, ConfirmMessageType Type, string TrackIdent);
	}
}
