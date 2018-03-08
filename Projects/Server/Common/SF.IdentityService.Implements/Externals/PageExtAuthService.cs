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
	/// 外部认证
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	public class PageExtAuthService :
		BaseExtAuthService,
		IPageExtAuthService
	{
		public Lazy<IAccessTokenGenerator> AccessTokenGenerator { get; }

		public IInvokeContext InvokeContext { get; }
		public HttpSetting HttpSetting { get; }
		public PageExtAuthService(
			NamedServiceResolver<IExternalAuthorizationProvider> Resolver, 
			Lazy<IUserCredentialStorage> UserCredentialStorage, 
			Lazy<IUserManager> UserManager, 
			IDataProtector DataProtector, 
			Lazy<IClientService> ClientService,
			 Lazy<IMediaService> MediaService,
			 ILogger<PageExtAuthService> Logger,
			IInvokeContext InvokeContext,
			ISettingService<HttpSetting> HttpSetting,
			Lazy<ITimeService> TimeService,
			 Lazy<IAccessTokenGenerator> AccessTokenGenerator
			) : base(
				Resolver, 
				UserCredentialStorage, 
				UserManager, 
				DataProtector, 
				ClientService,
				MediaService,
				TimeService,
				Logger
				)
		{
			this.InvokeContext = InvokeContext;
			this.HttpSetting = HttpSetting.Value;
			this.AccessTokenGenerator = AccessTokenGenerator;
		}
	
		class ExtAuthSession
		{
			public string Callback { get; set; }
			public string State { get; set; }
			public string ClientState { get; set; }
			public string ClientId { get; set; }
		}
		HttpResponseMessage Error(string Callback, string message)
		{
			return HttpResponse.Redirect(
				new Uri(Callback).WithFragment("message=" + Uri.EscapeDataString(message))
				);
		}
		public async Task<HttpResponseMessage> Start(
			string Provider,
			string Callback,
			string ClientId,
			string ClientState
			)
		{
			if (Callback.IsNullOrEmpty())
				throw new ArgumentException("未指定回调URI");

			if (string.IsNullOrEmpty(Provider))
				return Error(Callback, "认证提供者不能为空");
			if(ClientId.IsNullOrEmpty())
				return Error(Callback, "未指定客户端ID");
			if (ClientState.IsNullOrEmpty())
				return Error(Callback, "未指定客户端状态");

			var provider = Resolver(Provider);
			if (provider == null)
				throw new ArgumentException("不支持此外部认证提供者:" + Provider);
			var sess = new ExtAuthSession
			{
				State = Strings.NumberAndLowerChars.Random(16),
				Callback = Callback,
				ClientId=ClientId,
				ClientState= ClientState
			};
			var now = TimeService.Value.UtcNow;
			InvokeContext.Response.SetCookie(new System.Net.Cookie
			{
				Name = ExtAuthCookieHead + Provider,
				Value = await Encrypt(sess),
				Expires = TimeService.Value.UtcNow.AddHours(2)
			});

			return await provider.StartPageAuthorization(
				sess.State+":"+sess.ClientState,
				new Uri(new Uri(InvokeContext.Request.Uri), $"/api/pageextauth/callback/{Provider}").ToString()
				);
		}
		
		public async Task<HttpResponseMessage> Callback(string Id)
		{
			var p = Resolver(Id);
			var re = await p.ProcessPageCallback();
			if (re == null)
			{
				await OnSignin(0);
				return HttpResponse.Redirect(new Uri(HttpSetting.HttpRoot));
			}
			var sessStr = InvokeContext.Request.GetCookie(ExtAuthCookieHead + Id);
			if(sessStr==null)
			{
				await OnSignin(0);
				return HttpResponse.Redirect(new Uri(HttpSetting.HttpRoot));
			}

			var now = TimeService.Value.UtcNow;
			var sess =await Decrypt<ExtAuthSession>(sessStr);
			if (sess.State+":"+sess.ClientState != re.State)
				throw new ExternalServiceException($"外部认证返回验证状态错误");

			var uid=await Signin(Id, p, re);
			
			//清除外部认证Cookie
			InvokeContext.Response.SetCookie(
				new System.Net.Cookie {
					Name = ExtAuthCookieHead + Id
				});

			//ClientService.Value.SignInAsync();
			var access_token = await AccessTokenGenerator.Value.Generate(uid, sess.ClientId, null, null);

			return HttpResponse.Redirect(
				new Uri(new Uri(InvokeContext.Request.Uri), sess.Callback)
					.WithFragment($"state={Uri.EscapeDataString(sess.ClientState)}&access_token=" + Uri.EscapeDataString(access_token.Token))
					);
		}
		
	
	}
}
