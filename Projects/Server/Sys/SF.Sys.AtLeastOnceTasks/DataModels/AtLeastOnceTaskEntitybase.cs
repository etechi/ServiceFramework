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

using SF.Sys.Entities.DataModels;
using System;
using SF.Sys.AtLeastOnceTasks.Models;
using SF.Sys.Data;

namespace SF.Sys.AtLeastOnceTasks.DataModels
{
	public class DataAtLeastOnceTaskEntityBase<TKey> : 
		DataObjectEntityBase<TKey>, 
		IAtLeastOnceTask
		where TKey:IEquatable<TKey>
	{
		/// <summary>
		/// 任务状态
		/// </summary>
		[Index("timer",Order =1)]
		public virtual AtLeastOnceTaskState TaskState { get; set; }
		/// <summary>
		/// 任务执行次数
		/// </summary>
		public virtual int TaskExecCount { get; set; }
		/// <summary>
		/// 最后执行消息
		/// </summary>
		public virtual string TaskMessage { get; set; }
		/// <summary>
		/// 任务开始时间
		/// </summary>
		public virtual DateTime? TaskStartTime { get; set; }
		/// <summary>
		/// 任务最后尝试时间
		/// </summary>
		public virtual DateTime? TaskLastExecTime { get; set; }
		/// <summary>
		/// 任务下次尝试时间
		/// </summary>
		[Index("timer", Order = 2)]
		public virtual DateTime? TaskNextExecTime { get; set; }
	}
}
