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

using SF.Sys.Annotations;
using SF.Sys.Entities.Models;
using SF.Sys.Services.Management.Models;
using System;

namespace SF.Common.Notifications.Models
{
	/// <summary>
	/// 通知发送记录
	/// </summary>
	[EntityObject]
	public class NotificationSendRecord : EventEntityBase
	{
		/// <summary>
		/// 状态
		/// </summary>
		[TableVisible]
		public SendStatus Status { get; set; }

		/// <summary>
		/// 接收方
		/// </summary>
		[TableVisible]
		public string Target { get; set; }

		/// <summary>
		/// 发送服务
		/// </summary>
		[EntityIdent(typeof(ServiceInstance), nameof(ProviderName))]
		public long ProviderId { get; set; }

		/// <summary>
		/// 发送服务
		/// </summary>
		[Ignore]
		[TableVisible]
		public string ProviderName { get; set; }

		///// <summary>
		///// 发送方
		///// </summary>
		//public string Sender { get; set; }

		/// <summary>
		/// 标题
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// 内容
		/// </summary>
		[MultipleLines]
		public string Content { get; set; }


		/// <summary>
		/// 消息参数
		/// </summary>
		[MultipleLines]
		public string Args { get; set; }


		/// <summary>
		/// 完成时间
		/// </summary>
		public DateTime? CompletedTime { get; set; }

		/// <summary>
		/// 错误信息
		/// </summary>
		[MultipleLines]
		public string Error { get; set; }

		/// <summary>
		/// 发送结果
		/// </summary>
		public string Result { get; set; }

		/// <summary>
		/// 跟踪对象
		/// </summary>
		[EntityIdent]
		public string BizIdent { get; set; }

		/// <summary>
		/// 通知对象
		/// </summary>
		[EntityIdent(typeof(Notification),nameof(NotificationName))]
		public long NotificationId { get; set; }

		/// <summary>
		/// 通知对象
		/// </summary>
		[TableVisible]
		[Ignore]
		public string NotificationName { get;set;}
	}

}
