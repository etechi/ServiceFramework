using SF.Sys.Entities;
using System;

namespace SF.Common.Conversations.Front
{
	/// <summary>
	/// 会话成员
	/// </summary>
	public class SessionMember : IEntityWithId<long>
	{
		/// <summary>
		/// 成员ID
		/// </summary>
		public long Id { get; set; }
		
		/// <summary>
		/// 成员昵称
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 成员图标
		/// </summary>
		public string Icon { get; set; }

		/// <summary>
		/// 会话ID
		/// </summary>
		public long SessionId { get; set; }

		/// <summary>
		/// 成员业务类型
		/// </summary>
		public int BizType { get; set; }
		/// <summary>
		/// 业务ID类型
		/// </summary>
		public string BizIdentType { get; set; }

		/// <summary>
		/// 业务ID
		/// </summary>
		public long? BizIdent { get; set; }

		/// <summary>
		/// 消息数
		/// </summary>
		public int MessageCount { get; set; }
		/// <summary>
		/// 最后活动时间
		/// </summary>
		public DateTime LastActiveTime { get; set; }

		/// <summary>
		/// 最后消息
		/// </summary>
		public SessionMessage LastMessage { get; set; }

		/// <summary>
		/// 成员通知
		/// </summary>
		public string Notification { get; set; }


		/// <summary>
		/// 成员加入状态
		/// </summary>
		public SessionJoinState JoinState { get; set; }
	}
}