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
using System;
using SF.Data;
using System.Linq;

namespace SF.Auth.IdentityServices.IdentityServer4Impl
{
	public class ClientStore : IClientStore
	{
		IDataSet<DataModels.Client> Clients { get; }
		
		public ClientStore(IDataSet<DataModels.Client> Clients)
		{
			this.Clients = Clients;
		}

		public async Task<Client> FindClientByIdAsync(string clientId)
		{
			var cid = clientId.TryToInt64();
			if (!cid.HasValue)
				return null;

			var re = await Clients.AsQueryable().Where(c => c.Id == cid.Value).SingleOrDefaultAsync();
			if (re == null) return null;

			return new Client
			{
				BackChannelLogoutSessionRequired = re.BackChannelLogoutSessionRequired,
				AlwaysIncludeUserClaimsInIdToken = re.AlwaysIncludeUserClaimsInIdToken,
				IdentityTokenLifetime = re.IdentityTokenLifetime,
				AccessTokenLifetime = re.AccessTokenLifetime,
				AuthorizationCodeLifetime = re.AuthorizationCodeLifetime,
				AbsoluteRefreshTokenLifetime = re.AbsoluteRefreshTokenLifetime,
				SlidingRefreshTokenLifetime = re.SlidingRefreshTokenLifetime,
				ConsentLifetime = re.ConsentLifetime,
				//RefreshTokenUsage = re.RefreshTokenUsage,
				//UpdateAccessTokenClaimsOnRefresh = re.UpdateAccessTokenClaimsOnRefresh,
				//RefreshTokenExpiration = re.RefreshTokenExpiration,
				AccessTokenType = AccessTokenType.Jwt,
				EnableLocalLogin = true,
				//IdentityProviderRestrictions = re.IdentityProviderRestrictions,
				IncludeJwtId = true,//re.IncludeJwtId,
									//Claims = re.Claims,
									//AlwaysSendClientClaims = re.AlwaysSendClientClaims,
				ClientClaimsPrefix = re.ClientClaimsPrefix,
				//PairWiseSubjectSalt = re.PairWiseSubjectSalt,
				AllowedScopes = re.AllowedScopes.SplitAndNormalizae(';').ToArray(),
				AllowOfflineAccess = re.AllowOfflineAccess,
				//Properties = re.Properties,
				//BackChannelLogoutUri = re.BackChannelLogoutUri,
				Enabled = re.LogicState == Entities.EntityLogicState.Enabled,
				ClientId = re.Id.ToString(),
				//ProtocolType = re.ProtocolType,
				ClientSecrets = new[] { new Secret(re.ClientSecrets) },
				RequireClientSecret = re.RequireClientSecret,
				ClientName = re.Name,
				ClientUri = re.ClientUri,
				LogoUri = re.Icon,
				AllowedCorsOrigins = re.AllowedCorsOrigins.SplitAndNormalizae(';').ToArray(),
				RequireConsent = re.RequireConsent,
				AllowedGrantTypes = re.AllowedGrantTypes.SplitAndNormalizae(';').ToArray(),
				//RequirePkce = re.RequirePkce,
				//AllowPlainTextPkce = re.AllowPlainTextPkce,
				//AllowAccessTokensViaBrowser = re.AllowAccessTokensViaBrowser,
				RedirectUris = new[] { re.RedirectUris },
				PostLogoutRedirectUris = new[] { re.PostLogoutRedirectUris },
				FrontChannelLogoutUri = re.FrontChannelLogoutUri,
				FrontChannelLogoutSessionRequired = re.FrontChannelLogoutSessionRequired,
				AllowRememberConsent = re.AllowRememberConsent,
			};
		}
	}
}

