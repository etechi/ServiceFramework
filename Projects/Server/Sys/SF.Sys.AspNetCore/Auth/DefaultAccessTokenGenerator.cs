// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.



using Microsoft.IdentityModel.Tokens;
using SF.Sys.Auth;
using SF.Sys.Linq;
using SF.Sys.TimeServices;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SF.Sys.AspNetCore.Auth
{
	public class TokenProviderOptions
	{

		public string Issuer { get; set; }

		public string Audience { get; set; }

		public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(5);

		public SigningCredentials SigningCredentials { get; set; }
	}
	
	public class DefaultAccessTokenGenerator : IAccessTokenGenerator
	{
		ITimeService TimeService { get; }
		IUserProfileService UserProfileService { get; }
		TokenProviderOptions Options { get; }
		
		public DefaultAccessTokenGenerator(
			ITimeService TimeService,
			IUserProfileService UserProfileService,
			TokenProviderOptions Options
			)
		{
			this.Options = Options;
			this.TimeService = TimeService;
			this.UserProfileService = UserProfileService;
		}

		public async Task<AccessTokenGenerateResult> Generate(
			long UserId, 
			string ClientId, 
			string[] Scopes, 
			DateTime? Expires
			)
		{
			var now = TimeService.UtcNow;
			var user = await UserProfileService.GetUser(UserId);

			//var cs = await UserProfileService.GetClaims(UserId, ClientId, Scopes);

			// Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
			// You can add other claims here, if you want:
			var claims = new List<Claim>()
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
				new Claim(JwtRegisteredClaimNames.Jti, Strings.NumberAndLowerUpperChars.Random(16)),
				new Claim(JwtRegisteredClaimNames.Iat, now.ToUnixTime().ToString(), ClaimValueTypes.Integer64),
				new Claim(ClaimsIdentity.DefaultNameClaimType,user.Name)
			};
			//claims.AddRange(cs);
			if (user.Roles?.Any() ?? false)
				claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Roles.Join(",")));

			// Create the JWT and write it to a string
			var expires = Expires.HasValue?Expires.Value.ToUtcTime():now.AddDays(365);
			var jwt = new JwtSecurityToken(
				issuer: Options.Issuer,
				audience: Options.Audience,
				claims: claims,
				notBefore: now,
				expires: expires,
				signingCredentials: Options.SigningCredentials
				);
			var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

			return new AccessTokenGenerateResult
			{
				Token = encodedJwt,
				Expires = expires
			};

		}
	}
}