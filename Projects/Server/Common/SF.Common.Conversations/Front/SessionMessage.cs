using System;
using SF.Sys.Entities;
using SF.Sys.Annotations;

namespace SF.Common.Conversations.Front
{
	/// <summary>
	/// 会话消息
	/// </summary>
	public class SessionMessage : IEntityWithId<long>
	{
		/// <summary>
		/// ID
		/// </summary>
		public long Id { get; set; }

		/// <summary>
		/// 会话ID
		/// </summary>
		public long SessionId { get; set; }

		/// <summary>
		/// 消息时间
		/// </summary>
		public DateTime Time { get; set; }

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

		/// <summary>
		/// 发信用户ID,系统消息没有发信人
		/// </summary>
		public long? UserId { get; set; }

		/// <summary>
		/// 发信成员ID,系统消息没有发信人
		/// </summary>
		public long? PosterId { get; set; }

		/// <summary>
		/// 发信人昵称
		/// </summary>
		public string PosterName { get; set; }

		/// <summary>
		/// 发信人图标
		/// </summary>
		public string PosterIcon { get; set; }
	}
}