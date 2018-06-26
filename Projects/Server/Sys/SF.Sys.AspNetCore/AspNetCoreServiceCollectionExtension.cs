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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SF.Sys.AspNetCore;
using SF.Sys.AspNetCore.Auth;
using SF.Sys.Auth;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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
			sc.AddAspNetCoreHostingService();
			var services = sc.AsMicrosoftServiceCollection();

			if (!sc.Any(s => s.InterfaceType == typeof(Microsoft.AspNetCore.Hosting.IHostingEnvironment)))
				throw new InvalidOperationException("服务集合中找不到AspNetCore主机环境服务 IHostingEnvironment, 请确保在初始化AspNetCore服务之前，主机服务已加入到服务集合中");

			services.AddAspNetCoreSystemServices();

			var mvcbuilder = Microsoft.Extensions.DependencyInjection.MvcServiceCollectionExtensions.AddMvc(services);
			Microsoft.Extensions.DependencyInjection.MvcJsonMvcBuilderExtensions.AddJsonOptions(
				mvcbuilder,
				s =>
					SF.Sys.Serialization.Newtonsoft.JsonSerializer.ApplySetting(
						s.SerializerSettings,
						new SF.Sys.Serialization.JsonSetting
						{
							//IgnoreDefaultValue = true,
							WithType = false
						})
				);
			return sc;
		}
		public static IServiceCollection AddJwtAuthSupports(
			this IServiceCollection services,
			JwtAuthSettings Settings=null
			)
		{
			JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
			services.AddSingleton(sp =>
			{
				Settings = sp.LoadServiceConfigSetting(Settings).Result;
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
				return tokenValidationParameters;
			});

			var mssc = services.AsMicrosoftServiceCollection();
			mssc.AddAuthentication(options =>
				{
					options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer();

			mssc.AddOptions();
			mssc.AddSingleton(sp=>
			{
				var parameter = sp.Resolve<TokenValidationParameters>();
				return (IConfigureOptions<JwtBearerOptions>)new ConfigureNamedOptions<JwtBearerOptions>(
					JwtBearerDefaults.AuthenticationScheme, 
					options => {
						options.TokenValidationParameters = parameter;
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
			});


			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddAspNetCoreCommonAuthorization(
				sp=>
				{
					var parameter = sp.Resolve<TokenValidationParameters>();
					return new TokenProviderOptions
					{
						Issuer = Settings.Issuer,
						Audience = Settings.Audience,
						SigningCredentials = new SigningCredentials(
							parameter.IssuerSigningKey,
							SecurityAlgorithms.HmacSha256
							)
					};
				},
				sp =>
				{
					var parameter = sp.Resolve<TokenValidationParameters>();
					return new AccessTokenValidatorArguments
					{
						Algorithm = SecurityAlgorithms.HmacSha256,
						ValidationParameters = parameter
					};
				}
			);

			return services;
		}
	}
}
