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
		/// 会话
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
		/// 成员图标
		/// </summary>
		public string Icon { get; set; }

		/// <summary>
		/// 成员最后活跃时间
		/// </summary>
		[Index("session", Order = 2)]
		public DateTime LastActiveTime { get; set; }
		
		/// <summary>
		/// 最后消息
		/// </summary>
		[Index]
		public long? LastMessageId { get; set; }

		[ForeignKey(nameof(LastMessageId))]
		public DataSessionMessage LastMessage { get; set; }

		/// <summary>
		/// 当前成员发送消息数
		/// </summary>
		public int MessageCount { get; set; }

		/// <summary>
		/// 当前成员已读消息数
		/// </summary>
		public int MessageReaded { get; set; }

		/// <summary>
		/// 会话所有者是否同意加入
		/// </summary>
		public bool? SessionAccepted { get; set; }

		/// <summary>
		/// 用户是否同意加入
		/// </summary>
		public bool? MemberAccepted { get; set; }

		/// <summary>
		/// 加入状态
		/// </summary>
		public SessionJoinState JoinState { get; set; }

		
		/// <summary>
		/// 最后读取时间
		/// </summary>
		[TableVisible]
		public DateTime? LastReadTime { get; set; }
	}
}