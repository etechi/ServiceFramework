using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using ServiceProtocol.Annotations;
using ServiceProtocol.TextMessages;

namespace SF.Externals.WeiXin.Mp.TextMessages
{
    public class TemplateMessageSetting 
	{
        [Display(Name = "短信模板", Description = "当发送内容与模板名称一致时，使用模板替换结果作为短信发送内容。")]
        [TreeNodes]
        public MsgTemplateEx[] Templates { get;  set; }


        [Display(Name = "禁止发送", Description = "禁止发送消息。生成消息日志，但不进行实际发送，主要用于测试")]
        public bool Disabled { get; set; }
    }
}
