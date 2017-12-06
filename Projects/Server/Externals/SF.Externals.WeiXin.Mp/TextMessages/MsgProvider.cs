using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.TextMessages;
using ServiceProtocol.Annotations;
using System.Net.Http;

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
    [TypeDisplay(Name = "微信模板消息服务")]
    [ServiceProvider(typeof(IMsgProvider), typeof(TemplateMessageSetting))]
    public class MsgProvider : ServiceProtocol.TextMessages.IMsgProvider
    {
        public TemplateMessageSetting Setting { get; }
        public IWeiXinClient Client { get; }

        public MsgProvider(TemplateMessageSetting Setting, IWeiXinClient Client)
        {
            this.Setting = Setting;
            this.Client = Client;
        }
        public async Task<string> Send(string target, Message message)
        {
            if (Setting.Disabled)
                return "禁止发送";

            var ere = Setting.Templates.EvalTemplate(message.Body, message.Arguments);
            if (ere == null)
                return "找不到微信模板";

            var req = new Request
            {
                touser = target,
                template_id = ere.TemplateId,
                url = ere.Arguments.Get("MobileLink"),
                data = ere.Arguments.ToDictionary(
                    p => p.Key, 
                    p => new DataItem { value = p.Value }
                    )
            };
            var re = await Functional.Retry(() => Client.Json(
                "message/template/send",
                req
                ));
            var resp = Json.Decode<Response>(re);
            if (resp.errcode != 0)
                throw new Exception(resp.errcode + ":" + resp.errmsg);
            return resp.msgid.ToString();
        }
    }
}
