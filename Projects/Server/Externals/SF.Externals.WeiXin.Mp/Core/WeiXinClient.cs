using SF.Common.Media;
using SF.Sys;
using SF.Sys.HttpClients;
using SF.Sys.Mime;
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
		IHttpClient HttpClient { get; }
        Lazy<IMimeResolver> MimeResolver { get; }
        public WeiXinClient(
            IAccessTokenManager AccessTokenManager, 
            ISettingService<WeiXinMpSetting> Setting, 
            Lazy<ITimeService> TimeService,
            Lazy<IMediaManager> MediaManager,
            Lazy<IMimeResolver> MimeResolver,
            IHttpClient HttpClient
			)
        {
            this.MediaManager = MediaManager;
            this.AccessTokenManager = AccessTokenManager;
            this.Setting = Setting.Value;
            this.TimeService = TimeService;
			this.HttpClient = HttpClient;
            this.MimeResolver = MimeResolver;


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
            var bytes = await HttpClient.From(u).GetBytes();
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


        class upload_result
        {
            public string type { get; set; }
            public string media_id { get; set; }
            public long created_at { get; set; }
            public string errmsg { get; set; }
            public int errcode { get; set; }
        }
        public async Task<string> GetTempMediaId(string id)
        {
            var meta = await MediaManager.Value.ResolveAsync(id);
            if (meta == null)
                throw new PublicArgumentException("找不到资源:" + id);
            string type;
            switch (meta.Mime)
            {
                case "image/png":
                case "image/jpeg":
                case "image/jpg":
                case "image/gif":
                    type = "image";
                    if (meta.Length > 2 * 1024 * 1024)
                        throw new PublicArgumentException("微信最大支持2M图片");
                    break;
                case "audio/amr":
                case "audio/mp3":
                    type = "audio";
                    if (meta.Length > 2 * 1024 * 1024)
                        throw new PublicArgumentException("微信语言最大支持2M图片");
                    break;
                default:
                    throw new PublicArgumentException("微信不支持此文件类型:" + meta.Mime);
            }

            var ctn = await MediaManager.Value.GetContentAsync(meta);
            var bytes = await ctn.GetByteArrayAsync();
            //var access_token = "14_AnPbX43WsR5n8nAlE5uPBLccM27edsbTG3ixowav-uXpdJlAyDXvR-YJPbCaOxXpemM6FhiznQ6LYBB1aiuBHeu5BWQiMeTMQJhYoMS1oYtfiTX1lyahAkU5dEWzuLaFujkeAE6l5_uuqD4YYXXcAGAJII";//
            var access_token = await AccessTokenManager.GetAccessToken();
            var apiuribase = Setting.ApiUriBase;
            //apiuribase = apiuribase.Replace("https:", "http:");
            var u = new Uri(new Uri(apiuribase), "media/upload")
                .WithQueryString(
                    ("access_token", access_token),
                    ("type", type)
                );

            //var contents = new[] {
            //    new System.Net.Http.ByteArrayContent(bytes).WithHeaders(file:meta.Name,name:"media",mime:meta.Mime,disposition:"form-data"),
            //    new System.Net.Http.StringContent(meta.Name,System.Text.Encoding.UTF8,"text/plain").WithHeaders(name:"filename",disposition:"form-data"),
            //    new System.Net.Http.StringContent(bytes.Length.ToString(),System.Text.Encoding.UTF8,"text/plain").WithHeaders(name:"filelength",disposition:"form-data"),
            //    new System.Net.Http.StringContent(meta.Mime,System.Text.Encoding.UTF8,"text/plain").WithHeaders(name:"content-type",disposition:"form-data"),
            //};

            //ContentType: multipart / form - data; boundary = ------------------------0a75d91e4a10b2fe

            ///！！！！！！！！！！！！！！！！！！！！！！！！！
            ///微信服务器和.net core原生multipart/form-data协议不兼容

            var filename = meta.Name;
            if (filename.IndexOf('.')==-1)
                filename += "." + MimeResolver.Value.MimeToFileExtension(meta.Mime);

            var head = System.Text.Encoding.UTF8.GetBytes("--------------------------0a75d91e4a10b2fe\r\n" +
                    $"Content-Disposition: form-data; name=\"media\"; filename=\"{filename}\"\r\n" +
                    $"Content-Type: {meta.Mime}\r\n\r\n");
            var tail =System.Text.Encoding.UTF8.GetBytes("\r\n--------------------------0a75d91e4a10b2fe--\r\n");
            var buf = head.Concat(bytes, tail);
            var nctn = new System.Net.Http.ByteArrayContent(buf);
            nctn.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("multipart/form-data");
            nctn.Headers.ContentType.Parameters.Add(new System.Net.Http.Headers.NameValueHeaderValue("boundary", "------------------------0a75d91e4a10b2fe"));


            //var mfdc = new MultipartFormDataContent("--------------------------0a75d91e4a10b2fe");
            //var fctn = new System.Net.Http.ByteArrayContent(bytes);
            //fctn.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            //mfdc.Add(fctn, "media", meta.Name+".png");
            //fctn.Headers.ContentDisposition.Size = bytes.Length;
           
            var restr=await HttpClient.From(u).WithContent(nctn).GetString();
            if (restr.IsNullOrEmpty())
                throw new PublicInvalidOperationException("上传至微信失败，没有数据返回");

            var re = Json.Parse<upload_result>(restr);
            if(re.errcode>0)
                throw new PublicInvalidOperationException($"微信上传失败:{restr}");
            return re.media_id;
        }
    }
}
