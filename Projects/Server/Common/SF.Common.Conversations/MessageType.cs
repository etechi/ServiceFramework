namespace SF.Common.Conversations
{
	/// <summary>
	/// 消息类型
	/// </summary>
	public enum MessageType
	{
		/// <title>文本</title>
		/// <summary>
		///  Name为文本内容
		/// </summary>
		Text,

		/// <title>语音</title>
		/// <summary>
		/// Argument为语音ID
		/// </summary>
		Voice,
		/// <title>图片</title>
		/// <summary>
		/// Argument为图片ID
		/// </summary>
		Image,
		/// <summary>
		/// 系统消息
		/// </summary>
		System,

	}
}