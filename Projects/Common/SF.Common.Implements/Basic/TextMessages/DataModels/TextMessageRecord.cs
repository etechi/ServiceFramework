﻿using SF.Metadata;
using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Data.Storage;
using System.ComponentModel.DataAnnotations;
using SF.Services.TextMessages.Admin;

namespace SF.Basic.TextMessages.DataModels
{
	[Table("app_text_message_record")]
	[Comment(GroupName = "文本消息服务", Name = "文本消息记录", Description = "记录所有外发文本消息以及发送结果")]
	public class TextMessageRecord
	{
		[Comment("ID")]
		[Key]
		public long Id { get; set; }

		[Comment("发送状态")]
		[Index("status", Order = 1)]
		public SendStatus Status { get; set; }

		[Comment("目标用户ID")]
		[Index("user", Order = 1)]
		public int? TargetUserId { get; set; }

		[Comment("消息发送服务ID")]
		[Index("service", Order = 1)]
		public int ServiceId { get; set; }

		[Comment("发信人")]
		[MaxLength(100)]
		public string Sender { get; set; }

		[Comment("收信人")]
		[Required]
		public string Targets { get; set; }

		[Comment("标题")]
		[MaxLength(100)]
		public string Title { get; set; }

		[Comment("内容")]
		[MaxLength(1000)]
		public string Body { get; set; }

		[Comment("消息头部数据")]
		[MaxLength(1000)]
		public string Headers { get; set; }

		[Comment("消息参数", Description = "用于支持模板化消息发送")]
		[MaxLength(1000)]
		public string Args { get; set; }

		[Index]
		[Index("status", Order = 2)]
		[Index("user", Order = 2)]
		[Index("service", Order = 2)]
		[Comment("创建时间")]
		public DateTime CreatedTime { get; set; }

		[Comment("完成时间")]
		public DateTime? CompletedTime { get; set; }

		[Comment("错误记录")]
		public string Error { get; set; }

		[Comment("单项发送结果")]
		public string TargetResults { get; set; }

		[Comment("业务跟踪ID")]
		[MaxLength(100)]
		public string TrackEntityId { get; set; }

	}
}
