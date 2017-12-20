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
	public class RepeatOptions
	{
		public int RepeatCount { get; set; }
		/// <summary>
		/// 错误处理方式
		/// </summary>
		public ActionOnError ErrorAction { get; set; }
	}
	public class RepeatActionProvider : IActionProvider
	{
		public RepeatActionProvider()
		{
		}
		async Task<IActionResult> OnError(RepeatOptions options, Exception error,IRuntimeAction CurAction)
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
					if(CurAction==null || CurAction.FirstChildAction==null)
						return new NoneResult();
					return new ExecActionResult{Action = CurAction.FirstChildAction };
				default:
					throw new NotSupportedException($"不支持的错误处理方式:{errorAction}");
			}
		}
		class State
		{
			public int Step { get; set; }
		}
		public async Task<IActionResult> Execute(IActionExecContext Context)
		{
			var options = Json.Parse<RepeatOptions>(Context.Action.ActionProviderOptions);

			if (Context.Error != null)
				return await OnError(options, Context.Error,Context.Action);

			if (Context.Action.FirstChildAction == null)
				return new NoneResult();

			var state = Context.IsCallback ? Json.Parse<State>(Context.CallbackContext) : new State { Step=-1};
			state.Step++;
			if(state.Step>=options.RepeatCount || Context.Action.FirstChildAction==null)
				return state.Step==0?
					(IActionResult)new NoneResult(): 
					new ReturnResult { Result = Context.Result };
			return new ExecActionResult
			{
				Action = Context.Action.FirstChildAction,
				Context = Json.Stringify(state)
			};
		}
	}
}
