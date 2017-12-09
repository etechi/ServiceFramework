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
	/// 基础外部认证支持
	/// </summary>
	public class BaseExtAuthService
	{
		public NamedServiceResolver<IExternalAuthorizationProvider> Resolver { get; }
		public Lazy<IUserCredentialStorage> UserCredentialStorage { get; }
		public Lazy<IUserManager> UserManager { get; }
		public Lazy<ITimeService> TimeService { get; }
		public IDataProtector DataProtector { get; }
		public Lazy<IClientService> ClientService { get; }
		public Lazy<IMediaManager> MediaManager { get; }
		public ILogger Logger { get; }
		public BaseExtAuthService(
			NamedServiceResolver<IExternalAuthorizationProvider> Resolver,
			Lazy<IUserCredentialStorage> UserCredentialStorage,
			Lazy<IUserManager> UserManager,
			IDataProtector DataProtector,
			 Lazy<IClientService> ClientService,
			 Lazy<IMediaManager> MediaManager,
			ILogger Logger
			)
		{
			this.Resolver = Resolver;
			this.UserCredentialStorage = UserCredentialStorage;
			this.UserManager = UserManager;
			this.DataProtector = DataProtector;
			this.ClientService = ClientService;
			this.MediaManager = MediaManager;
			this.Logger = Logger;
		}


		protected const string ExtAuthCookieHead = "SFEAS-";
		protected async Task<string> Encrypt<T>(T data)
		{
			var sess = Json.Stringify(data);
			var re = await DataProtector.Encrypt("外部认证会话", sess.UTF8Bytes(), TimeService.Value.Now.AddHours(4), null);
			return re.Base64();
		}
		protected async Task<T> Decrypt<T>(string data)
		{
			var now = TimeService.Value.UtcNow;
			var re = await DataProtector.Decrypt("外部认证会话", data.Base64(), now, null);
			var sess = Json.Parse<T>(re.UTF8String());
			return sess;
		}
		
		protected virtual async Task<long> OnSignup(string ProviderType, UserInfo UserInfo)
		{
			var name = UserInfo.Claims.FirstOrDefault(c => c.TypeId == PredefinedClaimTypes.Name)?.Value ?? "用户"+Strings.Numbers.Random(6);
			var icon = UserInfo.Claims.FirstOrDefault(c => c.TypeId == PredefinedClaimTypes.Icon);
			if (icon!=null)
			{
				try
				{
					icon.Value = await MediaManager.Value.TryCreateByImageUri("ms", icon.Value, 1024 * 1024);
				}
				catch (Exception e)
				{
					Logger.Warn(e, $"获取用户图标时发生异常：用户:{name} Uri:{icon.Value}");
				}
			}

			var uid=await UserManager.Value.Create(new UserCreateArgument
			{
				UserAgent= ClientService.Value.UserAgent,
				ClaimTypeId=UserInfo.Credentials[0].TypeId,
				CredentialValue=UserInfo.Credentials[0].Value,
				SecurityStamp=Bytes.Random(16),
				ClaimValues=UserInfo.Claims,
				User=new User
				{
					Icon =icon.Value,
					Name = name
				}
			});
			return uid;
		}
		protected virtual Task OnSignin(long UserId) {
			return Task.CompletedTask;
		}

		protected async Task<long> TryFindUserId(ClaimValue[] Credentials)
		{
			long curUserId = 0;
			foreach (var c in Credentials)
			{
				var ci = await UserCredentialStorage.Value.Find(c.TypeId, c.Value);
				if (ci == null)
					continue;
				if (curUserId == 0)
					curUserId = ci.UserId;
				else if(curUserId!=ci.UserId)
					throw new ExternalServiceException($"外部用户的凭证绑定了多个用户:{Credentials.Select(t=>t.TypeId+"="+t.Value).Join(";")}=>{curUserId},{ci.UserId}");
			}
			return curUserId;
		}
		
		protected async Task<long> Signin(string ProviderId,IExternalAuthorizationProvider provider, ProcessCallbackResult re)
		{
			var iid = await TryFindUserId(re.Credentials);
			if (iid == 0)
			{
				var ui = await provider.GetUserInfo(re.Token);
				iid = await TryFindUserId(ui.Credentials);
				if (iid == 0)
					iid = await OnSignup(ProviderId, ui);
				foreach (var c in ui.Credentials)
					await UserCredentialStorage.Value.FindOrBind(c.TypeId, c.Value, true, iid);
			}
			await OnSignin(iid);
			return iid;
		}
	}
}
