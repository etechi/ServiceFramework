﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.



using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using SF.Sys.Auth;
using SF.Sys.TimeServices;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks; 

namespace SF.Sys.AspNetCore.Auth
{
	public class CustomJwtDataFormat : ISecureDataFormat<AuthenticationTicket>
	{
		private readonly string algorithm;
		private readonly TokenValidationParameters validationParameters;

		public CustomJwtDataFormat(string algorithm, TokenValidationParameters validationParameters)
		{
			this.algorithm = algorithm;
			this.validationParameters = validationParameters;
		}

		public AuthenticationTicket Unprotect(string protectedText)
			=> Unprotect(protectedText, null);

		public AuthenticationTicket Unprotect(string protectedText, string purpose)
		{
			var handler = new JwtSecurityTokenHandler();
			ClaimsPrincipal principal = null;
			SecurityToken validToken = null;

			try
			{
				principal = handler.ValidateToken(protectedText, this.validationParameters, out validToken);

				var validJwt = validToken as JwtSecurityToken;

				if (validJwt == null)
				{
					throw new ArgumentException("Invalid JWT");
				}

				if (!validJwt.Header.Alg.Equals(algorithm, StringComparison.Ordinal))
				{
					throw new ArgumentException($"Algorithm must be '{algorithm}'");
				}

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

			// Validation passed. Return a valid AuthenticationTicket:
			return new AuthenticationTicket(principal, new AuthenticationProperties(), "Cookie");
		}

		// This ISecureDataFormat implementation is decode-only
		public string Protect(AuthenticationTicket data)
		{
			throw new NotImplementedException();
		}

		public string Protect(AuthenticationTicket data, string purpose)
		{
			throw new NotImplementedException();
		}
	}
}