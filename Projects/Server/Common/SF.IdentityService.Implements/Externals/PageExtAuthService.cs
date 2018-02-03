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
			Lazy<ITimeService> TimeService
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
		}
	
		class ExtAuthSession
		{
			public string Callback { get; set; }
			public string State { get; set; }
		}
		public async Task<HttpResponseMessage> Start(
			string Provider,
			string Callback
			)
		{
			var provider = Resolver(Provider);
			var sess = new ExtAuthSession
			{
				State = Strings.NumberAndLowerChars.Random(16),
				Callback = Callback
			};
			var now = TimeService.Value.UtcNow;
			InvokeContext.Response.SetCookie(new System.Net.Cookie
			{
				Name = ExtAuthCookieHead + Provider,
				Value = await Encrypt(sess),
				Expires = TimeService.Value.UtcNow.AddHours(2)
			});

			return await provider.StartPageAuthorization(
				sess.State,
				new Uri(new Uri(InvokeContext.Request.Uri), $"/api/pageextauth/callback/{Provider}").ToString()
				);
		}
		
		public async Task<HttpResponseMessage> Callback(string Provider)
		{
			var p = Resolver(Provider);
			var re = await p.ProcessPageCallback();
			if (re == null)
			{
				await OnSignin(0);
				return HttpResponse.Redirect(new Uri(HttpSetting.HttpRoot));
			}
			var sessStr = InvokeContext.Request.GetCookie(ExtAuthCookieHead + Provider);
			if(sessStr==null)
			{
				await OnSignin(0);
				return HttpResponse.Redirect(new Uri(HttpSetting.HttpRoot));
			}

			var now = TimeService.Value.UtcNow;
			var sess =await Decrypt<ExtAuthSession>(sessStr);
			if (sess.State != re.State)
				throw new ExternalServiceException($"外部认证返回验证状态错误");

			await Signin(Provider, p, re);
			InvokeContext.Response.SetCookie(new System.Net.Cookie { Name = ExtAuthCookieHead + Provider });

			return HttpResponse.Redirect(new Uri(sess.Callback));
		}
		
	
	}
}
