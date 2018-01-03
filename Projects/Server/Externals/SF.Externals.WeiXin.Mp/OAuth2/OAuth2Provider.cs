using SF.Auth.IdentityServices.Externals;
using SF.Auth.IdentityServices.Models;
using SF.Sys;
using SF.Sys.Auth;
using SF.Sys.Collections.Generic;
using SF.Sys.HttpClients;
using SF.Sys.Logging;
using SF.Sys.NetworkService;
using SF.Sys.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SF.Externals.WeiXin.Mp.OAuth2
{
	public class AccessToken
	{
		public string access_token { get; set; }
		public string refresh_token { get; set; }
		public string openid { get; set; }
        public string unionid { get; set; }
    }

	/// <summary>
	/// 微信公众号认证
	/// </summary>
	public class OAuth2Provider : IOAuthAuthorizationProvider
	{
		public OAuth2Setting OAuthSetting { get; }
        public ILogger<OAuth2Provider> Logger { get; }
		public WeiXinMpSetting MPSetting { get; }
		public Lazy<IInvokeContext> InvokeContext { get; }
		public IHttpClient HttpClient { get; }
		public OAuth2Provider(ISettingService<WeiXinMpSetting> MpSetting,OAuth2Setting OAuthSetting, ILogService LogService, Lazy<IInvokeContext> InvokeContext, IHttpClient HttpClient)
        {
			this.OAuthSetting = OAuthSetting;
			this.MPSetting = MpSetting.Value;
			this.Logger = Logger;
			this.InvokeContext = InvokeContext;
			this.HttpClient = HttpClient;
		}
		
		public Task<HttpResponseMessage> StartPageAuthorization(string State,string Callback)
		{
			var uri = new Uri(OAuthSetting.AuthorizeUri).WithQueryString(
				 ("appid", MPSetting.AppId),
				 ("redirect_uri", Callback),
				 ("response_type", "code"),
				 ("scope", "snsapi_userinfo"),
				 ("state", State ?? "")
				).WithFragment("wechat_redirect");
			return Task.FromResult(HttpResponse.Redirect(uri));
		}


        public async Task<ProcessCallbackResult> ProcessPageCallback()
		{
			var reqUri = InvokeContext.Value.Request.Uri;
			var args = new Uri(reqUri).ParseQuery().ToDictionary(p=>p.key,p=>p.value);
			var code = args.Get("code");
			if (string.IsNullOrWhiteSpace(code))
				return null;

			var state = args.Get("state");
			var uri = new Uri(OAuthSetting.AccessTokenUri).WithQueryString(
				("appid", MPSetting.AppId),
				("secret", MPSetting.AppSecret),
				("code", code),
				("grant_type", "authorization_code")
				);
			var re = await HttpClient.From(uri).GetString();
			if (string.IsNullOrWhiteSpace(re))
				throw new ExternalServiceException(
					$"微信授权AccessToken没有返回数据，" +
					$"回调参数:{reqUri }，"+
					$"请求:{uri}");

			AccessToken token;
			try
			{
				token = Json.Parse<AccessToken>(re);
			}
			catch
			{
				throw new ExternalServiceException(
					$"微信授权AccessToken返回的数据无法解析，" +
					$"回调参数:{reqUri}，"+
					$"请求:{uri}, 应答：{re}"
					);
			}
			if(string.IsNullOrEmpty(token.access_token) ||string.IsNullOrEmpty(token.openid))
				throw new ExternalServiceException(
					$"微信授权AccessToken返回的数据中没有access_token或openid，" +
					$"回调参数:{reqUri}，"+
					$"请求:{uri}, 应答：{re}"
					);

			return new ProcessCallbackResult {
				Credentials =new[]{
					new ClaimValue(PredefinedClaimTypes.WeiXinUnionId, token.unionid),
					new ClaimValue(PredefinedClaimTypes.WeiXinMPId, token.openid),
					
				}.Where(c=>!c.Value.IsNullOrEmpty()).ToArray(),
				Token = Json.Stringify(token),
				State = state
			};

		}

		public async Task<string> RefreshToken(string Token)
		{
			var tokens = Json.Parse<AccessToken>(Token);
			if (string.IsNullOrEmpty(tokens.refresh_token))
				throw new ArgumentException("没有refresh_token");

			var uri = new Uri(OAuthSetting.RefreshTokenUri).WithQueryString(
				("appid", MPSetting.AppId),
				("grant_type", "authorization_code"),
				("refresh_token", tokens.refresh_token)
				);

			var re = await HttpClient.From(uri).GetString();
			if (string.IsNullOrWhiteSpace(re))
				throw new ArgumentException(
					$"微信授权RefreshToken没有返回数据，" +
					$"旧令牌:{Token}，" +
					$"请求:{uri}");

			AccessToken newToken;
			try
			{
				newToken = Json.Parse<AccessToken>(re);
			}
			catch
			{
				throw new ExternalServiceException(
					$"微信授权RefreshToken返回的数据无法解析，" +
					$"旧令牌:{Token}，" +
					$"请求:{uri}, 应答：{re}"
					);
			}
			if (string.IsNullOrEmpty(newToken.access_token) || string.IsNullOrEmpty(newToken.openid))
				throw new ExternalServiceException(
					$"微信授权RefreshToken返回的数据中没有access_token或openid，" +
					$"旧令牌:{Token}，" +
					$"请求:{uri}, 应答：{re}"
					);

			return Json.Stringify(newToken);

		}
		public class WeichatUserInfo
		{
			public string openid { get; set; }
			public string nickname { get; set; }
			public string sex { get; set; }
			public string province { get; set; }
			public string city { get; set; }
			public string country { get; set; }
			public string headimgurl { get; set; }
			public string unionid { get; set; }
		}

		public async Task<UserInfo> GetUserInfo(string Token)
		{
			var tokens = Json.Parse<AccessToken>(Token);
			var uri = new Uri(OAuthSetting.UserInfoUri).WithQueryString(
				("access_token", tokens.access_token),
				("openid", tokens.openid),
				("lang", "zh_CN")
				);

			var re = await HttpClient.From(uri).GetString();
			if (string.IsNullOrWhiteSpace(re))
				throw new ExternalServiceException(
					$"微信授权UserInfo没有返回数据，" +
					$"令牌:{Token}，" +
					$"请求:{uri}");

            Logger.Info($"获取微信用户信息:openid:{tokens.openid} access_token:{tokens.access_token} {re}");
            WeichatUserInfo ui;
			try
			{
				ui = Json.Parse<WeichatUserInfo>(re);
			}
			catch
			{
				throw new ExternalServiceException(
					$"微信授权UserInfo返回的数据无法解析，" +
					$"令牌:{Token}，" +
					$"请求:{uri}, 应答：{re}"
					);
			}
			if (string.IsNullOrEmpty(ui.nickname))
				throw new ExternalServiceException(
					$"微信授权UserInfo返回的数据中没有用户名，" +
					$"令牌:{Token}，" +
					$"请求:{uri}, 应答：{re}"
					);

			return new UserInfo
			{
				Claims = new[]
				{
					new ClaimValue(PredefinedClaimTypes.WeiXinUnionId, ui.unionid),
					new ClaimValue(PredefinedClaimTypes.WeiXinMPId, ui.openid),
					new ClaimValue(PredefinedClaimTypes.Name,ui.nickname),
					new ClaimValue(PredefinedClaimTypes.Country,ui.country),
					new ClaimValue(PredefinedClaimTypes.Province,ui.province),
					new ClaimValue(PredefinedClaimTypes.Icon,ui.headimgurl),
					new ClaimValue(PredefinedClaimTypes.Sex,(ui.sex == "1" ? SexType.Male : ui.sex == "2" ? SexType.Female : SexType.Unknown).ToString()),

				}.Where(c => !c.Value.IsNullOrEmpty()).ToArray(),
				Credentials = new []
				{
					new ClaimValue(PredefinedClaimTypes.WeiXinUnionId, ui.unionid),
					new ClaimValue(PredefinedClaimTypes.WeiXinMPId, ui.openid),
				}.Where(c => !c.Value.IsNullOrEmpty()).ToArray()
			};
		}

		public class ValidateAccessTokenResult
		{
			public int errcode { get; set; }
			public string errmsg { get; set; }
		}
		public async Task<string> ValidateAccessToken(string Token)
		{
			var tokens = Json.Parse<AccessToken>(Token);
			var uri = new Uri(OAuthSetting.AccessTokenValidatorUri).WithQueryString(
				("access_token", tokens.access_token),
				("openid", tokens.openid)
				);

			var re = await HttpClient.From(uri).GetString();
			if (string.IsNullOrWhiteSpace(re))
				throw new ExternalServiceException(
					$"微信授权码验证没有返回数据，" +
					$"令牌:{Token}，" +
					$"请求:{uri}");

			ValidateAccessTokenResult result;
			try
			{
				result = Json.Parse<ValidateAccessTokenResult>(re);
			}
			catch
			{
				throw new ExternalServiceException(
					$"微信授权码验证返回的数据无法解析，" +
					$"令牌:{Token}，" +
					$"请求:{uri}, 应答：{re}"
					);
			}
			if (result.errcode == 0)
				return null;
			return result.errcode + ":" + (result.errmsg??"");
		}

		public Task<Dictionary<string, string>> StartClientAuthorization()
		{
			throw new NotImplementedException();
		}

		public Task<ProcessCallbackResult> ProcessClientCallback(Dictionary<string, string> Argument)
		{
			throw new NotImplementedException();
		}
	}
}
