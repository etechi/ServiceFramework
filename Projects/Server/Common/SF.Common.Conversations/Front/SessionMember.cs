namespace SF.Common.Conversations.Front
{
	/// <summary>
	/// 会话成员
	/// </summary>
	public class SessionMember
	{
		/// <summary>
		/// 业务标识类型
		/// </summary>
		public string BizIdentType { get; set; }

		/// <summary>
		/// 业务标识
		/// </summary>
		public long BizIdent { get; set; }

		/// <summary>
		/// 用户ID
		/// </summary>
		public long UserId { get; set; }
		/// <summary>
		/// 名称
		/// </summary>
		public string Name { get; set; }
		
		/// <summary>
		/// 图标
		/// </summary>
		public string Icon { get; set; }

		/// <summary>
		/// 排序
		/// </summary>
		public int Order { get; set; }

	}
}