using SF.Sys;
using SF.Sys.HttpClients;
using SF.Sys.Linq;
using SF.Sys.TimeServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SF.Externals.Aliyun.Implements
{
	public class AliyunInvoker : IAliyunInvoker
	{
		AliyunSetting Setting { get; }
		IHttpClient HttpClient { get; }
		ITimeService TimeService { get; }
		public AliyunInvoker(AliyunSetting Setting, IHttpClient HttpClient, ITimeService TimeService)
		{
			this.TimeService = TimeService;
			this.Setting = Setting;
			this.HttpClient = HttpClient;
		}
		class ErrorInfo
		{
			public string RequestId { get; set; }
			public string HostId { get; set; }
			public string Code { get; set; }
			public string Message { get; set; }
		}
		
		static string HMACSHA1Hash(string Password,string Data)
		{
			using (var hmacsha1 = new HMACSHA1(Password.UTF8Bytes()))
			{
				return hmacsha1.ComputeHash(Data.UTF8Bytes()).Base64();
			}
		}
		public async Task<string> Invoke(
			string Method,
			string BaseUri,
			(string Key, string Value)[] ApiSettings,
			params (string Key, string Value)[] Arguments
			)
		{
			var args = new List<(string Key, string Value)>();
			args.AddRange(ApiSettings.Select(p => (p.Key, Uri.EscapeDataString(p.Value))));
			args.AddRange(Arguments.Select(p=>(p.Key,Uri.EscapeDataString(p.Value))));
			args.Add(("Format", "JSON"));
			args.Add(("AccessKeyId", Uri.EscapeDataString(Setting.AppKey)));
			args.Add(("SignatureMethod", "HMAC-SHA1"));
			args.Add(("Timestamp", Uri.EscapeDataString(TimeService.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"))));
			args.Add(("SignatureVersion", Uri.EscapeDataString("1.0")));
			args.Add(("SignatureNonce", Strings.NumberAndLowerUpperChars.Random(6)));

			args.Sort((a, b) => a.Key.CompareTo(b.Key));
			var content = args.Select(p => p.Key + "=" + p.Value).Join("&");

			var StringToSign =
				Method +
				"&%2F&" +
				Uri.EscapeDataString(content)
					.Replace("+", "%20")
					.Replace("*", "%2A")
					.Replace("%7E", "~")
				;

			var Signature = HMACSHA1Hash(Setting.AppSecret+"&", StringToSign);
			content += "&Signature=" + Uri.EscapeDataString(Signature);

			if (Method == "GET")
				BaseUri = BaseUri += "?" + content;

			var req = HttpClient.From(BaseUri);

			if (Method == "POST")
				req = req.WithContent(content);
			req.EnsureSuccess(false);
			var success = false;
			req.OnReturn(m => success = m.StatusCode == System.Net.HttpStatusCode.OK);

			var re = await req.GetString();
			if (success)
				return re;

			var err = Json.Parse<ErrorInfo>(re);
			throw new AliyunException(err.Code, err.Message, err.RequestId, err.HostId);
		}
	}
}
