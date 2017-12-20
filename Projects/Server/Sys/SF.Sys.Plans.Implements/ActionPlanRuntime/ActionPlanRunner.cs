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
using System.Threading.Tasks;
using SF.Sys.Entities;
using SF.Sys.Plans.Manager;
using SF.Sys.Linq;
using System.Linq;

namespace SF.Sys.Plans.ActionPlanRuntime
{

	

	class ActionPlanRunner : ICallable
	{
		class StackFrameData
		{
			/// <summary>
			/// PlanId
			/// </summary>
			public long p { get; set; }

			/// <summary>
			/// ActionId
			/// </summary>
			public long a { get; set; }

			/// <summary>
			/// ActionProviderId
			/// </summary>
			public long v { get; set; }

			/// <summary>
			/// 回调上下文
			/// </summary>
			public string x { get; set; }
		}
		class ActionPlanRunnerArgument
		{
			/// <summary>
			/// 调用堆栈
			/// </summary>
			public StackFrameData[] s { get; set; }

			/// <summary>
			/// 根执行计划ID
			/// </summary>
			public long p { get; set; }

		}
		class StackFrame
		{
			public IRuntimeAction Action { get; set; }
			public string Context { get; set; }
		}
		IEntityCache<long, RuntimePlan> PlanCache { get; }
		SF.Sys.Services.TypedInstanceResolver<IActionProvider> ActionProviderResolver { get; }

		public ActionPlanRunner(
			IEntityCache<long, RuntimePlan> PlanCache, 
			SF.Sys.Services.TypedInstanceResolver<IActionProvider> ActionProviderResolver
			)
		{
			this.PlanCache = PlanCache;
			this.ActionProviderResolver = ActionProviderResolver;
		}

		class ExecContext : IActionExecContext
		{


			public string Argument { get; set; }

			public object Result { get; set; }
			public bool IsCallback { get; set; }
			public string CallbackContext { get; set; }
			public Exception Error { get; set; }

			public IRuntimeAction Action { get; set; }
		}
	
		
		
		async Task LoadStackFrame(
			List<StackFrame> frames,
			StackFrameData[] framesData,
			ExecContext ctx,
			long curPlanId
			)
		{
			var validatedFrameIndex = 0;

			for (; validatedFrameIndex < framesData.Length; validatedFrameIndex++)
			{
				var f = framesData[validatedFrameIndex];
				if (curPlanId != f.p)
				{
					ctx.Error = new ActionCallValidateException(
						ValidatedFailedReason.ExecPlanChanged,
						$"执行中的计划调用动作的指定计划发生变动。 原计划ID:{f.p}, 新计划ID:{curPlanId}"
						);
					break;
				}

				var p = await PlanCache.Find(f.p);
				if (p == null)
				{
					ctx.Error = new ActionCallValidateException(
							ValidatedFailedReason.ExecPlanMissing,
							$"找不到执行中计划调用动作的计划: {f.p}"
							);
					break;
				}

				if (!p.TryGetValue(f.a, out var action))
				{
					ctx.Error = new ActionCallValidateException(
							ValidatedFailedReason.ActionMissing,
							$"在计划中找不到执行中的动作: 计划:{f.p} 动作:{f.a}"
							);
					break;
				}

				frames.Add(new StackFrame
				{
					Action = action,
					Context=f.x
				});

				if (action.ActionProviderId != f.v)
				{
					ctx.Error = new ActionCallValidateException(
							ValidatedFailedReason.ActionMissing,
							$"执行中动作的动作提供者发生变动。计划:{f.p} 原提供者:{f.v} 新动作:{action.ActionProviderId}"
							);
					validatedFrameIndex++;
					break;
				}

				//如果是最后一个，退出
				if (validatedFrameIndex == framesData.Length - 1)
					continue;

				//否者当前是计划调用动作，需要提取调用计划ID
				var ap = ActionProviderResolver(f.v);
				if (ap == null)
					throw new InvalidOperationException("找不到动作提供者服务:" + f.v);
				var peap = ap as IPlanExecActionProvider;
				if (peap == null)
					throw new InvalidOperationException($"动作提供者服务{f.v} {ap.GetType()} 不是一个计划调用动作");

				var pid = peap.ResolvePlanId(action.ActionProviderOptions);
				if (!pid.HasValue)
				{
					ctx.Error = new ActionCallValidateException(
						ValidatedFailedReason.NoExecPlanId,
						$"计划调用动作未指定计划ID。计划:{f.p} 原提供者:{f.v} 新动作:{action.ActionProviderId}"
						);
					validatedFrameIndex++;
					break;
				}
				curPlanId = pid.Value;
			}

			if (validatedFrameIndex < framesData.Length)
			{
				var cancelledException = new ActionCallValidateException(
						ValidatedFailedReason.PlanCancelled,
						"计划已取消",
						ctx.Error
						);
				var cctx = new ExecContext
				{
					IsCallback = true,
					Error = cancelledException
				};

				for (var i = framesData.Length - 1; i >= validatedFrameIndex; i--)
				{
					var f = framesData[i];
					var ap = ActionProviderResolver(f.v);
					if (ap == null)
						throw new InvalidOperationException("找不到动作提供者服务:" + f.v);
					cctx.CallbackContext = f.x;
					await ap.Execute(ctx);
				}
			}
		}

		
		async Task<IActionResult> ExecFrame(List<StackFrame> frames,ExecContext ctx)
		{
			var count = frames.Count;
			if (count== 0)
				return null;
			var frame = frames[count - 1];

			var provider = ActionProviderResolver(frame.Action.ActionProviderId);
			if (provider == null)
				throw new InvalidOperationException($"找不到动作的执行器:计划:{frame.Action.Plan} 动作:{frame.Action}");

			ctx.Action = frame.Action;
			ctx.CallbackContext = frame.Context;
			
			try
			{
				var re = await provider.Execute(ctx);
				return re;
			}
			catch (Exception err)
			{
				return new ErrorResult { Error = err };
			}
		}

