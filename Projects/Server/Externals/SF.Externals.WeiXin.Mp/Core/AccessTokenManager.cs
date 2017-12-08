using SF.Sys;
using SF.Sys.Caching;
using SF.Sys.TimeServices;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Externals.WeiXin.Mp.Core
{
	public class AccessTokenManager : 
        IAccessTokenManager
    {

        SyncScope accessTokenSyncScope { get; } = new SyncScope();
        SyncScope jsApiTicketSyncScope { get; } = new SyncScope();
		string _AccessToken;
		DateTime _AccessTokenExpire;

		string _JsApiTicket;
		DateTime _JsApiTicketExpire;

		ITimeService TimeService { get; }

		WeiXinMpSetting Setting { get; }

        public AccessTokenManager(
			ILocalCache<AccessToken> Cache,
            ITimeService TimeService,
			WeiXinMpSetting Setting
            )

        {
            this.Setting = Setting;
            this.TimeService = TimeService;
        }
        class AccessTokenResponse
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public int errcode { get; set; }
            public string errmsg { get; set; }
        }

        public async Task<string> GetAccessToken()
        {
			var now = TimeService.Now;
			if (_AccessToken != null && _AccessTokenExpire>now)
				return _AccessToken;

            return await accessTokenSyncScope.Sync(async () =>
            {
				if (_AccessToken != null && _AccessTokenExpire > now)
					return _AccessToken;

				var uri = new Uri(Setting.ApiUriBase + "token").WithQueryString(
                    ("grant_type", "client_credential"),
                    ("appid", Setting.AppId),
                    ("secret", Setting.AppSecret)
                    );
                var re = await TaskUtils.Retry(() => uri.GetString());
                var resp = Json.Parse<AccessTokenResponse>(re);
                if (resp.access_token == null)
                    throw new ExternalServiceException("获取微信公众号访问令牌失败："+resp.errcode + ":" + resp.errmsg);

				_AccessToken = resp.access_token;
				_AccessTokenExpire = TimeService.Now.AddSeconds(resp.expires_in / 2);
				return _AccessToken;
            });
        }

        class JsApiTicketResponse
        {
            public string ticket { get; set; }
            public int expires_in { get; set; }
            public int errcode { get; set; }
            public string errmsg { get; set; }
        }

        public async Task<string> GetJsApiTicket()
        {
			var now = TimeService.Now;
			if (_JsApiTicket != null && now < _JsApiTicketExpire)
				return _JsApiTicket;

            return await jsApiTicketSyncScope.Sync(async () =>
            {
				if (_JsApiTicket != null && now < _JsApiTicketExpire)
					return _JsApiTicket;

				var access_token = await GetAccessToken();
                var uri = new Uri(Setting.ApiUriBase + "ticket/getticket").WithQueryString(
                    ("access_token",access_token),
                    ("type", "jsapi")
                    );
                var re = await TaskUtils.Retry(() => uri.GetString());
                var resp = Json.Parse<JsApiTicketResponse>(re);
                if (resp.ticket == null)
                    throw new ExternalServiceException("获取微信公众号JSTicket失败:"+resp.errcode + ":" + resp.errmsg);

				_JsApiTicket = resp.ticket;
				_JsApiTicketExpire = TimeService.Now.AddSeconds(resp.expires_in / 2);
				return _JsApiTicket;
            });
        }
    }
}
