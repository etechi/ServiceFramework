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

using SF.Sys.Collections.Generic;
using SF.Sys.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Sys.Plans.Manager.DataModels;
using SF.Sys.Entities;
using SF.Sys.Plans.Manager;
using SF.Sys.Linq;

namespace SF.Sys.Plans.CallPlanRuntime
{
	class StackFrame
	{
		public long p;
		public int i;
	}
	class ActionPlanRunnerArgument
	{
		/// <summary>
		/// 调用堆栈
		/// </summary>
		public List<StackFrame> s { get; set; }

		/// <summary>
		/// 根执行计划ID
		/// </summary>
		public long p { get; set; }

		/// <summary>
		/// 回调上下文
		/// </summary>
		public string x { get; set; }

	}
	public class ActionPlanRunner : ICallable
	{
		IDataContext DataContext { get; }
		IEntityCache<long, ActionPlan> PlanCache { get; }
		SF.Sys.Services.TypedInstanceResolver<IActionProvider> ActionProviderResolver { get; }

		public ActionPlanRunner(IDataContext DataContext, IEntityCache<long, ActionPlan> PlanCache, SF.Sys.Services.TypedInstanceResolver<IActionProvider> ActionProviderResolver)
		{
			this.DataContext = DataContext;
			this.PlanCache = PlanCache;
			this.ActionProviderResolver = ActionProviderResolver;
		}

		Task Error(PlanExecutor e, string Message)
		{
			return null;
		}
		class ExecContext : IActionExecContext
		{
			public string Options { get; set; }

			public string Argument { get; set; }

			public object Data { get; set; }
			public bool IsCallback { get; set; }
			public string CallbackContext { get; set; }
		}
		public async Task<ICallResult> Execute(ICallContext CallContext)
		{
			var arg = Json.Parse<ActionPlanRunnerArgument>(CallContext.Argument);
			var callback = true;
			if (arg.s==null)
			{
				callback = false;
				arg.s = new List<StackFrame> { new StackFrame { i = 0, p = arg.p } };
			}

			var frames = arg.s;
			object Result = CallContext.CallData;
			for (; ; )
			{
				var count = frames.Count;
				if (count == 0)
					break;

				var cf = frames[count - 1];
				frames.RemoveAt(count - 1);

				var plan = await PlanCache.Find(cf.p);
				if (plan == null)
					throw new ArgumentException();

				var actions = (PlanAction[])plan.Items;
				if (cf.i >= actions.Length)
					throw new ArgumentException();
				var action = actions[cf.i];
				var provider = ActionProviderResolver(action.ActionProviderId);

				var ctx = new ExecContext
				{
					CallbackContext=callback?arg.x:null,
					IsCallback=callback,
					Options = action.Options,
					Data = Result,
				};
				var re = await provider.Execute(ctx);
				callback = false;
				if (re.Type==ActionResultType.s)
				{
					if (cf.i < actions.Length - 1)
					{
						cf.i++;
						frames.Add(cf);
					}
				}

				var result = await provider.Execute(ctx);
				switch (result.Type)
				{
					case ActionResultType.DelayTo:
						var dtr = (DelayToResult)result;
						return new RepeatCallResult
						{
							NextCallArgument = Json.Stringify(frames),
							NextCallTime = dtr.Target
						};
					case ActionResultType.ExecPlan:
						var ep = (ExecPlanResult)result;
						frames.Add(new StackFrame
						{
							p = ep.PlanId,
							i = 0
						});
						break;
					case ActionResultType.None:
						break;
					case ActionResultType.WaitEvent:
						var wer = (WaitEventResult)result;
						frames.Add(cf);
						return new RepeatCallResult
						{
							NextCallTime = wer.Expires,
							NextCallArgument = Json.Stringify(frames)
						};
					case ActionResultType.WaitUserInput:
						var wuir = (WaitUserInputResult)result;
						frames.Add(cf);
						return new RepeatCallResult
						{
							NextCallTime = wuir.Expires,
							NextCallArgument = Json.Stringify(frames)
						};
					case ActionResultType.Return:
						Result = ((ReturnResult)result).Result;
						break;
				}
			}

			return null;
		}
	}
}
