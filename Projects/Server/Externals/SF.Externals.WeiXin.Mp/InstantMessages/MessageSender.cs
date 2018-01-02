using SF.Common.Notifications;
using SF.Sys;
using SF.Sys.Auth;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using SF.Sys.Threading;

namespace SF.Externals.WeiXin.Mp.InstantMessages
{
	class RequestTextContent
    {
        public string content { get; set; }
    }
    class Request
    {
        public string touser { get; set; }
        public string msgtype { get; set; }
        public RequestTextContent text { get; set; }
    }
    class Response
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public long msgid { get; set; }
    }
	/// <summary>
	/// 微信公共号用户消息服务
	/// </summary>
    public class MessageSender : IMsgProvider
	{
        public IWeiXinClient Client { get; }
		public IUserProfileService UserProfileService { get; }

		public MessageSender(IWeiXinClient Client, IUserProfileService UserProfileService)
        {
			this.UserProfileService = UserProfileService;

			this.Client = Client;
        }
        
		public async Task<string> Send(NotificationSendArgument Arg)
		{
			var req = new Request
			{
				touser = Arg.Target,
			};
			//switch (message.Type)
			//{
			//    case MessageType.Text:
			req.msgtype = "text";
			req.text = new RequestTextContent
			{
				content = Arg.Content
			};
			//        break;
			//    //case MessageType.Image:
			//    default:
			//        throw new NotSupportedException();

			//}
			var re = await TaskUtils.Retry(
				() => Client.Json(
				"message/custom/send",
				req
				));
			var resp = Json.Parse<Response>(re);
			if (resp.errcode != 0)
				throw new Exception(resp.errcode + ":" + resp.errmsg);
			return resp.msgid.ToString();
		}

		public async Task<string> TargetResolve(long TargetId)
		{
			var re = await UserProfileService.GetClaims(TargetId, new[] { PredefinedClaimTypes.WeiXinMPId },null);
			return re.FirstOrDefault()?.Value;

		}
	}
}
