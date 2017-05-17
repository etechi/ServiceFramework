﻿using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Auth.Identities.Internals;
using SF.Auth.Identities.Models;
using SF.Core.ServiceManagement;

namespace SF.Auth.Identities.IdentityCredentialProviders
{
	public abstract class BaseIdentityCredentialProvider :
		IIdentityCredentialProvider
	{
		protected IIdentityCredentialStorage CredentialStorage { get; }
		public IServiceInstanceMeta ServiceInstance { get; }
		public abstract string Name { get; }

		public BaseIdentityCredentialProvider(
			IIdentityCredentialStorage CredentialStorage,
			IServiceInstanceMeta ServiceInstance
			)
		{
			this.CredentialStorage = CredentialStorage;
			this.ServiceInstance = ServiceInstance;
		}

		public virtual Task Bind(string Ident, string UnionIdent, bool Confirmed, long UserId)
			=> CredentialStorage.Bind(ServiceInstance.Id, Ident, UnionIdent, Confirmed, UserId);


		public virtual Task<IdentityCredential> Find(string Ident, string UnionIdent)
			=> CredentialStorage.Find(ServiceInstance.Id, Ident, UnionIdent);

		public virtual Task<IdentityCredential> FindOrBind(string Ident, string UnionIdent, bool Confirmed, long UserId)
			=> CredentialStorage.FindOrBind(ServiceInstance.Id, Ident, UnionIdent, Confirmed, UserId);

		public virtual Task<IdentityCredential[]> GetIdents(long UserId)
			=> CredentialStorage.GetIdents(ServiceInstance.Id, UserId);

		public virtual Task SetConfirmed(string Ident, bool Confirmed)
			=> CredentialStorage.SetConfirmed(ServiceInstance.Id, Ident, Confirmed);

		public virtual Task Unbind(string Ident, long UserId)
			=> CredentialStorage.Unbind(ServiceInstance.Id, Ident, UserId);

		public abstract Task<string> VerifyFormat(string Ident);
		public abstract bool IsConfirmable();
		public abstract Task<long> SendConfirmCode(string Ident, string Code, ConfirmMessageType Type, string TrackIdent);
	}
}
