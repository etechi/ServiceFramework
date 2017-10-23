#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

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
		public Lazy<IServiceInstanceDescriptor> ServiceInstanceDescriptor { get; set; }
		public Lazy<ITimeService> TimeService { get; set; }
		public Lazy<ILocalCache<IdentityData>> IdentityDataCache { get; set; }
		public TypedInstanceResolver<IIdentityCredentialProvider> CredentialProviderResolver { get; set; }
		public Lazy<IIdentityCredentialProvider> DefaultIdentityCredentialProvider { get; set; }
		public IEnumerable<IIdentityCredentialProvider> IdentityCredentialProviders { get; set; }
	}

}
