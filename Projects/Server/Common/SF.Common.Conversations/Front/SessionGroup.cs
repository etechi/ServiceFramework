using System.Collections.Generic;

namespace SF.Common.Conversations.Front
{
	/// <summary>
	/// 会话分组
	/// </summary>
	public class SessionGroup
	{
		/// <summary>
		/// 名称
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 文本
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// 会话
		/// </summary>
		public IEnumerable<Session> Sessions { get; set; }
		
		/// <summary>
		/// 排序
		/// </summary>
		public int Order { get; set; }
	}
}