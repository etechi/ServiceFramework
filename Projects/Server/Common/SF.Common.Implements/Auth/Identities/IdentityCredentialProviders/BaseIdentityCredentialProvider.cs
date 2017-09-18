﻿using SF.Auth.Identities.Internals;
using SF.Auth.Identities.Models;
using SF.Core.ServiceManagement;
using System.Threading.Tasks;

namespace SF.Auth.Identities.IdentityCredentialProviders
{
	public abstract class BaseIdentityCredentialProvider :
		IIdentityCredentialProvider
	{
		protected IIdentityCredentialStorage CredentialStorage { get; }
		public IServiceInstanceDescriptor ServiceInstance { get; }
		public abstract string Name { get; }
		public long Id => ServiceInstance.InstanceId;

		public BaseIdentityCredentialProvider(
			IIdentityCredentialStorage CredentialStorage,
			IServiceInstanceDescriptor ServiceInstance
			)
		{
			this.CredentialStorage = CredentialStorage;
			this.ServiceInstance = ServiceInstance;
		}

		public virtual Task Bind(string Ident, string UnionIdent, bool Confirmed, long UserId)
			=> CredentialStorage.Bind(ServiceInstance.InstanceId, Ident, UnionIdent, Confirmed, UserId);


		public virtual Task<IdentityCredential> Find(string Ident, string UnionIdent)
			=> CredentialStorage.Find(ServiceInstance.InstanceId, Ident, UnionIdent);

		public virtual Task<IdentityCredential> FindOrBind(string Ident, string UnionIdent, bool Confirmed, long UserId)
			=> CredentialStorage.FindOrBind(ServiceInstance.InstanceId, Ident, UnionIdent, Confirmed, UserId);

		public virtual Task<IdentityCredential[]> GetIdents(long UserId)
			=> CredentialStorage.GetIdents(ServiceInstance.InstanceId, UserId);

		public virtual Task SetConfirmed(string Ident, bool Confirmed)
			=> CredentialStorage.SetConfirmed(ServiceInstance.InstanceId, Ident, Confirmed);

		public virtual Task Unbind(string Ident, long UserId)
			=> CredentialStorage.Unbind(ServiceInstance.InstanceId, Ident, UserId);

		public abstract Task<string> VerifyFormat(string Ident);
		public abstract bool IsConfirmable();
		public abstract Task<long> SendConfirmCode(long? IdentityId, string Ident, string Code, ConfirmMessageType Type, string TrackIdent);
	}
}