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

namespace SF.Auth.IdentityServices.Externals
{
	public class ExtAuthState
	{
		public string Source { get; set; }
		public string Invitor { get; set; }
		public string Value { get; set; }
	}

	/// <summary>
	/// 外部认证
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	public abstract class ExtAuthService :
		IExtAuthService
	{
        public NamedServiceResolver<IExternalAuthorizationProvider> Resolver { get; }
		public IUserCredentialStorage UserCredentialStorage { get; }
        public ILocalCache<ExtAuthState> Cache { get; }
		public IUserManager UserManager { get; }
		public IInvokeContext InvokeContext { get; }
		public Lazy<ITimeService> TimeService { get; }
		public HttpSetting HttpSetting { get; }
		public IDataProtector DataProtector { get; }
		public ExtAuthService(
			NamedServiceResolver<IExternalAuthorizationProvider> Resolver,
			IUserCredentialStorage UserCredentialStorage,
			ILocalCache<ExtAuthState> Cache,
			IUserManager UserManager,
			IInvokeContext InvokeContext,
			ISettingService<HttpSetting> HttpSetting,
			IDataProtector DataProtector
			)
		{
            this.Cache = Cache;
			this.Resolver = Resolver;
			this.UserCredentialStorage = UserCredentialStorage;
			this.UserManager= UserManager;
			this.InvokeContext = InvokeContext;
			this.HttpSetting = HttpSetting.Value;
			this.DataProtector = DataProtector;
		}
		
        public Task<string> GetAuthState(string Source, string Invitor, string State)
        {
            var key = Guid.NewGuid().ToString("N");
            Cache.Set(
				key, 
				new ExtAuthState
            {
                Invitor = Invitor,
                Source = Source,
                Value = State
            }, DateTime.Now.AddHours(1));
			return Task.FromResult(key);
        }
		const string ExtAuthCookieHead = "SFEAS-";

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
				Value = (await DataProtector.Encrypt("外部认证Cookie",Json.Stringify(sess).UTF8Bytes(), now.AddHours(4), null)).Base64(),
				Expires=TimeService.Value.UtcNow.AddHours(2)
			});

            return await provider.StartAuthorization(
				sess.State,
				new Uri(new Uri(InvokeContext.Request.Uri), $"/api/externalauth/callback/{Provider}").ToString()
				);
		}
		protected abstract Task<long> OnSignup(string ProviderType,UserInfo UserInfo);
		protected abstract Task OnSignin(long UserId);

		async Task<long> TryFindUserId(Claim[] Credentials)
		{
			long curUserId = 0;
			foreach (var c in Credentials)
			{
				var ci = await UserCredentialStorage.Find(c.Type, c.Value);
				if (ci == null)
					continue;
				if (curUserId == 0)
					curUserId = ci.UserId;
				else if(curUserId!=ci.UserId)
					throw new ExternalServiceException($"外部用户的凭证绑定了多个用户:{Credentials.Select(t=>t.Type+"="+t.Value).Join(";")}=>{curUserId},{ci.UserId}");
			}
			return curUserId;
		}
		public async Task<HttpResponseMessage> Callback(string Provider)
		{
			var provider = Resolver(Provider);
			var re = await provider.ProcessCallback();
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
			var sess = Json.Parse<ExtAuthSession>(
				(await DataProtector.Decrypt("外部认证Cookie", sessStr.Base64(), now, null)).UTF8String()
				);
			if (sess.State != re.State)
				throw new ExternalServiceException($"外部认证返回验证状态错误");

			var iid = await TryFindUserId(re.Credentials);
			if(iid==0)
			{
				var ui=await provider.GetUserInfo(re.Token);
				iid = await TryFindUserId(ui.Credentials);
                if (iid==0)
                    iid = await OnSignup(Provider, ui);
				foreach (var c in ui.Credentials)
					await UserCredentialStorage.FindOrBind(c.Type, c.Value, true, iid);
			}
            await OnSignin(iid);
			InvokeContext.Response.SetCookie(new System.Net.Cookie { Name = ExtAuthCookieHead + Provider });

			return HttpResponse.Redirect(new Uri(sess.Callback));
		}

	}
}
