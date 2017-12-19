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
using SF.Sys.Data;
using SF.Sys.Entities.DataModels;

namespace SF.Sys.Plans.DataModels
{
	/// <summary>
	/// 计划执行器
	/// </summary>
	[Table("ActionPlanExecutor")]
    public class ActionPlanExecutor : ObjectEntityBase<string>
	{
		[Index]
		public long PlanId { get; set; }

		[ForeignKey(nameof(PlanId))]
		public ActionPlan Plan { get; set; }
		/// <summary>
		/// 上次执行时间
		/// </summary>
		public DateTime LastActiveTime { get; set; }
		
		/// <summary>
		/// 执行堆栈
		/// </summary>
		public string Stack { get; set; }
		
		/// <summary>
		/// 错误信息
		/// </summary>
		[MaxLength(200)]
		public string Error { get; set; }

		/// <summary>
		/// 最后执行时间
		/// </summary>
		public DateTime? LastExecTime { get; set; }
		
		/// <summary>
		/// 错误次数
		/// </summary>
		public int ErrorCount { get; set; }

		/// <summary>
		/// 执行异常信息
		/// </summary>
		[MaxLength(200)]
		public string ExecError { get; set; }

	}
}
