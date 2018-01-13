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

using SF.Sys.Annotations;
using SF.Sys.Entities.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SF.Auth.IdentityServices.Models
{
	/// <summary>
	/// 认证客户端配置
	/// </summary>
	[EntityObject]
	public class ClientConfigInternal : ObjectEntityBase<long>
	{
	}
	
	public class ClientScope
	{
		/// <summary>
		/// 范围
		/// </summary>
		[Key]
		[EntityIdent(typeof(ScopeInternal),nameof(ScopeName))]
		public string ScopeId { get; set; }

		/// <summary>
		/// 范围
		/// </summary>
		[TableVisible]
		[Ignore]
		public string ScopeName { get; set; }
	}
	public class ClientConfigEditable: ClientConfigInternal
	{
		///<title>发送用户声明</title>
		/// <summary>
		/// When requesting both an id token and access token, should the user claims always be added to the id token instead of requring the client to use the userinfo endpoint. Defaults to false.
		/// </summary>
		public bool AlwaysIncludeUserClaimsInIdToken { get; set; }

		///<title>身份令牌超时</title>
		/// <summary>
		///Lifetime of identity token in seconds (defaults to 300 seconds / 5 minutes)
		/// </summary>
		public int IdentityTokenLifetime { get; set; }

		///<title>访问令牌超时</title>
		/// <summary>
		/// Lifetime of access token in seconds (defaults to 3600 seconds / 1 hour)
		/// </summary>
		public int AccessTokenLifetime { get; set; }

		///<title>授权码超时</title>
		/// <summary>
		/// Lifetime of authorization code in seconds (defaults to 300 seconds / 5 minutes)
		/// </summary>
		public int AuthorizationCodeLifetime { get; set; }

		///<title>更新令牌绝对超时</title>
		/// <summary>
		/// Maximum lifetime of a refresh token in seconds. Defaults to 2592000 seconds /30 days
		/// </summary>
		public int AbsoluteRefreshTokenLifetime { get; set; }

		///<title>更新令牌间隔超时</title>
		/// <summary>
		/// Sliding lifetime of a refresh token in seconds. Defaults to 1296000 seconds / 15 days
		/// </summary>
		public int SlidingRefreshTokenLifetime { get; set; }

		///<title>授权超时</title>
		/// <summary>
		/// Lifetime of a user consent in seconds. Defaults to null (no expiration)
		/// </summary>
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

		///<title>客户端声明前缀</title>
		/// <summary>
		/// Gets or sets a value to prefix it on client claim types. Defaults to client_.
		/// </summary>
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
		///<title>允许请求范围</title>
		[TableRows]
		public IEnumerable<ClientScope> Scopes { get; set; }

		///<title>离线访问</title>
		/// <summary>
		/// Gets or sets a value indicating whether [allow offline access]. Defaults to false.
		/// </summary>
		public bool AllowOfflineAccess { get; set; }

		///<title>客户端密钥</title>
		/// <summary>
		/// Client secrets - only relevant for flows that require a secret
		/// </summary>
		[MaxLength(200)]
		public string ClientSecrets { get; set; }

		///<title>是否需要密钥</title>
		/// <summary>
		/// If set to false, no client secret is needed to request tokens at the token endpoint (defaults to true)
		/// </summary>
		public bool RequireClientSecret { get; set; }


		///<title>跨域设置</title>
		/// <summary>
		/// Gets or sets the allowed CORS origins for JavaScript clients. split by ;
		/// </summary>
		[MaxLength(200)]
		public string AllowedCorsOrigins { get; set; }

		///<title>是否需要授权</title>
		/// <summary>
		/// Specifies whether a consent screen is required (defaults to true)
		/// </summary>
		public bool RequireConsent { get; set; }

		///<title>授权类型</title>
		/// <summary>
		/// Specifies the allowed grant types (legal combinations of AuthorizationCode, Implicit,Hybrid, ResourceOwner, ClientCredentials). Defaults to Implicit. 
		/// </summary>
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
		///<title>需要注销会话</title>
		/// <summary>
		/// Specifies is the user's session id should be sent to the FrontChannelLogoutUri. (defaults to true)
		/// </summary>
		public bool FrontChannelLogoutSessionRequired { get; set; }

		///<title>是否记住授权</title>
		/// <summary>
		/// Specifies whether user can choose to store consent decisions (defaults to true)
		/// </summary>
		public bool AllowRememberConsent { get; set; }

		///<title>是否需要注销会话</title>
		/// <summary>
		///Specifies is the user's session id should be sent to the BackChannelLogoutUri Defaults to true.
		/// </summary>
		public bool BackChannelLogoutSessionRequired { get; set; }


	}

	/// <summary>
	/// 认证客户端
	/// </summary>
	[EntityObject]
	public class ClientInternal : UIObjectEntityBase<string>
	{
		/// <summary>
		/// 配置
		/// </summary>
		[EntityIdent(typeof(ClientConfigInternal),nameof(ClientConfigName))]
		public long ClientConfigId { get; set; }

		/// <summary>
		/// 配置
		/// </summary>
		[TableVisible]
		[Ignore]
		public string ClientConfigName { get; set; }

		/// <summary>
		/// 客户端密钥
		/// </summary>
		[MaxLength(100)]
		public string ClientSecrets { get; set; }

	}

	public class ClientEditable : ClientInternal
	{

		///<title>客户端Url</title>
		/// <summary>
		///URI to further information about client (used on consent screen)
		/// </summary>
		[MaxLength(200)]
		public string ClientUri { get; set; }

		///<title>注销跳转地址</title>
		/// <summary>
		///Specifies allowed URIs to redirect to after logout
		/// </summary>
		public string PostLogoutRedirectUris { get; set; }

		///<title>前端注销跳转地址</title>
		/// <summary>
		///Specifies logout URI at client for HTTP front-channel based logout.
		/// </summary>
		public string FrontChannelLogoutUri { get; set; }

		///<title>登录跳转地址</title>
		/// <summary>
		///Specifies allowed URIs to return tokens or authorization codes to
		/// </summary>
		public string RedirectUris { get; set; }

	}

}

