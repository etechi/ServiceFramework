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

using SF.Auth.IdentityServices.Internals;
using SF.Services.Security;
using SF.Sys.Caching;
using SF.Sys.Clients;
using SF.Sys.Services;
using SF.Sys.TimeServices;
using System;
using System.Collections.Generic;

namespace SF.Auth.IdentityServices
{

	public class UserServiceSetting
	{
		public bool VerifyCodeVisible { get; set; }

		public Lazy<ILocalCache<VerifyCode>> VerifyCodeCache { get; set; }
		public Lazy<IUserStorage> IdentStorage { get; set; }
		public Lazy<IUserCredentialStorage> CredentialStorage { get; set; }
		public Lazy<IClientService> ClientService { get; set; }
		//public Lazy<IAccessToken> AccessToken{ get; set; }
		//public Lazy<IAccessTokenHandler> AccessTokenHandler { get; set; }
		public Lazy<IPasswordHasher> PasswordHasher { get; set; }
		public Lazy<IServiceInstanceDescriptor> ServiceInstanceDescriptor { get; set; }
		public Lazy<ITimeService> TimeService { get; set; }
		public Lazy<ILocalCache<UserData>> IdentityDataCache { get; set; }

		public NamedServiceResolver<IUserCredentialProvider> CredentialProviderResolver { get; set; }
		public string DefaultIdentityCredentialProvider { get; set; }
		public IEnumerable<IUserCredentialProvider> IdentityCredentialProviders { get; set; }
	}

}
