using SF.Common.Notifications.Senders;
using SF.Sys;
using SF.Sys.Auth;
using SF.Sys.Collections.Generic;
using SF.Sys.HttpClients;
using SF.Sys.Linq;
using SF.Sys.NetworkService;
using SF.Sys.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SF.Externals.JPush
{
	public class JPushSetting
	{
		/// <summary>
		/// AppKey
		/// </summary>
		public string AppKey { get; set; }

		/// <summary>
		/// AppSecret
		/// </summary>
		public string AppSecret { get; set; }

		/// <summary>
		/// 服务器地址
		/// </summary>
		public string Uri { get; set; } 

		/// <summary>
		/// 别名前缀
		/// </summary>
		public string AliasPrefix { get; set; }

		/// <summary>
		/// 标签前缀
		/// </summary>
		public string TagPrefix { get; set; }
	}

	/// <summary>
	/// JPush模板消息服务
	/// </summary>
	public class NotificationSendProvider : INotificationSendProvider, Sys.NetworkService.IClientSettingProvider
	{
		JPushSetting Setting { get; }
		IHttpClient HttpClient { get; }

		string IClientSettingProvider.Name => "jpush";

		public NotificationSendProvider(JPushSetting Setting, IHttpClient HttpClient)
		{
			this.Setting = Setting;
			this.HttpClient = HttpClient;
		}
		class Error
		{
			public string message { get; set; }
			public int code { get; set; }
		}
		class Response
		{
			public Error error { get; set; }
			public string sendno { get; set; }
			public string msg_id { get; set; }
		}
		public async Task<string> Send(ISendArgument message)
		{
			var tag = message.Groups?.ToArray();
			var req = new
			{
				//cid = message.Id.ToString(),
				platform = "all",
				audience = new
				{
					alias = message.Targets,
					tag = (tag?.Length ?? 0) == 0 ? null : tag,
				},
				notification = new
				{
					alert = message.Title
				},
				options=new {
					apns_production= true
				}
			};
			var reqstr = Json.Stringify(req);
			var Authorization = "Basic " + (Setting.AppKey + ":" + Setting.AppSecret).UTF8Bytes().Base64();

			var re = await HttpClient
					.From(Setting.Uri)
					.WithContent(reqstr)
					.WithHeader("Authorization", Authorization)
					.EnsureSuccess(false)
					.GetString();
			var resp = SF.Sys.Json.Parse<Response>(re);
			//忽略找不到用户的错误
			if (resp.error != null && resp.error.code!=1011) 
				throw new ExternalServiceException("JPush调用失败:" + re+" 请求:"+reqstr);
			return resp.msg_id;
		}

		public Task<IEnumerable<string>> TargetResolve(IEnumerable<long> TargetIds)
		{
			return Task.FromResult(TargetIds.Select(i => Setting.AliasPrefix+ i.ToString()));
		}

		public Task<IEnumerable<string>> GroupResolve(IEnumerable<long> GroupIds)
		{
			return Task.FromResult(GroupIds.Select(i => Setting.TagPrefix+i.ToString()));
		}

		Task<object> IClientSettingProvider.GetOption(string ClientId)
		{
			return Task.FromResult((object)new
			{
				

			});
		}
	}
}
