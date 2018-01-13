using SF.Sys.Entities;
using SF.Sys.Annotations;
using SF.Sys.Entities.Models;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using SF.Sys.Auth;
using System.ComponentModel.DataAnnotations;

namespace SF.Common.Conversations.Models
{
	/// <summary>
	/// 交谈会话
	/// </summary>
	public class Session: ObjectEntityBase
	{
		/// <summary>
		/// 图标
		/// </summary>
		public string Icon { get; set; }
		
		/// <summary>
		/// 成员数量
		/// </summary>
		[ReadOnly(true)]
		[TableVisible]
		public int MemberCount { get; set; }
		
		/// <summary>
		/// 消息数
		/// </summary>
		public int MessageCount { get; set; }
		
		/// <summary>
		/// 最后活跃时间
		/// </summary>
		[ReadOnly(true)]
		[TableVisible]
		public DateTime LastActiveTime { get; set; }

		/// <summary>
		/// 所有人
		/// </summary>
		[EntityIdent(typeof(User), nameof(OwnerName))]
		[Uneditable]
		public long? OwnerId { get; set; }

		/// <summary>
		/// 所有人
		/// </summary>
		[TableVisible]
		[Ignore]
		public string OwnerName { get; set; }

		/// <summary>
		/// 所有人成员
		/// </summary>
		[EntityIdent(typeof(SessionMember),nameof(OwnerMemberName))]
		[ReadOnly(true)]
		public long? OwnerMemberId { get; set; }

		/// <summary>
		/// 所有人成员
		/// </summary>
		[TableVisible]
		[Ignore]
		public string OwnerMemberName { get; set; }


		/// <summary>
		/// 业务标识类型
		/// </summary>
		[EntityType]
		public string BizIdentType { get; set; }

		/// <summary>
		/// 业务标识
		/// </summary>
		[EntityIdent(null,nameof(BizIdentName),EntityTypeField =nameof(BizIdentType))]
		public long? BizIdent { get; set; }

		/// <summary>
		/// 业务标识
		/// </summary>
		[Ignore]
		[TableVisible]
		public string BizIdentName { get; set; }

		/// <summary>
		/// 最后消息
		/// </summary>
		[ReadOnly(true)]
		[EntityIdent(typeof(SessionMessage),nameof(LastMessageName))]
		public long? LastMessageId { get; set; }

		/// <summary>
		/// 最后消息
		/// </summary>
		[TableVisible]
		[Ignore]
		public string LastMessageName { get; set; }
		
		
	}

	public class SessionEditable : Session
	{
		/// <summary>
		/// 成员
		/// </summary>
		[Ignore]
		public IEnumerable<SessionMember> Members { get; set; }
	}
}