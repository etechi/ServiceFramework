using SF.Common.TextMessages;
using SF.Sys;
using System;
using System.Threading;
using System.Threading.Tasks;
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
    public class MessageSender : ITextMessageService
	{
        public IWeiXinClient Client { get; }

        public MessageSender(IWeiXinClient Client)
        {
            this.Client = Client;
        }
        
		public Task<long> Send(long? targetId, string target, Message message)
		{
			var req = new Request
			{
				touser = target,
			};
			//switch (message.Type)
			//{
			//    case MessageType.Text:
			req.msgtype = "text";
			req.text = new RequestTextContent
			{
				content = message.Body
			};
			//        break;
			//    //case MessageType.Image:
			//    default:
			//        throw new NotSupportedException();

			//}
			var re = await TaskUtils.Retry(() => Client.Json(
				"message/custom/send",
				req
				));
			var resp = Json.Parse<Response>(re);
			if (resp.errcode != 0)
				throw new Exception(resp.errcode + ":" + resp.errmsg);
			return resp.msgid.ToString();
		}
	}
}
