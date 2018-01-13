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

using Microsoft.Extensions.DependencyInjection;
using SF.Auth.IdentityServices;
using SF.Auth.IdentityServices.IdentityServer4Impl;
using Microsoft.AspNetCore.Builder;
namespace SF.Sys.Services
{
	public static class IdentityServiceDIExtension 
	{
		public static IServiceCollection AddIdentityServer4Support(this IServiceCollection sc)
		{
			var msc = sc.AsMicrosoftServiceCollection();
			msc.AddIdentityServer(o =>
				{
					o.UserInteraction.LoginUrl = "/user/signin";

				})
				.AddClientStore<ClientStore>()
				.AddProfileService<ProfileService>()
				.AddResourceStore<ResourceStore>()
				.AddResourceOwnerValidator<UserResourceOwnerPasswordValidator>();

			//msc.AddAuthentication("Bearer")
			//   .AddIdentityServerAuthentication(options =>
			//   {
			//	   options.Authority = "http://localhost:5000";
			//	   options.RequireHttpsMetadata = false;

			//	   options.ApiName = "Document";
				   
			//   });
			sc.AddTransient<ITokenService, DefaultTokenService>();
			sc.AddTransient<IAccessTokenHandler, AccessTokenHandler>();
			return sc;
		}
	}
}

