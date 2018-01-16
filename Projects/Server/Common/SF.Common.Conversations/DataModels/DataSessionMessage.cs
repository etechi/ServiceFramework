using System;
using SF.Sys.Entities;
using SF.Sys.Annotations;
using SF.Sys.Entities.DataModels;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Sys.Data;
using System.ComponentModel.DataAnnotations;

namespace SF.Common.Conversations.DataModels
{
	/// <summary>
	/// 会话消息
	/// </summary>
	[Table("SessionMessage")]
	public class DataSessionMessage : DataEventEntityBase
	{
		[Index("session", Order = 2)]
		public override long Id { get; set; }
		/// <summary>
		/// 会话
		/// </summary>
		[Index("session",Order =1)]
		public long SessionId { get; set; }

		[ForeignKey(nameof(SessionId))]
		public DataSessionStatus Session { get; set; } 

		/// <summary>
		/// 消息类型
		/// </summary>
		public MessageType Type { get; set; }

		/// <summary>
		/// 消息内容
		/// </summary>
		[MaxLength(1000)]
		public string Text { get; set; }

		/// <summary>
		/// 消息参数
		/// </summary>
		[MaxLength(1000)]
		public string Argument { get; set; }

		/// <summary>
		/// 发信成员ID
		/// </summary>
		[Index]
		public long? PosterId { get; set; }

		[ForeignKey(nameof(PosterId))]
		public DataSessionMemberStatus Poster { get; set; }

	}
}