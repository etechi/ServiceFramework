using SF.Sys.Entities;
using SF.Sys.Annotations;
using System;

namespace SF.Common.Conversations.Front
{
	/// <summary>
	/// 会话
	/// </summary>
	public class Session:IEntityWithId<long>
	{
		/// <summary>
		/// ID
		/// </summary>
		public long Id { get; set; }
		
	

		/// <summary>
		/// 会话名称
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 会话图标
		/// </summary>
		public string Icon { get; set; }
		/// <summary>
		/// 会话标志
		/// </summary>
		public SessionFlag Flags { get; set; }

		/// <summary>
		/// 最后活动时间
		/// </summary>
		public DateTime LastActiveTime { get; set; }
		
		/// <summary>
		/// 最后消息
		/// </summary>
		public SessionMessage LastMessage { get; set; }


		/// <summary>
		/// 成员业务类型
		/// </summary>
		public int MemberBizType { get; set; }
		
		/// <summary>
		/// 成员业务标识类型
		/// </summary>
		public string MemberBizIdentType { get; set; }

		/// <summary>
		/// 成员业务标识
		/// </summary>
		public long? MemberBizIdent { get; set; }

		/// <summary>
		/// 会话业务标识类型
		/// </summary>
		public string SessionBizIdentType { get; set; }

		/// <summary>
		/// 会话业务标识
		/// </summary>
		public long? SessionBizIdent { get; set; }


		/// <summary>
		/// 成员数量
		/// </summary>
		public int MemberCount { get; set; }
		
		/// <summary>
		/// 消息数
		/// </summary>
		public int MessageCount { get; set; }

		/// <summary>
		/// 未读消息数
		/// </summary>
		public int MessageUnreaded { get; set; }
		/// <summary>
		/// 最后访问时间
		/// </summary>
		public DateTime? LastReadTime { get; set; }


		/// <summary>
		/// 加入状态
		/// </summary>
		public SessionJoinState JoinState { get; set; }

		

		/// <summary>
		/// </summary>
		[EntityIdent(typeof(SessionMember))]
		public long? OwnerMemberId { get; set; }

		/// <summary>
		/// 所有人成员名称
		/// </summary>
		[FromEntityProperty(nameof(OwnerMemberId), nameof(SessionMember.Name))]
		public string OwnerMemberName { get; set; }

		/// <summary>
		/// 所有人成员图标
		/// </summary>
		[FromEntityProperty(nameof(OwnerMemberId), nameof(SessionMember.Icon))]
		public string OwnerMemberIcon { get; set; }

	}
}	