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

using IdentityServer4.Services;
using IdentityServer4.Stores;
using SF.Metadata;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using SF.Core.ServiceManagement;
using Microsoft.Extensions.DependencyInjection;
using SF.Auth.IdentityServices.IdentityServer4Impl;
using SF.Auth.IdentityServices.Managers;
using SF.Auth.IdentityServices.Models;
using SF.Auth.IdentityServices;
using SF.Core.ServiceManagement.Management;
using SF.Auth.IdentityServices.Internals;
using SF.Auth.IdentityServices.UserCredentialProviders;

namespace SF.Core.ServiceManagement
{
	public static class IdentityServiceDIExtension 
	{
		public static Core.ServiceManagement.IServiceCollection AddIdentityServer4Support(this Core.ServiceManagement.IServiceCollection sc)
		{
			sc.AsMicrosoftServiceCollection()
				.AddIdentityServer(o =>
				{
					o.UserInteraction.LoginUrl = "/user/signin";

				})
				.AddClientStore<ClientStore>()
				.AddProfileService<ProfileService>()
				.AddResourceStore<ResourceStore>();
			return sc;
		}
	}
}

