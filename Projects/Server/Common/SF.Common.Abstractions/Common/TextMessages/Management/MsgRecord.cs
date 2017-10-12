#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

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
	[EntityObject]
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
