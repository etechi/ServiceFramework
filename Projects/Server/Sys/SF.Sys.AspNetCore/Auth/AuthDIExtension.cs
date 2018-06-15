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

using SF.Sys.AspNetCore.Auth;
using SF.Sys.Auth;
using SF.Sys.Services;
using SF.Sys.TimeServices;
using System;

namespace SF.Sys.Services
{
	public static class AuthDIExtension
	{
		public static IServiceCollection AddAspNetCoreCommonAuthorization(
			this IServiceCollection services,
			Func<IServiceProvider,TokenProviderOptions> TokenProviderOptions,
			Func<IServiceProvider, AccessTokenValidatorArguments> TokenValidatorArguments
			)
		{
			services.AddSingleton<IAccessTokenGenerator>(
				sp =>
					new JwtAccessTokenGenerator(
						sp.Resolve<ITimeService>(),
						sp.Resolve<IUserProfileService>(),
						TokenProviderOptions(sp)
						)
				);
			services.AddSingleton<IAccessTokenValidator>(
				sp => new JwtAccessTokenValidator(
					sp.Resolve<ITimeService>(),
					TokenValidatorArguments(sp)
					));
			//services.AddAuthentication(
			//	CookieAuthenticationDefaults.AuthenticationScheme
			//	)
			//	//.AddOpenIdConnect("oidc", "OpenID Connect", options =>
			//	//{
			//	//	options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
			//	//	options.SignOutScheme = IdentityServerConstants.SignoutScheme;
			//	//	options.RequireHttpsMetadata = false;
			//	//	options.Authority = "http://localhost:52706";
			//	//	options.ClientId = "local.internal";

			//	//	options.TokenValidationParameters = new TokenValidationParameters
			//	//	{
			//	//		NameClaimType = "name",
			//	//		RoleClaimType = "role"
			//	//	};
			//	//})
			//	.AddCookie(opt =>
			//	{
			//		opt.Cookie.HttpOnly = true;
			//		opt.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
			//		opt.Cookie.Path = "/";
			//		opt.Cookie.Name = "sf-sess";
			//		opt.LoginPath = "/user/signin";
			//		//opt.TicketDataFormat
			//	})
			//	;
			//services.AddAuthentication(
			//	"admin"
			//	)
			//	.AddCookie("admin", opt =>
			//	{
			//		opt.Cookie.HttpOnly = true;
			//		opt.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
			//		opt.Cookie.Path = "/admin/";
			//		opt.Cookie.Name = "sf-sess-admin";
			//		opt.LoginPath = "/admin/signin";
			//		opt.Cookie.Expiration = TimeSpan.FromHours(1);
			//	}
			//);



			//var securityKey = "12faod919&^*%1212";

			//var tokenValidationParameters = new TokenValidationParameters
			//{
			//	// The signing key must match!
			//	ValidateIssuerSigningKey = true,
			//	IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey)),

			//	// Validate the JWT Issuer (iss) claim
			//	ValidateIssuer = true,
			//	ValidIssuer = "SFServer",

			//	// Validate the JWT Audience (aud) claim
			//	ValidateAudience = true,
			//	ValidAudience = "SFServer",

			//	// Validate the token expiry
			//	ValidateLifetime = true,

			//	// If you want to allow a certain amount of clock drift, set that here:
			//	ClockSkew = TimeSpan.Zero,
			//	IssuerSigningKeys=null

			//}; 
			//services.AddAuthentication(
			//	JwtBearerDefaults.AuthenticationScheme
			//	)
			//	.AddJwtBearer(opt =>
			//	{
			//		opt.TokenValidationParameters = tokenValidationParameters;
			//	});

			//services.AddAuthorization(options =>
			//{
			//	//options.DefaultPolicy =
			//	//	new AuthorizationPolicyBuilder("default")
			//	//	.RequireAuthenticatedUser()
			//	//	.Build();

			//	options.AddPolicy("admin-bizness", policy =>
			//	{
			//		policy
			//			.RequireAuthenticatedUser()
			//			.RequireRole("bizadmin")
			//			.AddAuthenticationSchemes("admin-bizness")
			//			;

			//	});
			//	options.AddPolicy("admin-system", policy =>
			//	{
			//		policy
			//		.RequireAuthenticatedUser()
			//		.RequireRole("sysadmin")
			//		.AddAuthenticationSchemes("admin-system");
			//	});

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
