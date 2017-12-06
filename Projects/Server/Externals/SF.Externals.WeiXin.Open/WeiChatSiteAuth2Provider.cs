using ServiceProtocol.Annotations;
using ServiceProtocol.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProtocol.Auth.Externals.Providers.WeiChat
{


    [TypeDisplay(Name ="微信网站认证")]
    [ServiceProvider(typeof(IExternalAuthorizationProvider),typeof(WeiChatSiteAuth2Setting))]
	public class WeiChatSiteAuth2Provider : IOAuthAuthorizationProvider
	{
		public WeiChatSiteAuth2Setting Setting { get; }
        public ILogger Logger { get; }

        public WeiChatSiteAuth2Provider(WeiChatSiteAuth2Setting Setting, ILogService LogService)
        {
			this.Setting = Setting;
            this.Logger = LogService.GetLogger(GetType());
        }
		
		public Task<HttpResponseMessage> StartAuthorization(string State,string ClientType,string Callback)
		{
			var uri = new Uri(Setting.AuthorizeUri).WithQueryString(
				new KeyValuePair<string, string>("appid", Setting.AppId),
				new KeyValuePair<string, string>("redirect_uri", Callback),
				new KeyValuePair<string, string>("response_type", "code"),
				new KeyValuePair<string, string>("scope", "snsapi_login"),
				new KeyValuePair<string, string>("state", State ?? "")
				).WithFragment("wechat_redirect");
			var msg = new HttpResponseMessage(System.Net.HttpStatusCode.Redirect);
			msg.Headers.Location = uri;
			return Task.FromResult(msg);
		}

		

		public async Task<ProcessCallbackResult> ProcessCallback(HttpRequestMessage Request)
		{
			var args = Request.RequestUri.ParseQuery().ToDictionary();
			var code = args.Get("code");
            if (string.IsNullOrWhiteSpace(code))
                return null;

			var state = args.Get("state");
			var uri = new Uri(Setting.AccessTokenUri).WithQueryString(
				new KeyValuePair<string, string>("appid", Setting.AppId),
				new KeyValuePair<string, string>("secret", Setting.AppSecret),
				new KeyValuePair<string, string>("code", code),
				new KeyValuePair<string, string>("grant_type", "authorization_code")
				);
			var re = await Http.Get(uri);
			if (string.IsNullOrWhiteSpace(re))
				throw new ArgumentException(
					$"微信授权AccessToken没有返回数据，" +
					$"回调参数:{Request.RequestUri }，"+
					$"请求:{uri}");

			AccessToken token;
			try
			{
				token = Json.Decode<AccessToken>(re);
			}
			catch
			{
				throw new ArgumentException(
					$"微信授权AccessToken返回的数据无法解析，" +
					$"回调参数:{Request.RequestUri}，"+
					$"请求:{uri}, 应答：{re}"
					);
			}
			if(string.IsNullOrEmpty(token.access_token) ||string.IsNullOrEmpty(token.openid))
				throw new ArgumentException(
					$"微信授权AccessToken返回的数据中没有access_token或openid，" +
					$"回调参数:{Request.RequestUri }，"+
					$"请求:{uri}, 应答：{re}"
					);

			return new ProcessCallbackResult {
				ExtIdent= token.openid,
				Token = Json.Encode(token),
				State = state
			};

		}

		public async Task<string> RefreshToken(string Token)
		{
			var tokens = Json.Decode<AccessToken>(Token);
			if (string.IsNullOrEmpty(tokens.refresh_token))
				throw new ArgumentException("没有refresh_token");

			var uri = new Uri(Setting.RefreshTokenUri).WithQueryString(
				new KeyValuePair<string, string>("appid", Setting.AppId),
				new KeyValuePair<string, string>("grant_type", "authorization_code"),
				new KeyValuePair<string, string>("refresh_token", tokens.refresh_token)
				);

			var re = await Http.Get(uri);
			if (string.IsNullOrWhiteSpace(re))
				throw new ArgumentException(
					$"微信授权RefreshToken没有返回数据，" +
					$"旧令牌:{Token}，" +
					$"请求:{uri}");

			AccessToken newToken;
			try
			{
				newToken = Json.Decode<AccessToken>(re);
			}
			catch
			{
				throw new ArgumentException(
					$"微信授权RefreshToken返回的数据无法解析，" +
					$"旧令牌:{Token}，" +
					$"请求:{uri}, 应答：{re}"
					);
			}
			if (string.IsNullOrEmpty(newToken.access_token) || string.IsNullOrEmpty(newToken.openid))
				throw new ArgumentException(
					$"微信授权RefreshToken返回的数据中没有access_token或openid，" +
					$"旧令牌:{Token}，" +
					$"请求:{uri}, 应答：{re}"
					);

			return Json.Encode(newToken);

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
			var tokens = Json.Decode<AccessToken>(Token);
			var uri = new Uri(Setting.UserInfoUri).WithQueryString(
				new KeyValuePair<string, string>("access_token", tokens.access_token),
				new KeyValuePair<string, string>("openid", tokens.openid),
				new KeyValuePair<string, string>("lang", "zh_CN")
				);

			var re = await Http.Get(uri);
			if (string.IsNullOrWhiteSpace(re))
				throw new ArgumentException(
					$"微信授权UserInfo没有返回数据，" +
					$"令牌:{Token}，" +
					$"请求:{uri}");

            Logger.Info($"获取微信用户信息:openid:{tokens.openid} access_token:{tokens.access_token} {re}");
            WeichatUserInfo ui;
			try
			{
				ui = Json.Decode<WeichatUserInfo>(re);
			}
			catch
			{
				throw new ArgumentException(
					$"微信授权UserInfo返回的数据无法解析，" +
					$"令牌:{Token}，" +
					$"请求:{uri}, 应答：{re}"
					);
			}
			if (string.IsNullOrEmpty(ui.nickname))
				throw new ArgumentException(
					$"微信授权UserInfo返回的数据中没有用户名，" +
					$"令牌:{Token}，" +
					$"请求:{uri}, 应答：{re}"
					);
            return new UserInfo
            {
                Ident = ui.unionid ?? ui.openid,
                ExtIdent = ui.openid,
                Country = ui.country,
                Province = ui.province,
                City = ui.city,
                ExtraData = null,
                IconUrl = ui.headimgurl,
                NickName = ui.nickname,
                Sex = ui.sex == "1" ? SexType.Male : ui.sex == "2" ? SexType.Female : SexType.Unknown
            };

        }

        public class ValidateAccessTokenResult
		{
			public int errcode { get; set; }
			public string errmsg { get; set; }
		}
		public async Task<string> ValidateAccessToken(string Token)
		{
			var tokens = Json.Decode<AccessToken>(Token);
			var uri = new Uri(Setting.AccessTokenValidatorUri).WithQueryString(
				new KeyValuePair<string, string>("access_token", tokens.access_token),
				new KeyValuePair<string, string>("openid", tokens.openid)
				);

			var re = await Http.Get(uri);
			if (string.IsNullOrWhiteSpace(re))
				throw new ArgumentException(
					$"微信授权码验证没有返回数据，" +
					$"令牌:{Token}，" +
					$"请求:{uri}");

			ValidateAccessTokenResult result;
			try
			{
				result = Json.Decode<ValidateAccessTokenResult>(re);
			}
			catch
			{
				throw new ArgumentException(
					$"微信授权码验证返回的数据无法解析，" +
					$"令牌:{Token}，" +
					$"请求:{uri}, 应答：{re}"
					);
			}
			if (result.errcode == 0)
				return null;
			return result.errcode + ":" + (result.errmsg??"");
		}
	}
}
