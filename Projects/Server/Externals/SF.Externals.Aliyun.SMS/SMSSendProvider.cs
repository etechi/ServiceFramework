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

namespace SF.Externals.Aliyun.SMS
{
	public class AliyunSMSException : AliyunException
	{
		public AliyunSMSException(string Code, string Message, string RequestId, string HostId) :
			base(Code, Message, RequestId, HostId)
		{
		}
	}
	public class AliyunSMSSetting
	{

		/// <summary>
		/// 服务器地址
		/// </summary>
		public string Uri { get; set; } = "https://dysmsapi.aliyuncs.com/";

		/// <summary>
		/// 版本
		/// </summary>
		public string Version { get; set; } = "2017-05-25";

		/// <summary>
		/// 地区
		/// </summary>
		public string RegionId { get; set; } = "cn-shanghai";

		/// <summary>
		/// 短信签名
		/// </summary>
		public string SignName { get; set; }
	}

	public class SMSSendProvider : INotificationSendProvider
	{
		AliyunSMSSetting Setting { get; }
		IAliyunInvoker AliyunInvoker { get; }
		IUserProfileService UserProfileService { get; }
		public SMSSendProvider(
			IAliyunInvoker AliyunInvoker,
			AliyunSMSSetting Setting,
			IUserProfileService UserProfileService
			)
		{
			this.AliyunInvoker = AliyunInvoker;
			this.Setting = Setting;
			this.UserProfileService = UserProfileService;
		}

		class Response
		{
			public string Code { get; set; }
			public string BizId { get; set; }
			public string RequestId { get; set; }
			public string Message { get; set; }
		}

		public async Task<string> Send(ISendArgument message)
		{
			var re = await AliyunInvoker.Invoke(
				"GET",
				Setting.Uri,
				new[]{
					("Action", "SendSms"),
					("Version",Setting.Version),
					("SignName",Setting.SignName),
					("RegionId",Setting.RegionId),
				},
				("PhoneNumbers", message.Targets.Single()),
				("TemplateCode", message.Template),
				("TemplateParam", Json.Stringify(message.GetArguments())),
				("OutId", message.Id.ToString())
				);
			var resp = Json.Parse<Response>(re);
			if (resp.Code == "OK")
				return resp.BizId;
			throw new AliyunSMSException(
				resp.Code,
				"阿里云短信发送失败:" + resp.Message,
				resp.RequestId,
				null
				);
			//{"Message":"OK","RequestId":"F15AA5BC-2FCA-4BC0-94BC-5B00173EE195","BizId":"233012518522020479^0","Code":"OK"}
		}

		public async Task<IEnumerable<string>> TargetResolve(IEnumerable<long> TargetIds)
		{
			var claims = new[] { SF.Sys.Auth.PredefinedClaimTypes.Phone };
			var list = new List<string>();
			foreach (var target in TargetIds)
			{
				var re = await UserProfileService.GetClaims(
					target,
					claims,
					null
					);
				list.Add(re?.FirstOrDefault()?.Value);
			}
			return list;
		}

		public Task<IEnumerable<string>> GroupResolve(IEnumerable<long> GroupIds)
		{
			throw new NotSupportedException();
		}

	}
}
