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



using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SF.Sys.AspNetCore.Auth;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.Services
{
	public class JwtAuthSettings
	{
		public string SecurityKey { get; set; }
		public string Issuer { get; set; }
		public string Audience { get; set; }
	}
	public static class AspNetCoreServiceCollectionExtension 
	{
		public static IServiceCollection AddAspNetCoreSupport(this IServiceCollection sc)
		{
			return sc.AddAspNetCoreHostingService();
		}
		public static IServiceCollection AddJwtAuthSupports(
			this IServiceCollection services,
			JwtAuthSettings Settings
			)
		{
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Settings.SecurityKey));
			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = key,
				ValidateIssuer = false,
				ValidIssuer = Settings.Issuer,
				ValidateAudience = false,
				ValidAudience = Settings.Audience,
				ValidateLifetime = false,
				ClockSkew = TimeSpan.Zero
			};
			JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
			services.AsMicrosoftServiceCollection().AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				
			})
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = tokenValidationParameters;

					options.RequireHttpsMetadata = false;
					//// base-address of your identityserver
					//options.Authority = "http://localost:5000/";

					//// name of the API resource
					//options.Audience = "Document";
					options.Events = new JwtBearerEvents
					{
						OnMessageReceived = (e) =>
						{

							return Task.CompletedTask;
						},
						OnTokenValidated = (e) =>
						{
							return Task.CompletedTask;
						},
						OnAuthenticationFailed = (e) =>
						{
							return Task.CompletedTask;
						},
						OnChallenge = (e) =>
						{

							return Task.CompletedTask;
						}
					};
					
				});

			//services.AddAspNetCoreCommonAuthorization();

			services.AddAspNetCoreCommonAuthorization(
				new TokenProviderOptions
				{
					Issuer = Settings.Issuer,
					Audience = Settings.Audience,
					SigningCredentials = new SigningCredentials(
						key,
						SecurityAlgorithms.HmacSha256
						)
				},
				new AccessTokenValidatorArguments
				{
					Algorithm = SecurityAlgorithms.HmacSha256,
					ValidationParameters = tokenValidationParameters
				}
			);

			return services;
		}
	}
}
