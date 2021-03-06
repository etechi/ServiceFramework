﻿#region Apache License Version 2.0
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
	/// 消息记录
	/// </summary>
	[EntityObject]
	public class MsgRecord : EventEntityBase
	{
		/// <summary>
		/// 消息名称
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 状态
		/// </summary>
		[TableVisible]
		public SendStatus Status { get; set; }

		/// <summary>
		/// 消息策略
		/// </summary>
		[EntityIdent(typeof(NotificationSendPolicy),nameof(PolicyName))]
		public long PolicyId { get; set; }

		/// <summary>
		/// 消息策略
		/// </summary>
		public string PolicyName { get; set; }

		/// <summary>
		/// 消息参数
		/// </summary>
		[MultipleLines]
		[JsonData]
		public string Args { get; set; }


		/// <summary>
		/// 开始时间
		/// </summary>
		public override DateTime Time { get; set; }

		/// <summary>
		/// 结束时间
		/// </summary>
		public DateTime? EndTime { get; set; }

		/// <summary>
		/// 跟踪对象
		/// </summary>
		[EntityIdent]
		public string TrackEntityId { get; set; }

		/// <summary>
		/// 所有动作
		/// </summary>
		public int ActionCount { get; set; }

		/// <summary>
		/// 完成动作
		/// </summary>
		public int CompletedActionCount { get; set; }

		/// <summary>
		/// 动作记录
		/// </summary>
		[TableRows]
		public NotificationSendRecord[] ActionRecords { get; set; }
	}

}
