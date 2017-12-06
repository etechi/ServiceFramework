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
        static SyncScope accessTokenSyncScope { get; } = new SyncScope();
        static SyncScope jsApiTicketSyncScope { get; } = new SyncScope();
        static string AccessTokenCacheKey { get; } = typeof(AccessTokenManager).FullName + "/AccessToken";
        static string JsApiTicketCacheKey { get; } = typeof(AccessTokenManager).FullName + "/JSApiTicket";

        ILocalCache Cache { get; }
        ITimeService TimeService { get; }

        WeiXinSetting Setting { get; }

        public AccessTokenManager(
            Caching.ICache Cache,
            Times.ITimeService TimeService,
            WeiXinSetting Setting
            )

        {
            this.Setting = Setting;
            this.Cache = Cache;
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
            var token = this.Cache.Get(AccessTokenCacheKey) as string;
            if (token != null)
                return token;

            return await accessTokenSyncScope.Sync(async () =>
            {
                token = this.Cache.Get(AccessTokenCacheKey) as string;
                if (token != null)
                    return token;

                var uri = new Uri(Setting.ApiUriBase + "token").WithQueryString(
                    Tuple.Create("grant_type", "client_credential"),
                    Tuple.Create("appid", Setting.AppId),
                    Tuple.Create("secret", Setting.AppSecret)
                    );
                var re = await Functional.Retry(() => uri.GetString());
                var resp = Json.Decode<AccessTokenResponse>(re);
                if (resp.access_token == null)
                    throw new Exception(resp.errcode + ":" + resp.errmsg);

                return this.Cache.AddOrGetExisting(
                    AccessTokenCacheKey,
                    resp.access_token,
                    TimeService.Now.AddSeconds(resp.expires_in / 2)
                    ) as string ?? resp.access_token;
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
            var ticket = this.Cache.Get(JsApiTicketCacheKey) as string;
            if (ticket != null)
                return ticket;

            return await jsApiTicketSyncScope.Sync(async () =>
            {
                ticket = this.Cache.Get(JsApiTicketCacheKey) as string;
                if (ticket != null)
                    return ticket;

                var access_token = await GetAccessToken();
                var uri = new Uri(Setting.ApiUriBase + "ticket/getticket").WithQueryString(
                    Tuple.Create("access_token",access_token),
                    Tuple.Create("type", "jsapi")
                    );
                var re = await Functional.Retry(() => uri.GetString());
                var resp = Json.Decode<JsApiTicketResponse>(re);
                if (resp.ticket == null)
                    throw new Exception(resp.errcode + ":" + resp.errmsg);

                return this.Cache.AddOrGetExisting(
                    JsApiTicketCacheKey,
                    resp.ticket,
                    TimeService.Now.AddSeconds(resp.expires_in / 2)
                    ) as string ?? resp.ticket;
            });
        }
    }
}
