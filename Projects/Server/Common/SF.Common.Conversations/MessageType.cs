namespace SF.Common.Conversations
{
	/// <summary>
	/// 消息类型
	/// </summary>
	public enum MessageType
	{
		/// <summary>
		/// 文本, Name为文本内容
		/// </summary>
		Text,
		/// <summary>
		/// 语音，Argument为语音ID
		/// </summary>
		Voice,
		/// <summary>
		/// 图片，Argument为图片ID
		/// </summary>
		Image,
		/// <summary>
		/// 系统消息
		/// </summary>
		System,

	}
}