		DateTime? ProcessActionResult(
			List<StackFrame> frames,
			ExecContext ctx,
			IActionResult result
			)
		{
			var frame = frames[frames.Count - 1];
			switch (result.Type)
			{
				case ActionResultType.DelayTo:
					var dtr = (DelayToResult)result;
					frame.Context = dtr.Context;
					return dtr.Target;
				case ActionResultType.NewStackFrame:
					var ep = (ExecActionResult)result;
					frame.Context = ep.Context;
					frames.Add(new StackFrame { Action = ep.Action, Context = ep.Context });
					return null;
				case ActionResultType.WaitEvent:
					var wer = (WaitEventResult)result;
					frame.Context = wer.Context;
					return wer.Expires;
				case ActionResultType.None:
					break;
				case ActionResultType.WaitUserInput:
					var wuir = (WaitUserInputResult)result;
					frame.Context = wuir.Context;
					return wuir.Expires;
				case ActionResultType.Return:
					ctx.Result = ((ReturnResult)result).Result;
					break;
				case ActionResultType.Error:
					ctx.Error = ((ErrorResult)result).Error;
					break;
				default:
					throw new NotSupportedException($"发现不支持的动作结果类型:${result.Type} ${result}");
			}
			if (ctx.Error != null || frame.Action.NextAction == null)
				frames.RemoveAt(frames.Count - 1);
			else
			{
				frame.Action = frame.Action.NextAction;
				frame.Context = null;
			}
			return null;
		}

		public async Task<ICallResult> Execute(ICallContext CallContext)
		{
			var arg = Json.Parse<ActionPlanRunnerArgument>(CallContext.Argument);
			var ctx = new ExecContext
			{
				Error = CallContext.Exception,
				Result = CallContext.CallData
			};
			var frames = new List<StackFrame>();
			if (arg.s==null)
			{
				var plan = await PlanCache.Find(arg.p);
				if (plan == null)
					ctx.Error=new ActionCallValidateException(
						ValidatedFailedReason.ExecPlanMissing,
						$"找不到执行计划:{arg.p}"
						);
				else if (plan.FirstAction!=null)
					frames.Add(new StackFrame { Action = plan.FirstAction });
			}
			else
			{
				ctx.IsCallback = true;
				await LoadStackFrame(frames, arg.s, ctx, arg.p);
			}

			for (; ; )
			{
				var re = await ExecFrame(frames, ctx);
				if (re == null)
					break;

				var delayTarget = ProcessActionResult(frames, ctx, re);
				if (delayTarget.HasValue)
				{
					arg.s = frames.Select(f => new StackFrameData
					{
						a = f.Action.Id,
						v = f.Action.ActionProviderId,
						x = f.Context,
						p = f.Action.Plan.Id
					}).ToArray();

					return new RepeatCallResult
					{
						NextCallArgument = Json.Stringify(arg),
						NextCallTime = delayTarget.Value
					};
				}
			}

			return null;
		}
	}
}
