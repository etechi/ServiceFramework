using SF.Sys.Entities;
using SF.Sys.Entities.DataModels;
using SF.Sys.Annotations;
using SF.Sys.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel.DataAnnotations;

namespace SF.Common.Conversations.DataModels
{
	/// <summary>
	/// 会话成员
	/// </summary>
	[Table("SessionMember")]
	public class DataSessionMember : DataObjectEntityBase
	{
		/// <summary>
		/// 会话状态
		/// </summary>
		[Index("member",IsUnique =true,Order =1)]
		[Index("session", Order = 1)]
		public long SessionId { get; set; }

		[ForeignKey(nameof(SessionId))]
		public DataSession Session { get; set; }

		/// <summary>
		/// 用户
		/// </summary>
		[Index]
		[Index("member", IsUnique = true, Order = 2)]
		public override long? OwnerId { get; set; }

		/// <summary>
		/// 成员最后活跃时间
		/// </summary>
		[Index("session", Order = 2)]
		public DateTime LastMessageTime { get; set; }
		
		/// <summary>
		/// 最后消息
		/// </summary>
		[Index]
		public long? LastMessageId { get; set; }

		[ForeignKey(nameof(LastMessageId))]
		public DataSessionMessage LastMessage { get; set; }

		/// <summary>
		/// 发送消息
		/// </summary>
		public int MessageCount { get; set; }

		/// <summary>
		/// 已读消息
		/// </summary>
		public int MessageReaded { get; set; }

		/// <summary>
		/// 最后阅读时间
		/// </summary>
		public DateTime? LastReadTime { get; set; }
	}
}