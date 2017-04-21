using SF.Data;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.System.TextMessages.Admin
{
	
	public class MsgRecordQueryArgument : Data.Entity.QueryArgument<long>
	{
		[Comment( "状态")]
		public SendStatus? Status { get; set; }

		[Comment( "目标用户")]
		[EntityIdent("用户")]
		public long? TargeUserId { get; set; }


		[Comment( "发送服务")]
		[EntityIdent("系统服务")]
		public int? ServiceId { get; set; }

		[Comment( "发送时间")]
		public QueryRange<DateTime> Time { get; set; }

		[Comment( "发送对象")]
		public string Target { get; set; }
	}

}
