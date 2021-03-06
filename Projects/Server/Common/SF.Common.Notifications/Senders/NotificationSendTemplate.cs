﻿using SF.Sys;
using SF.Sys.Annotations;
using System.Collections.Generic;
using System.Linq;

namespace SF.Common.Notifications.Senders
{
	public class NotificationTemplate
	{
		/// <summary>
		/// 模板名称
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 模板内容
		/// </summary>
		public string Content { get; set; }
	}

    public class NotificationTemplateArgument
	{
		/// <summary>
		/// 参数名称
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 参数内容
		/// </summary>
		public string Value { get; set; }
    }
    public class NotificationTemplateEx
	{
		/// <summary>
		/// 匹配内容
		/// </summary>
		[EntityTitle]
        public string Name { get; set; }

		/// <summary>
		/// 外部模板ID
		/// </summary>
		public string TemplateId { get; set; }

		/// <summary>
		/// 模板参数
		/// </summary>
		[TableRows]
        public NotificationTemplateArgument[] Arguments { get; set; }
    }
    public class MsgEvalResult
    {
        public string TemplateId { get; set; }
        public Dictionary<string,string> Arguments { get; set; }
    }
    public static class MsgTemplateExtension
    {
        public static string EvalTemplate(this NotificationTemplate[] Templates,string Content,IReadOnlyDictionary<string,object> Args)
        {
            if (Templates == null || Templates.Length == 0)
                return Content;
            foreach (var t in Templates)
                if (t.Name == Content)
                    return t.Content.Replace(Args);
            return Content;
        }
   
        public static MsgEvalResult EvalTemplate(this NotificationTemplateEx[] Templates, string Content, IReadOnlyDictionary<string, object> Args)
        {
            if (Templates == null || Templates.Length == 0)
                return null;
            foreach (var t in Templates)
                if (t.Name == Content)
                    return new MsgEvalResult
                    {
                        TemplateId=t.TemplateId,
                        Arguments = t.Arguments
                        .GroupBy(p => p.Name)
                        .ToDictionary(
                            g => g.Key,
                            g => g.First().Value.Replace( Args)
                            )
                    };
            return null;
        }
    }
}
