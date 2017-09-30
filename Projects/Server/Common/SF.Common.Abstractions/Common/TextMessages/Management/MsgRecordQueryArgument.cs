using SF.Auth.Identities.Models;
using SF.Core.ServiceManagement.Models;
using SF.Entities;
using SF.Metadata;
using System;

namespace SF.Common.TextMessages.Management
{

	public class MsgRecordQueryArgument : Entities.QueryArgument<ObjectKey<long>>
	{
		[Comment( "状态")]
		public SendStatus? Status { get; set; }

		[Comment( "目标用户")]
		[EntityIdent(typeof(Identity))]
		public long? TargeUserId { get; set; }


		[Comment( "发送服务")]
		[EntityIdent(typeof(ServiceInstance))]
		public long? ServiceId { get; set; }

		[Comment( "发送时间")]
		public QueryRange<DateTime> Time { get; set; }

		[Comment( "发送对象")]
		public string Target { get; set; }
	}

}
