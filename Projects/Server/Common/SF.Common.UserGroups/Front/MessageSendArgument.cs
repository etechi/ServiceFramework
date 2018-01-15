namespace SF.Common.Conversations.Front
{
	/// <summary>
	/// 会话消息发送参数
	/// </summary>
	public class MessageSendArgument
	{
		/// <summary>
		/// 会话ID
		/// </summary>
		public long SessionId { get; set; }

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