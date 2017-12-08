namespace SF.Externals.WeiXin.Mp.TextMessages
{
	public class TemplateMessageSetting 
	{
		///<title>禁止发送</title>
		/// <summary>
		/// 禁止发送消息。生成消息日志，但不进行实际发送，主要用于测试
		/// </summary>
        public bool Disabled { get; set; }
    }
}
