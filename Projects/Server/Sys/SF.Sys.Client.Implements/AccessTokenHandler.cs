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


using System;
using System.Security.Claims;
using System.Text;

namespace SF.Sys.Clients
{
	public class AccessTokenHandler : IAccessTokenHandler
	{
		IDefaultAccessTokenProperties DefaultAccessTokenProperties { get; }
		ITimeService TimeService { get; }
		public AccessTokenHandler(IDefaultAccessTokenProperties DefaultAccessTokenProperties, ITimeService TimeService)
		{
			this.DefaultAccessTokenProperties = DefaultAccessTokenProperties;
			this.TimeService = TimeService;
		}
		DateTime? GetExpires(DateTime? Expires)
		{
			if (!Expires.HasValue && DefaultAccessTokenProperties.Expires.HasValue)
				Expires = TimeService.Now.Add(DefaultAccessTokenProperties.Expires.Value);
			return Expires;

		}
		public string Create(ClaimsPrincipal User, DateTime? Expires)
		{
			var handler = new JwtSecurityTokenHandler();
			
			return handler.CreateEncodedJwt(new SecurityTokenDescriptor
			{
				Issuer = DefaultAccessTokenProperties.Issuer, // 指定 Token 签发者，也就是这个签发服务器的名称
				Audience = User.GetUserIdent().ToString(), // 指定 Token 接收者
				SigningCredentials = DefaultAccessTokenProperties.SigningCredentials,
				Subject = (ClaimsIdentity)User.Identity,
				Expires = GetExpires( Expires)
			});
		}

		public ClaimsPrincipal Validate(string AccessToken)
		{
			var handler = new JwtSecurityTokenHandler();
			var user=handler.ValidateToken(
				AccessToken,
				DefaultAccessTokenProperties.TokenValidationParameters,
				out var token
				);
			var validJwt = token as JwtSecurityToken;
			if (validJwt == null)
				throw new PublicDeniedException("JWT验签失败");

			if (!validJwt.Header.Alg.Equals(DefaultAccessTokenProperties.SigningCredentials.Algorithm , StringComparison.Ordinal))
				throw new PublicDeniedException("JWT验签失败,签名算法错误");

			if(user==null)
				throw new PublicDeniedException("JWT验签失败");

			return user;
		}
	}

	public class DefaultAccessTokenProperties : IDefaultAccessTokenProperties
	{
		public string Issuer { get; }
		public TimeSpan? Expires { get; }
		public SigningCredentials SigningCredentials { get; }

		public TokenValidationParameters TokenValidationParameters { get; }

		public DefaultAccessTokenProperties(
			string Issuer,
			string securityKey,
			TimeSpan? Expires,
			string securityAlgorithms = SecurityAlgorithms.HmacSha256
			)
		{
			this.Issuer = Issuer;
			this.Expires = Expires;
			var secKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
			secKey.KeyId = "uid";
			this.SigningCredentials = new SigningCredentials(
				secKey,
				securityAlgorithms ?? SecurityAlgorithms.HmacSha256
				);
			this.TokenValidationParameters = new TokenValidationParameters
			{
				// The signing key must match!
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = secKey,

				// Validate the JWT Issuer (iss) claim
				ValidateIssuer = true,
				ValidIssuer = Issuer,

				// Validate the token expiry
				ValidateLifetime = true,

				// If you want to allow a certain amount of clock drift, set that here:
				ClockSkew = TimeSpan.Zero
			};
		}
	}
}
