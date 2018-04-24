namespace SF.Common.Conversations.Front
{
	/// <summary>
	/// 会话消息发送参数
	/// </summary>
	public class MessageSendArgument
	{
		/// <summary>
		/// 会话业务标识类型
		/// </summary>
		public string BizIdentType { get; set; }

		/// <summary>
		/// 会话业务标识
		/// </summary>
		public long BizIdent { get; set; }

		/// <summary>
		/// 消息类型
		/// </summary>
		public MessageType Type { get; set; }

		/// <summary>
		/// 消息内容
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// 消息参数
		/// </summary>
		public string Argument { get; set; }
	}
}