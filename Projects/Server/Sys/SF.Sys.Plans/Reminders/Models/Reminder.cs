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
using SF.Sys.Entities;
using SF.Sys.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SF.Sys.Reminders.Models
{
	public class RemindAction:IEntityWithId<string>
	{
		/// <summary>
		/// 标题
		/// </summary>
		[ReadOnly(true)]
		public string Id { get; set; }

		/// <summary>
		/// 动作
		/// </summary>
		[ReadOnly(true)]
		public string Action { get; set; }

		/// <summary>
		/// 描述
		/// </summary>
		[ReadOnly(true)]
		public string Description { get; set; }
	}
	/// <summary>
	/// 提醒
	/// </summary>
	[EntityObject]
	public class Reminder : SF.Sys.AtLeastOnceTasks.Models.AtLeastOnceTaskEntityBase<long>
	{	
		/// <summary>
		/// 计划时间
		/// </summary>
		[TableVisible]
		public DateTime PlanTime { get; set; }

		/// <summary>
		/// 下次执行计划
		/// </summary>
		[TableRows]
		public RemindAction[] Actions { get; set; }
	}
}
