using SF.Sys.Entities;
using SF.Sys.Annotations;
using SF.Sys.Entities.DataModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Data;
using System.Collections.Generic;
using System;

namespace SF.Common.Conversations.DataModels
{
	/// <summary>
	/// 会话状态
	/// </summary>
	[Table("SessionStatus")]
	public class DataSessionStatus: DataObjectEntityBase
	{
		/// <summary>
		/// 所有人ID
		/// </summary>
		[Index]
		public override long? OwnerId { get; set; }

		/// <summary>
		/// 会话标志
		/// </summary>
		public SessionFlag Flags { get; set; }

		/// <summary>
		/// 业务类型
		/// </summary>
		[Index("biz",IsUnique =true,Order =1)]
		[MaxLength(100)]
		[Required]
		public string BizIdentType { get; set; }

		/// <summary>
		/// 业务标识
		/// </summary>
		[Index("biz", IsUnique = true, Order = 2)]
		public long BizIdent { get; set; }

		/// <summary>
		/// 最后消息文本
		/// </summary>
		public string LastMessageText { get; set; }

		/// <summary>
		/// 最后消息类型
		/// </summary>
		public MessageType LastMessageType { get; set; }

		
		/// <summary>
		/// 最后消息
		/// </summary>
		[Index]
		public long? LastMessageId { get; set; }

		[ForeignKey(nameof(LastMessageId))]
		public DataSessionMessage LastMessage { get; set; }
		/// <summary>
		/// 消息数
		/// </summary>
		public int MessageCount { get; set; }

		
		/// <summary>
		/// 会话成员状态
		/// </summary>
		[InverseProperty(nameof(DataSessionMemberStatus.Session))]
		public ICollection<DataSessionMemberStatus> Members { get; set; }

		/// <summary>
		/// 会话消息
		/// </summary>
		[InverseProperty(nameof(DataSessionMessage.Session))]
		public ICollection<DataSessionMessage> Messages { get; set; }


		
	}
}