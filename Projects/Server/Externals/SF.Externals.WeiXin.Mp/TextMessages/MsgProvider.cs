using SF.Common.TextMessages;
using SF.Sys;
using SF.Sys.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Externals.WeiXin.Mp.TextMessages
{
	class DataItem
    {
        public string value { get; set; }
        public string color { get; set; }
    }
    class Request
    {
        public string touser { get; set; }                     
        public string template_id { get; set; }
        public string url { get; set; }
        public Dictionary<string,DataItem> data { get; set; }
    }
    class Response
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public long msgid { get; set; }
    }
	/// <summary>
	/// 微信公共号模板消息服务
	/// </summary>
    public class MsgProvider : IMsgProvider
    {
        public TemplateMessageSetting Setting { get; }
        public IWeiXinClient Client { get; }
		public IUserProfileService UserProfileService { get; }
		public MsgProvider(TemplateMessageSetting Setting, IWeiXinClient Client, IUserProfileService UserProfileService)
        {
            this.Setting = Setting;
            this.Client = Client;
			this.UserProfileService = UserProfileService;
		}
        public async Task<string> Send(MsgSendArgument message)
        {
			if (Setting.Disabled)
				return "禁止发送";

			var req = new Request
			{
				touser = message.Target,
				template_id = message.Template,
				url = message.Arguments.FirstOrDefault(p => p.Key == "MobileLink").Value,
                data = message.Arguments.ToDictionary(
                    p => p.Key, 
                    p => new DataItem { value = p.Value }
                    )
            };
            var re = await TaskUtils.Retry(() => Client.Json(
                "message/template/send",
                req
                ));
            var resp = SF.Sys.Json.Parse<Response>(re);
            if (resp.errcode != 0)
                throw new ExternalServiceException(resp.errcode + ":" + resp.errmsg);
            return resp.msgid.ToString();
        }

		public async Task<string> TargetResolve(long TargetId)
		{
			var re = await UserProfileService.GetClaims(TargetId, new[] { PredefinedClaimTypes.WeiXinMPId }, null);
			return re.FirstOrDefault()?.Value;
		}
	}
}
