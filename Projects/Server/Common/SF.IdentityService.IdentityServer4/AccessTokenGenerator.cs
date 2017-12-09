﻿#region Apache License Version 2.0
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

using IdentityServer4.Stores;
using System.Threading.Tasks;
using IdentityServer4.Models;
using System.Linq;
using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys;
using SF.Sys.Auth;
using IdentityServer4.Services;
using IdentityServer4.Configuration;
using IdentityServer4.Validation;
using System;
using System.Security.Claims;

namespace SF.Auth.IdentityServices.IdentityServer4Impl
{
	public class AccessTokenGenerator : IAccessTokenGenerator
	{
		ITokenService TokenService { get; }
		IdentityServerOptions Options { get; }
		IClientStore ClientStore { get; }
		IResourceStore ResourceStore { get; }

		public AccessTokenGenerator(
			ITokenService TokenService, 
			IClientStore ClientStore , 
			IResourceStore ResourceStore ,
			IdentityServerOptions Options
			)
		{
			this.TokenService = TokenService;
			this.Options = Options;
			this.ClientStore = ClientStore;
			this.ResourceStore = ResourceStore;

		}
		public async Task<string> Generate(long UserId, string ClientId, string[] Scopes,DateTime? Expires)
		{
			if (ClientId.IsNullOrEmpty())
				throw new ArgumentException(nameof(ClientId));

			var client = await ClientStore.FindClientByIdAsync(ClientId);
			if (client == null)
				throw new ArgumentException($"找不到客户端:{ClientId}");


			var Request = new TokenCreationRequest();
			Request.Subject = new ClaimsPrincipal(
			   new ClaimsIdentity(
				   new[] { new Claim("sub", UserId.ToString()) },
				   "SFAuth"
				   )
			   );

			Request.IncludeAllIdentityClaims = true;
			Request.ValidatedRequest = new ValidatedRequest
			{
				Subject = Request.Subject,
				Options = Options
			};
			Request.ValidatedRequest.SetClient(client);
			
			Request.Resources = await ResourceStore.FindEnabledResourcesByScopeAsync(Scopes);

			var Token = await TokenService.CreateAccessTokenAsync(Request);
			Token.Issuer = Options.IssuerUri;
			var TokenValue = await TokenService.CreateSecurityTokenAsync(Token);
			return TokenValue;
		}
	}
}

