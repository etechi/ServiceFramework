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

namespace SF.Common.TextMessages.Management.DataModels
{
	/// <summary>
	/// 文本消息记录
	/// </summary>
	[Table("CommonTextMessageRecord")]
	public class TextMessageRecord : EventEntityBase
	{


		/// <summary>
		/// 发送状态
		/// </summary>
		[Index("status", Order = 1)]
		public SendStatus Status { get; set; }

		/// <summary>
		/// 目标用户ID
		/// </summary>
		[Index("user", Order = 1)]
		public override long? UserId { get; set; }

		/// <summary>
		/// 消息发送服务ID
		/// </summary>
		[Index("service", Order = 1)]
		public long ServiceId { get; set; }

		/// <summary>
		/// 发信人
		/// </summary>
		[MaxLength(100)]
		public string Sender { get; set; }

		/// <summary>
		/// 收信人
		/// </summary>
		[Required]
		public string Target { get; set; }

		/// <summary>
		/// 标题
		/// </summary>
		[MaxLength(100)]
		public string Title { get; set; }

		/// <summary>
		/// 内容
		/// </summary>
		[MaxLength(1000)]
		public string Body { get; set; }

		/// <summary>
		/// 消息头部数据
		/// </summary>
		[MaxLength(1000)]
		public string Headers { get; set; }

		///<title>消息参数</title>
		/// <summary>
		/// 用于支持模板化消息发送
		/// </summary>
		[MaxLength(1000)]
		public string Args { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		[Index]
		[Index("status", Order = 2)]
		[Index("user", Order = 2)]
		[Index("service", Order = 2)]
		public override DateTime Time { get; set; }

		/// <summary>
		/// 完成时间
		/// </summary>
		public DateTime? CompletedTime { get; set; }

		/// <summary>
		/// 错误记录
		/// </summary>
		public string Error { get; set; }

		/// <summary>
		/// 单项发送结果
		/// </summary>
		public string Result { get; set; }

		/// <summary>
		/// 业务跟踪ID
		/// </summary>
		[MaxLength(100)]
		public string TrackEntityId { get; set; }

	}
}
