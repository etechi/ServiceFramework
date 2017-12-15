using System;
using System.Threading.Tasks;
using System.Net.Http;
using SF.Sys.Caching;
using SF.Sys.Services;
using System.Security.Claims;
using SF.Auth.IdentityServices.Internals;
using System.Collections.Generic;
using SF.Sys;
using System.Linq;
using SF.Sys.Linq;
using SF.Auth.IdentityServices.Managers;
using SF.Sys.NetworkService;
using SF.Sys.TimeServices;
using SF.Sys.Settings;
using SF.Services.Security;
using SF.Sys.Auth;
using SF.Auth.IdentityServices.Models;
using SF.Sys.Clients;
using SF.Common.Media;
using SF.Sys.Logging;

namespace SF.Auth.IdentityServices.Externals
{
	/// <summary>
	/// 客户端外部认证
	/// </summary>
	public class ClientExtAuthService :
		BaseExtAuthService,
		IClientExtAuthService
	{
		public Lazy<IAccessTokenHandler> AccessTokenGenerator { get; }
		public Lazy<IUserProfileService> UserProfileService { get; }
		
		public ClientExtAuthService(
			NamedServiceResolver<IExternalAuthorizationProvider> Resolver, 
			Lazy<IUserCredentialStorage> UserCredentialStorage, 
			Lazy<IUserManager> UserManager, 
			IDataProtector DataProtector, 
			Lazy<IClientService> ClientService,
			 Lazy<IMediaManager> MediaManager,
			 ILogger<ClientExtAuthService> Logger,
			Lazy<IUserProfileService> UserProfileService,
			Lazy<IAccessTokenHandler> AccessTokenGenerator,
			Lazy<ITimeService> TimeService
			) : base(
				Resolver, 
				UserCredentialStorage, 
				UserManager, 
				DataProtector,
				ClientService,
				MediaManager,
				TimeService,
				Logger
				)
		{
			this.UserProfileService = UserProfileService;
			this.AccessTokenGenerator = AccessTokenGenerator;
		}
		
		class ClientExtAuthState
		{
			public string Provider { get; set; }
			public string ClientId { get; set; }
		}
		public async Task<ExtAuthArgument> GetAuthArgument(string Provider,string ClientId)
		{
			var provider = Resolver(Provider) ??
							throw new ArgumentException("找不到外部认证提供者:" + Provider);
			var encState = await Encrypt(new ClientExtAuthState
			{
				Provider=Provider,
				ClientId=ClientId
			});

			var re = new ExtAuthArgument
			{
				Arguments = await provider.StartClientAuthorization(),
				Provider = Provider,
				State = encState
			};
			return re;
		}
		public async Task<AuthCallbackResult> AuthCallback(AuthCallbackArgument Arg)
		{
			var decState = await Decrypt<ClientExtAuthState>(Arg.State);

			var provider = Resolver(decState.Provider);
			var re = await provider.ProcessClientCallback(Arg.Arguments);
			var uid = await Signin(decState.Provider, provider, re);

			return new AuthCallbackResult
			{
				AccessToken = await AccessTokenGenerator.Value.Generate(uid, decState.ClientId, null, null),
				User = await UserProfileService.Value.GetUser(uid)
			};
		}


	}
}
