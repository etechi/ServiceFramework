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
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SF.Sys.Reminders.Models
{
	/// <summary>
	/// 提醒记录
	/// </summary>
	[EntityObject]
	public class RemindRecord : EventEntityBase
	{
		/// <summary>
		/// 名称
		/// </summary>
		[MaxLength(100)]
		[Required]
		[TableVisible]
		public string Name { get; set; }

		/// <summary>
		/// 动作
		/// </summary>
		[MaxLength(100)]
		[Required]
		[TableVisible]
		public string Action { get; set; }

		/// <summary>
		/// 描述
		/// </summary>
		[MaxLength(200)]
		public string Description { get; set; }

		/// <summary>
		/// 错误
		/// </summary>
		[MaxLength(200)]
		public string Error { get; set; }

		/// <summary>
		/// 提醒
		/// </summary>
		[EntityIdent(typeof(Reminder),nameof(ReminderName))]
		public long ReminderId { get; set; }

		/// <summary>
		/// 提醒名称
		/// </summary>
		[TableVisible]
		[Ignore]
		public string ReminderName { get; set; }

		/// <summary>
		/// 业务跟踪
		/// </summary>
		[MaxLength(100)]
		[Required]
		[TableVisible]
		public string BizIdent { get; set; }
	}
}
