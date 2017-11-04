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

using SF.Data.Models;
using SF.Entities;
using SF.Entities.AutoEntityProvider;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace SF.Auth.IdentityServices.Models
{
	[Comment("认证客户端配置")]
	public class ClientConfigInternal : ObjectEntityBase<long>
	{
	}
	
	public class ClientScope
	{
		[Key]
		[EntityIdent(typeof(ScopeInternal),nameof(ScopeName))]
		[Comment("范围")]
		public string ScopeId { get; set; }

		[Comment("范围")]
		[TableVisible]
		[Hidden]
		public string ScopeName { get; set; }
	}
	public class ClientConfigEditable: ClientConfigInternal
	{ 
		[Comment("发送用户声明", "When requesting both an id token and access token, should the user claims always be added to the id token instead of requring the client to use the userinfo endpoint. Defaults to false.")]
		public bool AlwaysIncludeUserClaimsInIdToken { get; set; }

		[Comment("身份令牌超时", "Lifetime of identity token in seconds (defaults to 300 seconds / 5 minutes)")]
		public int IdentityTokenLifetime { get; set; }

		[Comment("访问令牌超时", "Lifetime of access token in seconds (defaults to 3600 seconds / 1 hour)")]
		public int AccessTokenLifetime { get; set; }

		[Comment("授权码超时", "Lifetime of authorization code in seconds (defaults to 300 seconds / 5 minutes)")]
		public int AuthorizationCodeLifetime { get; set; }

		[Comment("更新令牌绝对超时", "Maximum lifetime of a refresh token in seconds. Defaults to 2592000 seconds /30 days")]
		public int AbsoluteRefreshTokenLifetime { get; set; }

		[Comment("更新令牌间隔超时", "Sliding lifetime of a refresh token in seconds. Defaults to 1296000 seconds / 15 days")]
		public int SlidingRefreshTokenLifetime { get; set; }

		[Comment("授权超时", "Lifetime of a user consent in seconds. Defaults to null (no expiration)")]
		public int? ConsentLifetime { get; set; }
		////
		//// 摘要:
		////     ReUse: the refresh token handle will stay the same when refreshing tokens OneTime:
		////     the refresh token handle will be updated when refreshing tokens
		//public TokenUsage RefreshTokenUsage { get; set; }
		////
		//// 摘要:
		////     Gets or sets a value indicating whether the access token (and its claims) should
		////     be updated on a refresh token request. Defaults to false.
		//public bool UpdateAccessTokenClaimsOnRefresh { get; set; }
		////
		//// 摘要:
		////     Absolute: the refresh token will expire on a fixed point in time (specified by
		////     the AbsoluteRefreshTokenLifetime) Sliding: when refreshing the token, the lifetime
		////     of the refresh token will be renewed (by the amount specified in SlidingRefreshTokenLifetime).
		////     The lifetime will not exceed AbsoluteRefreshTokenLifetime.
		//public TokenExpiration RefreshTokenExpiration { get; set; }

		[Comment("客户端声明前缀", " Gets or sets a value to prefix it on client claim types. Defaults to client_.")]
		[MaxLength(100)]
		public string ClientClaimsPrefix { get; set; }
		////
		//// 摘要:
		////     Gets or sets a salt value used in pair-wise subjectId generation for users of
		////     this client.
		//public string PairWiseSubjectSalt { get; set; }

		//
		// 摘要:
		//     Specifies the api scopes that the client is allowed to request. If empty, the
		//     client can't access any scope
		[Comment("允许请求范围")]
		public IEnumerable<ClientScope> Scopes { get; set; }

		[Comment("离线访问", " Gets or sets a value indicating whether [allow offline access]. Defaults to false.")]
		public bool AllowOfflineAccess { get; set; }

		[Comment("客户端密钥", " Client secrets - only relevant for flows that require a secret")]
		[MaxLength(200)]
		public string ClientSecrets { get; set; }

		[Comment("是否需要密钥", "If set to false, no client secret is needed to request tokens at the token endpoint (defaults to true)")]
		public bool RequireClientSecret { get; set; }


		[Comment("跨域设置", "Gets or sets the allowed CORS origins for JavaScript clients. split by ;")]
		[MaxLength(200)]
		public string AllowedCorsOrigins { get; set; }

		[Comment("是否需要授权", "Specifies whether a consent screen is required (defaults to true)")]
		public bool RequireConsent { get; set; }

		[Comment("授权类型", "Specifies the allowed grant types (legal combinations of AuthorizationCode, Implicit,Hybrid, ResourceOwner, ClientCredentials). Defaults to Implicit. ")]
		public string[] AllowedGrantTypes { get; set; }
		////
		//// 摘要:
		////     Specifies whether a proof key is required for authorization code based token
		////     requests (defaults to false).
		//public bool RequirePkce { get; set; }
		////
		//// 摘要:
		////     Specifies whether a proof key can be sent using plain method (not recommended
		////     and defaults to false.)
		//public bool AllowPlainTextPkce { get; set; }
		////
		//// 摘要:
		////     Controls whether access tokens are transmitted via the browser for this client
		////     (defaults to false). This can prevent accidental leakage of access tokens when
		////     multiple response types are allowed.
		//public bool AllowAccessTokensViaBrowser { get; set; }
		//
		[Comment("需要注销会话", "Specifies is the user's session id should be sent to the FrontChannelLogoutUri. (defaults to true)")]
		public bool FrontChannelLogoutSessionRequired { get; set; }

		[Comment("是否记住授权", "Specifies whether user can choose to store consent decisions (defaults to true)")]
		public bool AllowRememberConsent { get; set; }

		[Comment("是否需要注销会话", "Specifies is the user's session id should be sent to the BackChannelLogoutUri Defaults to true.")]
		public bool BackChannelLogoutSessionRequired { get; set; }


	}

	[Comment("认证客户端")]
	public class ClientInternal : UIObjectEntityBase<string>
	{
		[EntityIdent(typeof(ClientConfigInternal),nameof(ClientConfigName))]
		public long ClientConfigId { get; set; }

		[Comment("配置")]
		[TableVisible]
		[Hidden]
		public string ClientConfigName { get; set; }

		[Comment("客户端密钥")]
		public string Secret { get; set; }

	}

	public class ClientEditable : ClientInternal
	{
		[Comment("客户端Url", "URI to further information about client (used on consent screen)")]
		[MaxLength(200)]
		public string ClientUri { get; set; }

		[Comment("注销跳转地址", "Specifies allowed URIs to redirect to after logout")]
		public string PostLogoutRedirectUris { get; set; }

		[Comment("前端注销跳转地址", "Specifies logout URI at client for HTTP front-channel based logout.")]
		public string FrontChannelLogoutUri { get; set; }



		[Comment("登录跳转地址", "Specifies allowed URIs to return tokens or authorization codes to")]
		public string RedirectUris { get; set; }

	}

}

