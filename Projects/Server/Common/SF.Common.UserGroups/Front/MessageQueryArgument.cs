using System;
using SF.Sys.Entities;

namespace SF.Common.Conversations.Front
{
	/// <summary>
	/// 会话消息查询
	/// </summary>
	public class MessageQueryArgument : QueryArgument
	{
		/// <summary>
		/// 会话ID
		/// </summary>
		public long SessionId { get; set; }

		/// <summary>
		/// 若设置，则返回指定ID之后的记录,用于获取新增消息
		/// </summary>
		public long? StartId { get; set; }
	}
}