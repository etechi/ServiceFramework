using SF.Common.Media;
using SF.Sys;
using SF.Sys.Settings;
using SF.Sys.TimeServices;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SF.Externals.WeiXin.Mp.Core
{

	public class WeiXinClient : IWeiXinClient
    {
        IAccessTokenManager AccessTokenManager { get; }
        WeiXinMpSetting Setting { get; }
        Lazy<ITimeService> TimeService { get; }
        Lazy<IMediaManager> MediaManager { get; }
        public WeiXinClient(
            IAccessTokenManager AccessTokenManager, 
            ISettingService<WeiXinMpSetting> Setting, 
            Lazy<ITimeService> TimeService,
            Lazy<IMediaManager> MediaManager
            )
        {
            this.MediaManager = MediaManager;
            this.AccessTokenManager = AccessTokenManager;
            this.Setting = Setting.Value;
            this.TimeService = TimeService;
        }
        public async Task<string> RequestString(string uri, HttpContent Content)
        {
            var access_token = await AccessTokenManager.GetAccessToken();
            var u = new Uri(new Uri(Setting.ApiUriBase), uri)
                .WithQueryString(("access_token", access_token));
            if (Content == null)
                return await u.GetString();
            else
                return await u.PostAndReturnString(Content);
        }

        public async Task<JsApiSignatureResult> JsApiSignature(string uri)
        {
            var jsapi_ticket = await AccessTokenManager.GetJsApiTicket();
            var noncestr = Strings.NumberAndLowerUpperChars.Random(16);
            var timestamp = TimeService.Value.Now.ToUnixTime().ToString();

            var str = $"jsapi_ticket={jsapi_ticket}&noncestr={noncestr}&timestamp={timestamp}&url={uri}";
            var hash= str.UTF8Bytes().Sha1().Hex();
            return new JsApiSignatureResult
            {
                appId = Setting.AppId,
                nonceStr = noncestr,
                signature = hash,
                timestamp = timestamp
            };
        }
        public async Task<string> SaveAndGetMediaId(string serverId)
        {
            var access_token = await AccessTokenManager.GetAccessToken();
            var u = new Uri(new Uri(Setting.ApiUriBase), "media/get")
                .WithQueryString(
                    ("access_token", access_token),
                    ("media_id",serverId)
                );
            var bytes = await u.GetBytes();
            var mm = new MediaMeta
            {
                Mime = "image/jpeg",
                Name = "",
                Type = "ms"
            };
            var mc = new SF.Sys.ByteArrayContent
            {
                Data = bytes
            };
            var re = await MediaManager.Value.SaveAsync(mm, mc);
            return re;
        }

    }
}
