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

using SF.Sys.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Sys.Entities;
using SF.Sys.Plans.Manager.Models;
using SF.Sys.Annotations;
using System;

namespace SF.Sys.Plans.ActionPlanRuntime.ActionProviders
{
	public enum ActionOnError
	{
		/// <summary>
		/// 重试
		/// </summary>
		Retry,
		/// <summary>
		/// 执行下一个动作
		/// </summary>
		MoveNext,
		/// <summary>
		/// 继续报错
		/// </summary>
		Error
	}
	public class PlanExecOptions
	{
		/// <summary>
		/// 动作计划
		/// </summary>
		[EntityIdent(typeof(ActionPlan))]
		public long PlanId { get; set; }

		/// <summary>
		/// 错误处理方式
		/// </summary>
		public ActionOnError ErrorAction { get; set; }
	}
	public class PlanExecActionProvider : IActionProvider
	{
		IEntityCache<long,IRuntimePlan> PlanCache { get; }
		public PlanExecActionProvider(IEntityCache<long, IRuntimePlan> PlanCache)
		{
			this.PlanCache = PlanCache;
		}
		async Task<IActionResult> OnError(PlanExecOptions options, Exception error)
		{
			var errorAction = ActionOnError.Retry;
			if (error is ActionCallValidateException acve)
			{
				switch (acve.Reason)
				{
					case ValidatedFailedReason.ActionProviderChanged:
					case ValidatedFailedReason.ExecPlanChanged:
						break;
					case ValidatedFailedReason.PlanCancelled:
						errorAction = ActionOnError.Error;
						break;
					case ValidatedFailedReason.ActionMissing:
					case ValidatedFailedReason.ExecPlanMissing:
					case ValidatedFailedReason.NoExecPlanId:
					default:
						errorAction = ActionOnError.MoveNext;
						break;

				}
			}
			if (errorAction < options.ErrorAction)
				errorAction = options.ErrorAction;

			switch (errorAction)
			{
				case ActionOnError.Error:
					return new ErrorResult { Error = error };
				case ActionOnError.MoveNext:
					return new NoneResult();
				case ActionOnError.Retry:
					var plan = await PlanCache.Find(options.PlanId);
					if (plan == null || plan.FirstAction == null)
						return new NoneResult();
					return new ExecActionResult{Action = plan.FirstAction};
				default:
					throw new NotSupportedException($"不支持的错误处理方式:{errorAction}");
			}
		}
		public async Task<IActionResult> Execute(IActionExecContext Context)
		{
			var options = Json.Parse<PlanExecOptions>(Context.Action.ActionProviderOptions);

			if (Context.Error != null)
				return await OnError(options, Context.Error);

			if (Context.IsCallback)
				return new ReturnResult { Result = Context.Result };

			var plan = await PlanCache.Find(options.PlanId);
			if (plan == null)
				return await OnError(
					options, 
					new ActionCallValidateException(
						ValidatedFailedReason.ExecPlanMissing, 
						$"找不到指定的计划:{options.PlanId}"
						)
				);
			return new ExecActionResult
			{
				Action = plan.FirstAction,
			};
		}
	}
}
