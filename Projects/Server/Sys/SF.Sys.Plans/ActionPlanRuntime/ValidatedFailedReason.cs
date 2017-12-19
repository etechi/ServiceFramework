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

namespace SF.Sys.Plans.ActionPlanRuntime
{
	public enum ValidatedFailedReason
	{
		/// <summary>
		/// 执行中的计划调用动作的指定计划发送变动
		/// </summary>
		ExecPlanChanged,

		/// <summary>
		/// 找不到执行中计划调用动作的计划
		/// </summary>
		ExecPlanMissing,

		/// <summary>
		/// 在计划中找不到执行中的动作
		/// </summary>
		ActionMissing,

		/// <summary>
		/// 执行中动作的动作提供者发生变动
		/// </summary>
		ActionProviderChanged,

		/// <summary>
		/// 计划调用动作未指定计划ID
		/// </summary>
		NoExecPlanId,

		/// <summary>
		/// 计划已取消
		/// </summary>
		PlanCancelled
	}
}
