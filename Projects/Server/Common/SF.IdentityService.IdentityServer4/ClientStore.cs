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

using IdentityServer4.Stores;
using System.Threading.Tasks;
using IdentityServer4.Models;
using System.Linq;
using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys;

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
			if (clientId==null)
				return null;

			var re = await (
				from c in Clients.AsQueryable()
				where c.Id == clientId && c.LogicState == EntityLogicState.Enabled
				select new
				{
					cfg = c.ClientConfig,
					cli=c,
					scopes=c.ClientConfig.Scopes.Select(s=>s.ScopeId)
				}).SingleOrDefaultAsync();
			if (re == null) return null;
			var cfg = re.cfg;
			var cli = re.cli;
			return new Client
			{
				BackChannelLogoutSessionRequired = cfg.BackChannelLogoutSessionRequired,
				AlwaysIncludeUserClaimsInIdToken = cfg.AlwaysIncludeUserClaimsInIdToken,
				IdentityTokenLifetime = cfg.IdentityTokenLifetime,
				AccessTokenLifetime = cfg.AccessTokenLifetime,
				AuthorizationCodeLifetime = cfg.AuthorizationCodeLifetime,
				AbsoluteRefreshTokenLifetime = cfg.AbsoluteRefreshTokenLifetime,
				SlidingRefreshTokenLifetime = cfg.SlidingRefreshTokenLifetime,
				ConsentLifetime = cfg.ConsentLifetime,
				//RefreshTokenUsage = re.RefreshTokenUsage,
				//UpdateAccessTokenClaimsOnRefresh = re.UpdateAccessTokenClaimsOnRefresh,
				//RefreshTokenExpiration = re.RefreshTokenExpiration,
				AccessTokenType = AccessTokenType.Jwt,
				EnableLocalLogin = true,
				//IdentityProviderRestrictions = re.IdentityProviderRestrictions,
				IncludeJwtId = true,//re.IncludeJwtId,
									//Claims = re.Claims,
									//AlwaysSendClientClaims = re.AlwaysSendClientClaims,
				ClientClaimsPrefix = cfg.ClientClaimsPrefix,
				//PairWiseSubjectSalt = re.PairWiseSubjectSalt,
				AllowedScopes = re.scopes.ToArray(),
				AllowOfflineAccess = cfg.AllowOfflineAccess,
				//Properties = re.Properties,
				//BackChannelLogoutUri = re.BackChannelLogoutUri,
				Enabled = cli.LogicState == EntityLogicState.Enabled,
				ClientId = cli.Id.ToString(),
				//ProtocolType = re.ProtocolType,
				ClientSecrets = new[] { new Secret(cli.ClientSecrets) },
				RequireClientSecret = cfg.RequireClientSecret,
				ClientName = cli.Name,
				ClientUri = cli.ClientUri,
				LogoUri = cli.Icon,
				AllowedCorsOrigins = Json.Parse<string[]>(cfg.AllowedCorsOrigins),
				RequireConsent = cfg.RequireConsent,
				AllowedGrantTypes = Json.Parse<string[]>(cfg.AllowedGrantTypes),
				//RequirePkce = re.RequirePkce,
				//AllowPlainTextPkce = re.AllowPlainTextPkce,
				//AllowAccessTokensViaBrowser = re.AllowAccessTokensViaBrowser,
				RedirectUris = new[] { cli.RedirectUris },
				PostLogoutRedirectUris = new[] { cli.PostLogoutRedirectUris },
				FrontChannelLogoutUri = cli.FrontChannelLogoutUri,
				FrontChannelLogoutSessionRequired = cfg.FrontChannelLogoutSessionRequired,
				AllowRememberConsent = cfg.AllowRememberConsent,
			};
		}
	}
}

