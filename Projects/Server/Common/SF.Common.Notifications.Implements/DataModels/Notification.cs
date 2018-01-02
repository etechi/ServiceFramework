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
using SF.Sys.Annotations;
using SF.Common.Notifications.Models;
using SF.Sys.Entities;
using System.Collections.Generic;

namespace SF.Common.Notifications.DataModels
{

	/// <summary>
	/// 通知记录
	/// </summary>
	[Table("Notification")]
	public class Notification : 
		IEntityWithId<long>
	{
		/// <summary>
		/// ID
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public long Id { get; set; }

		/// <summary>
		/// 模式
		/// </summary>
		public NotificationMode Mode { get; set; }
		/// <summary>
		/// 标题
		/// </summary>
		[MaxLength(200)]
		[Required]
		public string Name { get; set; }

		/// <summary>
		/// 图片
		/// </summary>
		[MaxLength(50)]
		public string Image { get; set; }

		/// <summary>
		/// 内容
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// 链接
		/// </summary>
		[MaxLength(200)]
		public string Link { get; set; }

		/// <summary>
		/// 发送者ID
		/// </summary>
		[Index]
		public long? SenderId { get; set; }

		/// <summary>
		/// 业务跟踪标识
		/// </summary>
		[MaxLength(100)]
		[Index]
		public string BizIdent { get; set; }

		/// <summary>
		/// 过期时间
		/// </summary>
		[Index("boardcast", Order = 1)]
		public DateTime Expires { get; set; }

		/// <summary>
		/// 时间
		/// </summary>
		[Index("boardcast", Order = 2)]
		public DateTime Time { get; set; }

		/// <summary>
		/// 逻辑状态
		/// </summary>
		public EntityLogicState LogicState { get; set; }

		[InverseProperty(nameof(NotificationTarget.Notification))]
		public ICollection<NotificationTarget> Targets { get; set; }

		[MaxLength(100)]
		[Required]
		public string PolicyIdent { get; set; }
		
		/// <summary>
		/// 策略Id
		/// </summary>
		[Index]
		public long? PolicyId { get; set; }
		
		[ForeignKey(nameof(PolicyId))]
		public NotificationSendPolicy Policy { get; set; }


		///<title>消息参数</title>
		/// <summary>
		/// 用于支持模板化消息发送
		/// </summary>
		[JsonData]
		public string Args { get; set; }
		

		/// <summary>
		/// 完成时间
		/// </summary>
		public DateTime? EndTime { get; set; }


		/// <summary>
		/// 所有动作
		/// </summary>
		public int ActionCount { get; set; }

		/// <summary>
		/// 完成动作
		/// </summary>
		public int CompletedActionCount { get; set; }

		/// <summary>
		/// 发送状态
		/// </summary>
		public SendStatus Status { get; set; }

		[MaxLength(1000)]
		public string Error { get; set; }


		[InverseProperty(nameof(NotificationSendRecord.Notification))]
		public ICollection<NotificationSendRecord> SendRecords { get; set; }
	}

}
