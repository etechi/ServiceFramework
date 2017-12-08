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
using SF.Common.TextMessages.Models;

namespace SF.Common.TextMessages.Management.DataModels
{

	/// <summary>
	/// 文本消息记录
	/// </summary>
	[Table("CommonTextMessageRecords")]
	public class MsgRecord : EventEntityBase
	{ 
		/// <summary>
		/// 目标用户ID
		/// </summary>
		[Index("user", Order = 1)]
		public override long? UserId { get; set; }


		[MaxLength(100)]
		[Required]
		public string PolicyIdent { get; set; }
		/// <summary>
		/// 策略Id
		/// </summary>
		[Index]
		public long PolicyId { get; set; }
		
		[ForeignKey(nameof(PolicyId))]
		public MsgPolicy Policy { get; set; }


		///<title>消息参数</title>
		/// <summary>
		/// 用于支持模板化消息发送
		/// </summary>
		[JsonData]
		public string Args { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		[Index]
		[Index("status", Order = 2)]
		[Index("user", Order = 2)]
		[Index("service", Order = 2)]
		[Display(Name = "")]
		public override DateTime Time { get; set; }

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
		/// 业务跟踪ID
		/// </summary>
		[MaxLength(100)]
		public string TrackEntityId { get; set; }

		/// <summary>
		/// 发送状态
		/// </summary>
		public SendStatus Status { get; set; }

		[MaxLength(1000)]
		public string Error { get; set; }

	}

}
