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
using System;

namespace SF.Sys.AtLeastOnceTasks.Models
{
	public class AtLeastOnceTaskEntityBase<TKey> : 
		ObjectEntityBase<TKey>
		where TKey:IEquatable<TKey>
	{
		/// <summary>
		/// 任务状态
		/// </summary>
		[TableVisible]
		public AtLeastOnceTaskState TaskState { get; set; }
		
		/// <summary>
		/// 任务开始时间
		/// </summary>
		public DateTime? TaskStartTime { get; set; }
		/// <summary>
		/// 最后执行时间
		/// </summary>
		[TableVisible]
		public DateTime? TaskLastExecTime { get; set; }
		/// <summary>
		/// 下次执行时间
		/// </summary>
		[TableVisible]
		public DateTime? TaskNextExecTime { get; set; }

		/// <summary>
		/// 任务执行次数
		/// </summary>
		[TableVisible]
		public int TaskExecCount { get; set; }

		/// <summary>
		/// 任务执行消息
		/// </summary>
		[TableVisible]
		public string TaskMessage { get; set; }
		
	}
}
