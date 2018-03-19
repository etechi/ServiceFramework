using System;
using SF.Sys.Entities;
using SF.Sys.Annotations;
using SF.Sys.Entities.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SF.Common.Conversations.Models
{
	/// <summary>
	/// 会话消息
	/// </summary>
	public class SessionMessage : EventEntityBase
	{
		
		/// <summary>
		/// 会话
		/// </summary>
		[EntityIdent(typeof(SessionStatus),nameof(SessionName))]
		[Uneditable]
		public long SessionId { get; set; }

		/// <summary>
		/// 会话
		/// </summary>
		[Ignore]
		[TableVisible]
		public string SessionName { get; set; }


		/// <summary>
		/// 消息类型
		/// </summary>
		[Uneditable]
		public MessageType Type { get; set; }

		/// <summary>
		/// 消息内容
		/// </summary>
		[MaxLength(1000)]
		[EntityTitle]
		public string Text { get; set; }

		/// <summary>
		/// 消息参数
		/// </summary>
		[MaxLength(100)]
		public string Argument { get; set; }

		/// <summary>
		/// 发信成员ID
		/// </summary>
		[EntityIdent(typeof(SessionMemberStatus),nameof(PosterName))]
		[ReadOnly(true)]
		public long? PosterId { get; set; }

		/// <summary>
		/// 发信人昵称
		/// </summary>
		[Ignore]
		[TableVisible]
		public string PosterName { get; set; }

	}

	public class SessionMessageDetail : SessionMessage
	{
		public long? SessionOwnerId { get; set; }

		public string SessionBizType { get; set; }
		public long SessionBizIdent { get; set; }

		public string MemberBizType { get; set; }
		public string MemberBizIdent { get; set; }
	}
}