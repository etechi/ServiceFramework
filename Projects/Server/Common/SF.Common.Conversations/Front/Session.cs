using System;
using SF.Sys.Entities;
using SF.Sys.Annotations;

namespace SF.Common.Conversations.Front
{

	/// <summary>
	/// 会话
	/// </summary>
	public class Session
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
		/// 图标
		/// </summary>
		public string Icon { get; set; }
		/// <summary>
		/// 名称
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 文本
		/// </summary>
		public string Text { get; set; }
		/// <summary>
		/// 时间
		/// </summary>
		public DateTime? Time { get; set; }
		/// <summary>
		/// 未读
		/// </summary>
		public int Unread { get; set; }
	}
}