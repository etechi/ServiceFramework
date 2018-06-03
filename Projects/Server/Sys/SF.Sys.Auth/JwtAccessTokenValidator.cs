// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.



using Microsoft.IdentityModel.Tokens;
using SF.Sys.Auth;
using SF.Sys.TimeServices;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SF.Sys.Auth
{
	public class AccessTokenValidatorArguments
	{
		public string Algorithm { get; set; }
		public TokenValidationParameters ValidationParameters { get; set; }
	}

	/// <summary>
	/// Default token service
	/// </summary>
	public class JwtAccessTokenValidator : IAccessTokenValidator
	{
		AccessTokenValidatorArguments Parameters { get; }
		ITimeService TimeService { get; }

		public JwtAccessTokenValidator(
			ITimeService TimeService,
			AccessTokenValidatorArguments Parameters
			)
		{
			this.Parameters = Parameters;
			this.TimeService = TimeService;
		}
	
		public Task<ClaimsPrincipal> Validate(string Token)
		{
			var handler = new JwtSecurityTokenHandler();
			try
			{
				var principal = handler.ValidateToken(Token, Parameters.ValidationParameters, out var validToken);

				var validJwt = validToken as JwtSecurityToken;

				if (validJwt == null)
				{
					return null;
				}

				if (!validJwt.Header.Alg.Equals(Parameters.Algorithm, StringComparison.Ordinal))
				{
					return null;
				}

				return Task.FromResult(principal);

				// Additional custom validation of JWT claims here (if any)
			}
			catch (SecurityTokenValidationException)
			{
				return null;
			}
			catch (ArgumentException)
			{
				return null;
			}

		}
	}
}