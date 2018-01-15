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
	public class SessionMember : ObjectEntityBase
	{
		/// <summary>
		/// 成员图标
		/// </summary>
		public string Icon { get; set; }

		/// <summary>
		/// 会话
		/// </summary>
		[EntityIdent(typeof(Session), nameof(SessionName))]
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
		public long? OwnerId { get; set; }

		/// <summary>
		/// 用户
		/// </summary>
		[TableVisible]
		[Ignore]
		public string OwnerName { get; set; }

		/// <summary>
		/// 成员业务类型
		/// </summary>
		public int BizType { get; set; }


		/// <summary>
		/// 业务标识类型
		/// </summary>
		[MaxLength(100)]
		public string BizIdentType { get; set; }

		/// <summary>
		/// 业务标识
		/// </summary>
		public long? BizIdent { get; set; }

		/// <summary>
		/// 是否接收成员
		/// </summary>
		public bool? SessionAccepted { get; set; }

		/// <summary>
		/// 是否同意加入
		/// </summary>
		public bool? MemberAccepted { get; set; }

		/// <summary>
		/// 加入状态
		/// </summary>
		[ReadOnly(true)]
		[TableVisible]
		public SessionJoinState JoinState { get; set; }


		/// <summary>
		/// 最后活跃时间
		/// </summary>
		[ReadOnly(true)]
		[TableVisible]
		public DateTime LastActiveTime{get;set;}
		/// <summary>
		/// 最后访问时间
		/// </summary>
		[TableVisible]
		public DateTime? LastReadTime { get; set; }
		/// <summary>
		/// 消息数
		/// </summary>
		public int MessageCount { get; set; }
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
	}
}