using SF.Auth.Identities.Internals;
using SF.Clients;
using SF.Core.Caching;
using SF.Core.ServiceManagement;
using SF.Core.Times;
using SF.Services.Security;
using System;
using System.Collections.Generic;

namespace SF.Auth.Identities
{

	public class IdentityServiceSetting
	{
		public bool VerifyCodeVisible { get; set; }

		public Lazy<ILocalCache<VerifyCode>> VerifyCodeCache { get; set; }
		public Lazy<IIdentStorage> IdentStorage { get; set; }
		public Lazy<IClientService> ClientService { get; set; }
		public Lazy<IDataProtector> DataProtector { get; set; }
		public Lazy<IPasswordHasher> PasswordHasher { get; set; }

		public Lazy<ITimeService> TimeService { get; set; }
		public Lazy<ILocalCache<IdentityData>> IdentityDataCache { get; set; }
		public TypedInstanceResolver<IIdentityCredentialProvider> CredentialProviderResolver { get; set; }
		public Lazy<IIdentityCredentialProvider> DefaultIdentityCredentialProvider { get; set; }
		public IEnumerable<IIdentityCredentialProvider> IdentityCredentialProviders { get; set; }
	}

}
