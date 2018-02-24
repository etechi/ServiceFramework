using SF.Sys.Entities;
using SF.Sys.Entities.Models;
using SF.Sys.Annotations;
using SF.Sys.Auth;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Common.Conversations.Models
{
	/// <summary>
	/// 会话成员
	/// </summary>
	public class SessionMemberStatus : ObjectEntityBase
	{

		/// <summary>
		/// 会话
		/// </summary>
		[EntityIdent(typeof(SessionStatus), nameof(SessionName))]
		[Uneditable]
		public long SessionId { get; set; }

		/// <summary>
		/// 会话
		/// </summary>
		[TableVisible]
		[Ignore]
		public string SessionName { get; set; }

		/// <summary>
		/// 用户
		/// </summary>
		[EntityIdent(typeof(User), nameof(OwnerName))]
		[Uneditable]
		public long? OwnerId { get; set; }

		/// <summary>
		/// 用户
		/// </summary>
		[TableVisible]
		[Ignore]
		public string OwnerName { get; set; }

		/// <summary>
		/// 发送消息
		/// </summary>
		[ReadOnly(true)]
		public int MessageCount { get; set; }

		/// <summary>
		/// 已读消息
		/// </summary>
		[ReadOnly(true)]
		public int MessageReaded { get; set; }

		/// <summary>
		/// 最后阅读时间
		/// </summary>
		public DateTime? LastReadTime { get; set; }

		/// <summary>
		/// 最后活跃时间
		/// </summary>
		[ReadOnly(true)]
		[TableVisible]
		public DateTime LastMessageTime{get;set;}


		/// <summary>
		/// 最后消息
		/// </summary>
		[ReadOnly(true)]
		[EntityIdent(typeof(SessionMessage), nameof(LastMessageName))]
		public long? LastMessageId { get; set; }

		/// <summary>
		/// 最后消息
		/// </summary>
		[TableVisible]
		[Ignore]
		public string LastMessageName { get; set; }

		/// <summary>
		/// 连续消息分段数
		/// </summary>
		public int MessageSectionCount { get; set; }

		/// <summary>
		/// 连续消息分段时间
		/// </summary>
		public int MessageSectionTotalTime { get; set; }
	}
}