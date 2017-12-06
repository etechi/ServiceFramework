using ServiceProtocol.Biz.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SF.Externals.WeiXin.Mp.Core
{

    public class WeiXinClient : IWeiXinClient
    {
        IAccessTokenManager AccessTokenManager { get; }
        WeiXinMpSetting Setting { get; }
        Lazy<Times.ITimeService> TimeService { get; }
        Lazy<IMediaManager> MediaManager { get; }
        public WeiXinClient(
            IAccessTokenManager AccessTokenManager, 
            WeiXinMpSetting Setting, 
            Lazy<Times.ITimeService> TimeService,
            Lazy<IMediaManager> MediaManager
            )
        {
            this.MediaManager = MediaManager;
            this.AccessTokenManager = AccessTokenManager;
            this.Setting = Setting;
            this.TimeService = TimeService;
        }
        public async Task<string> RequestString(string uri, HttpContent Content)
        {
            var access_token = await AccessTokenManager.GetAccessToken();
            var u = new Uri(new Uri(Setting.ApiUriBase), uri)
                .WithQueryString(Tuple.Create("access_token", access_token));
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
        public async Task<string> GetMediaId(string serverId)
        {
            var access_token = await AccessTokenManager.GetAccessToken();
            var u = new Uri(new Uri(Setting.ApiUriBase), "media/get")
                .WithQueryString(
                    Tuple.Create("access_token", access_token),
                    Tuple.Create("media_id",serverId)
                );
            var bytes = await u.GetBytes();
            var mm = new MediaMeta
            {
                Mime = "image/jpeg",
                Name = "",
                Type = "ms"
            };
            var mc = new ByteArrayMediaContent
            {
                Data = bytes
            };
            var re = await MediaManager.Value.SaveAsync(mm, mc);
            return re;
        }

    }
}
