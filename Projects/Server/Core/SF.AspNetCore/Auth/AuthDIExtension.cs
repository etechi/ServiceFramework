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

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SF.Core.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.AspNetCore.Auth
{
	public static class AuthDIExtension
	{
		public static IServiceCollection AddAspNetCoreCommonAuthorization(this IServiceCollection services)
		{
			services.AddAuthentication(
				CookieAuthenticationDefaults.AuthenticationScheme
				)
				.AddCookie(opt =>
				{

					opt.Cookie.HttpOnly = true;
					opt.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
					opt.Cookie.Path = "/";
					opt.Cookie.Name = "sf-sess";
					opt.LoginPath = "/account/signin";
				}
			);

			var securityKey = "12faod919&^*%1212";

			var tokenValidationParameters = new TokenValidationParameters
			{
				// The signing key must match!
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey)),

				// Validate the JWT Issuer (iss) claim
				ValidateIssuer = true,
				ValidIssuer = "SFServer",

				// Validate the JWT Audience (aud) claim
				ValidateAudience = true,
				ValidAudience = "SFServer",

				// Validate the token expiry
				ValidateLifetime = true,

				// If you want to allow a certain amount of clock drift, set that here:
				ClockSkew = TimeSpan.Zero

				
			};
			services.AddAuthentication(
				JwtBearerDefaults.AuthenticationScheme
				)
				.AddJwtBearer(opt =>
				{
					opt.TokenValidationParameters = tokenValidationParameters;
				});
			
			//services.AddAuthorization(options =>
			//{
			//	options.DefaultPolicy =
			//		new AuthorizationPolicyBuilder("default")
			//		.RequireAuthenticatedUser()
			//		.Build();

			//	//options.AddPolicy("admin-bizness", policy =>
			//	//{
			//	//	policy.RequireAuthenticatedUser();

			//	//});
			//	//options.AddPolicy("admin-bizness", policy =>
			//	//{

			//	//	policy.RequireAuthenticatedUser();
			//	//});

			//});


		

			//services.AddAuthentication(opt=>
			//{
			//	opt.DefaultScheme = "default";

			//})
			//.AddJwtBearer(opt =>
			//{
			//	opt.TokenValidationParameters = tokenValidationParameters;

			//})
			//.AddCookie(opt =>
			//{
			//	opt.Cookie.HttpOnly = true;
			//	opt.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
			//	opt.Cookie.Path = "/";
			//	opt.Cookie.Name = "sf-sess";
			//});


			return services;
		}
	}
}
