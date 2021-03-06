﻿using SF.Common.Notifications.Senders;
using SF.Sys;
using SF.Sys.Auth;
using SF.Sys.Collections.Generic;
using SF.Sys.Linq;
using SF.Sys.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class MsgProvider : INotificationSendProvider
    {
        public TemplateMessageSetting Setting { get; }
        public IAccessTokenManager AccessTokenManager { get; }
		public IUserProfileService UserProfileService { get; }
		public MsgProvider(TemplateMessageSetting Setting, IAccessTokenManager AccessTokenManager, IUserProfileService UserProfileService)
        {
            this.Setting = Setting;
            this.AccessTokenManager= AccessTokenManager;
			this.UserProfileService = UserProfileService;
		}
        public async Task<string> Send(ISendArgument message)
        {
			if (Setting.Disabled)
				return "禁止发送";
			var args = message.GetArguments();
			var req = new Request
			{
				touser = message.Targets.Single(),
				template_id = message.Template,
				url = args.Get("@url"),
                data = args
						.Where(a=>!a.Key.StartsWith("@"))
						.Select(a=>new { Key = a.Key.Split2('.'), a.Value })
						.GroupBy(a=>a.Key.Item1)
						.ToDictionary(
							p => p.Key, 
							p => new DataItem {
								value = p.FirstOrDefault(i=>i.Key.Item2==null || i.Key.Item2=="text")?.Value,
								color= p.FirstOrDefault(i => i.Key.Item2 == "color")?.Value ?? Setting.TextColor,
							}
						)
            };
            var re = await TaskUtils.Retry(() => AccessTokenManager.Json(
               "message/template/send",
                req
                ));
            var resp = SF.Sys.Json.Parse<Response>(re);
            if (resp.errcode != 0)
                throw new ExternalServiceException(resp.errcode + ":" + resp.errmsg);
            return resp.msgid.ToString();
        }

		
		public async Task<IEnumerable<string>> TargetResolve(IEnumerable<long> TargetIds)
		{
			var re = await UserProfileService.GetClaims(TargetIds.First(), new[] { PredefinedClaimTypes.WeiXinMPId }, null);
			var id = re.FirstOrDefault();
			return id == null ? Enumerable.Empty<string>() : EnumerableEx.From(id.Value);
		}

		public Task<IEnumerable<string>> GroupResolve(IEnumerable<long> GroupIds)
		{
			throw new System.NotSupportedException();
		}
	}
}
