using SF.Data;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Common.TextMessages.Admin
{
	[EntityObject("文本消息记录")]
	public class MsgRecord : IObjectWithId<long>
	{
		[Key]
		[TableVisible]
		[Comment( "Id")]
		public long Id { get; set; }

		[Comment( "状态")]
		[TableVisible]
		public SendStatus Status { get; set; }

		[Comment( "目标用户")]
		[EntityIdent("用户", nameof(TargetUserName))]
		public long? TargetUserId { get; set; }

		[Comment( "目标用户")]
		[Ignore]
		[TableVisible]
		public string TargetUserName { get; set; }

		[Comment( "接收方")]
		[TableVisible]
		public string Targets { get; set; }

		[Comment( "摘要")]
		[Ignore]
		[TableVisible]
		public string Summary { get; set; }

		[EntityIdent("系统服务", nameof(ServiceName))]
		[Comment( "发送服务")]
		public int ServiceId { get; set; }

		[Ignore]
		[Comment( "发送服务")]
		[TableVisible]
		public string ServiceName { get; set; }

		[Comment( "发送方")]
		public string Sender { get; set; }

		[Comment( "标题")]
		public string Title { get; set; }

		[Comment( "内容")]
		[MultipleLines]
		public string Body { get; set; }

		[Comment( "消息头部")]
		[MultipleLines]
		public string Headers { get; set; }

		[Comment( "消息参数")]
		[MultipleLines]
		public string Args { get; set; }

		[Comment( "发送时间")]
		[TableVisible]
		public DateTime CreatedTime { get; set; }

		[Comment( "完成时间")]
		public DateTime? CompletedTime { get; set; }

		[Comment( "错误信息")]
		[MultipleLines]
		public string Error { get; set; }

		[Comment( "单项错误信息")]
		[MultipleLines]
		public string TargetResults { get; set; }

		[Comment( "跟踪对象")]
		[EntityIdent]
		public string TrackEntityId { get; set; }
	}

}
