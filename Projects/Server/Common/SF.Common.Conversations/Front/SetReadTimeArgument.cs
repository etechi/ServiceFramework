namespace SF.Common.Conversations.Front
{
	/// <summary>
	/// 设置会话消息浏览时间参数
	/// </summary>
	public class SetReadTimeArgument
	{
		/// <summary>
		/// 业务标识类型
		/// </summary>
		public string BizIdentType { get; set; }

		/// <summary>
		/// 会话业务标识
		/// </summary>
		public long BizIdent { get; set; }

	}
}