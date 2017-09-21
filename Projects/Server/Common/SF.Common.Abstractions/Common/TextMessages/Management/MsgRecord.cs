using SF.Core.ServiceManagement.Models;
using SF.Data;
using SF.Data.Models;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Common.TextMessages.Management
{
	[EntityObject("文本消息记录")]
	public class MsgRecord : EventEntityBase
	{

		[Comment( "状态")]
		[TableVisible]
		public SendStatus Status { get; set; }

		[Comment( "接收方")]
		[TableVisible]
		public string Target { get; set; }

		[EntityIdent(typeof(ServiceInstance), nameof(ServiceName))]
		[Comment( "发送服务")]
		public long ServiceId { get; set; }

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


		[Comment( "完成时间")]
		public DateTime? CompletedTime { get; set; }

		[Comment( "错误信息")]
		[MultipleLines]
		public string Error { get; set; }

		[Comment("发送结果")]
		public string Result { get; set; }

		[Comment( "跟踪对象")]
		[EntityIdent]
		public string TrackEntityId { get; set; }
	}

}
