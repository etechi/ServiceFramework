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
namespace SF.Sys.ActionPlans.DataModels
{
	/// <summary>
	/// 调用实例
	/// </summary>
	[Table("ActionPlanAction")]
    public class ActionPlanAction : ItemEntityBase<ActionPlan>
	{
		[Index]
		public long PlanId { get; set; }

		[ForeignKey(nameof(PlanId))]
		public ActionPlan Plan { get; set; }
		
		/// <summary>
		/// 动作服务类型
		/// </summary>
		[Index]
		public long ActionProviderId { get; set; }

		/// <summary>
		/// 动作设置
		/// </summary>
		public string ActionProviderOptions { get; set; }

		/// <summary>
		/// 超时(s)
		/// </summary>
		public int ExpireSeconds { get; set; }

		/// <summary>
		/// 错误延时
		/// </summary>
		public int DelaySecondsOnError { get; set; }
	}
}
