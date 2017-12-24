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
using SF.Sys.ActionPlans.Models;
using SF.Sys.Annotations;
using System;

namespace SF.Sys.ActionPlans.Runtime.ActionProviders
{
	public class DelayOptions
	{
		public TimeSpan Time { get; set; }
		public ActionOnError ErrorAction { get; set; }
	}
	public class DelayProvider : IActionProvider
	{
		Lazy<TimeServices.ITimeService> TimeService { get; }
		public DelayProvider(Lazy<TimeServices.ITimeService> TimeService)
		{
			this.TimeService = TimeService;
		}
		IActionResult OnError(DelayOptions options, Exception error)
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
					return new DelayToResult{Target= TimeService .Value.Now.Add(options.Time)};
				default:
					throw new NotSupportedException($"不支持的错误处理方式:{errorAction}");
			}
		}
		
		public Task<IActionResult> Execute(IActionExecContext Context)
		{
			IActionResult result;
			var options = Json.Parse<DelayOptions>(Context.Action.ActionProviderOptions);

			if (Context.Error != null)
				result = OnError(options, Context.Error);
			else if (Context.IsCallback)
				result = new NoneResult();
			else
				result = new DelayToResult
				{
					Target = TimeService.Value.Now.Add(options.Time)
				};
			return Task.FromResult(result);
		}
	}
}
