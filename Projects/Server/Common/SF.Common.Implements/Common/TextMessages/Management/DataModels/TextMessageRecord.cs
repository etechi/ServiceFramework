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

using SF.Metadata;
using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Data;
using System.ComponentModel.DataAnnotations;
using SF.Common.TextMessages.Management;
using SF.Entities.DataModels;

namespace SF.Common.TextMessages.Management.DataModels
{
	[Table("CommonTextMessageRecord")]
	[Comment(GroupName = "文本消息服务", Name = "文本消息记录", Description = "记录所有外发文本消息以及发送结果")]
	public class TextMessageRecord : EventEntityBase
	{
		

		[Comment("发送状态")]
		[Index("status", Order = 1)]
		public SendStatus Status { get; set; }

		[Comment("目标用户ID")]
		[Index("user", Order = 1)]
		public override long? UserId { get; set; }

		[Comment("消息发送服务ID")]
		[Index("service", Order = 1)]
		public long ServiceId { get; set; }

		[Comment("发信人")]
		[MaxLength(100)]
		public string Sender { get; set; }

		[Comment("收信人")]
		[Required]
		public string Target { get; set; }

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
		public override DateTime Time { get; set; }

		[Comment("完成时间")]
		public DateTime? CompletedTime { get; set; }

		[Comment("错误记录")]
		public string Error { get; set; }

		[Comment("单项发送结果")]
		public string Result { get; set; }

		[Comment("业务跟踪ID")]
		[MaxLength(100)]
		public string TrackEntityId { get; set; }

	}
}
