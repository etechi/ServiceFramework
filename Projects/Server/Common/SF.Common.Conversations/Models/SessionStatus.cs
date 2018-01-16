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
	/// 会话状态
	/// </summary>
	public class SessionStatus: ObjectEntityBase
	{
		
		/// <summary>
		/// 消息数
		/// </summary>
		[ReadOnly(true)]
		public int MessageCount { get; set; }
		
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
		/// 会话标志
		/// </summary>
		public SessionFlag Flags { get; set; }



		/// <summary>
		/// 业务标识类型
		/// </summary>
		[EntityType]
		public string BizIdentType { get; set; }

		/// <summary>
		/// 业务标识
		/// </summary>
		[EntityIdent(null,nameof(BizIdentName),EntityTypeField =nameof(BizIdentType))]
		public long BizIdent { get; set; }

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

	public class SessionEditable : SessionStatus
	{
	}
}