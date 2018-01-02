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

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Entities.DataModels;
using SF.Sys.Data;
using SF.Common.Notifications.Models;
using SF.Common.Notifications.Senders;
using System.Collections.Generic;
using SF.Sys;

namespace SF.Common.Notifications.DataModels
{
	[Table("NotificationSendRecord")]
	public class NotificationSendRecord : EventEntityBase,ISendArgument
	{
		/// <summary>
		/// 状态
		/// </summary>
		public SendStatus Status { get; set; }

		/// <summary>
		/// 发送服务
		/// </summary>
		[Index]
		public long ProviderId { get; set; }

		/// <summary>
		/// 接收用户
		/// </summary>
		[Index]
		public long? TargetId { get; set; }

		/// <summary>
		/// 接收方
		/// </summary>
		public string Target { get; set; }

		/// <summary>
		/// 标题
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// 内容
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// 使用模板
		/// </summary>
		public string Template { get; set; }

		/// <summary>
		/// 消息参数
		/// </summary>
		public string Args { get; set; }

		/// <summary>
		/// 重试间隔
		/// </summary>
		public int RetryInterval { get; set; }

		/// <summary>
		/// 发送重试次数
		/// </summary>
		public int RetryCount { get; set; }

		/// <summary>
		/// 发送超时时间
		/// </summary>
		public DateTime Expires { get; set; }

		/// <summary>
		/// 下次发送时间
		/// </summary>
		public DateTime SendTime { get; set; }
		/// <summary>
		/// 上次发送时间
		/// </summary>
		public DateTime? LastSendTime { get; set; }

		/// <summary>
		/// 错误信息
		/// </summary>
		[MaxLength(1000)]
		public string Error { get; set; }

		/// <summary>
		/// 发送结果
		/// </summary>
		[MaxLength(1000)]
		public string Result { get; set; }

		/// <summary>
		/// 跟踪对象
		/// </summary>
		[MaxLength(100)]
		[Index]
		public string BizIdent { get; set; }

		/// <summary>
		/// 消息记录
		/// </summary>
		[Index]
		public long NotificationId { get; set; }

		[ForeignKey(nameof(NotificationId))]
		public Notification Notification { get; set; }

		public IReadOnlyDictionary<string,string> GetArguments()
		{
			if (Args.IsNullOrEmpty())
				return new Dictionary<string, string>();
			return Json.Parse<Dictionary<string, string>>(Args);
		}
	}


}
