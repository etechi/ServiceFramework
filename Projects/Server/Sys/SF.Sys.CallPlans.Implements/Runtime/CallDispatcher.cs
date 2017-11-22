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

using SF.Sys.Logging;
using SF.Sys.Services;
using SF.Sys.TimeServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Sys.CallPlans.Runtime
{
	public class TimeoutException : PublicException
	{
		public TimeoutException(string message) : base(message) { }
		public TimeoutException(string message, System.Exception innerException) : base(message, innerException) { }
	}
	public class CallDispatcher:
		ICallDispatcher
	{
		public ILogger Logger { get; }
		public IServiceScopeFactory ScopeFactory { get; }
		public ITimeService TimeService { get; set; }
		public CallDispatcher(ILogger<CallDispatcher> Logger, IServiceScopeFactory ScopeFactory, TimeServices.ITimeService TimeService)
		{
			this.Logger = Logger;
			this.TimeService = TimeService;
			this.ScopeFactory = ScopeFactory;
		}

		public async Task<int> SystemStartupCleanup()
		{
			var now = TimeService.Now;
			using (var scope = ScopeFactory.CreateServiceScope())
			{
				var storage = scope.ServiceProvider.Resolve<ICallPlanStorage>();
				var timers = await storage.GetInstancesForCleanup(ConstantTimes.ExecutingStartTime);
				var error_unexcepted = new Exception("系统异常终止");
				var actions = new List<ICallPlanStorageAction>();
				foreach (var timer in timers)
				{
					actions.Add(CreateStorageAction(storage,timer, error_unexcepted, now, null));
				}
				await storage.ExecuteActions(actions);
				return timers.Length;
			}
		}
		public async Task<Task[]> Execute(int count)
		{
			var now = TimeService.Now;
			string[] ids;
			using (var scope = ScopeFactory.CreateServiceScope())
			{
				var storage = scope.ServiceProvider.Resolve<ICallPlanStorage>();
				ids = await storage.GetOnTimeInstances(
					count,
					now,
					ConstantTimes.ExecutingStartTime,
					ConstantTimes.InitTime
					);
			}

			return ids.Select(id =>
					Task.Run(() => ExecuteInstance(id,null))
					).ToArray();
		}

		void Log(LogLevel Level, string message,ICallInstance instance,Exception error=null)
		{
			this.Logger.Write(
				Level,
				error,
				"{0} {1} {2} {3}", 
				message, 
				instance.Callable, 
				instance.CallArgument, 
				instance.CallError
				);
		}
		public async Task Execute(string id,object CallData)
		{
            await ExecuteInstance(id,CallData);
		}


        static ObjectSyncQueue<string> ExecQueue { get; } = new ObjectSyncQueue<string>();
		class CallContext : ICallContext
		{
			public string Argument { get; set; }

			public string Context { get; set; }

			public Exception Exception { get; set; }

			public object CallData { get; set; }
		}
		Task ExecuteInstance(string id,object CallData)
		{
             //同一个调用不能进入多次
             return ExecQueue.Queue(id, async () =>
             {
                 using (var scope = ScopeFactory.CreateServiceScope())
                 {
					 var sp = scope.ServiceProvider;
					 var storage = sp.Resolve<ICallPlanStorage>();
                     var instance = await storage.GetInstance(id);
                     var i = id.IndexOf(':');
                     if (i == -1)
                     {
                         Logger.Error("发现无效主键！ " + id);
                         return;
                     }
					 var si = id.IndexOf('-', 0, i);
                     var CallableName = si==-1?id.Substring(0, i):id.Substring(0,si);
					 var CallableIdent = si == -1 ? null : (long?)long.Parse(id.Substring(si + 1, i - si - 1));
                     var CallContext = id.Substring(i + 1);

                     try
                     {
                         Exception error = null;
                         var now = TimeService.Now;
                         int? repeatDelay = null;
                         if (instance.Expire > now)
                         {
                             try
                             {
                                 var cb = sp.Resolve<CallableFactory>().Create(sp, CallableName,CallableIdent);
                                 await cb.Execute(
									 new CallContext {
										 Argument=instance.CallArgument,
										 Context=CallContext,
										 Exception=ExceptionFactory.CreateFromError(instance.CallError),
										 CallData=CallData
									 }
									 );
                                 Log(LogLevel.Trace, "执行完成", instance);
                             }
                             catch (RepeatCallException rce)
                             {
                                 repeatDelay = (int)rce.Target.Subtract(now).TotalSeconds;
                             }
                             catch (Exception e)
                             {
                                 Log(LogLevel.Error, "执行异常", instance, e);
                                 error = e;
                                 now = TimeService.Now;
                             }
                         }
                         if (instance.Expire <= now)
                             Log(LogLevel.Error, "定时器过期", instance);

                         await Save(storage, instance, error, now, repeatDelay);
                     }
                     catch (Exception e)
                     {
                         Log(LogLevel.Error, "发生未捕获异常", instance, e);
                     }
                 }
             });
		}
		ICallPlanStorageAction CreateStorageAction(
            ICallPlanStorage storage, 
            ICallInstance timer, 
            Exception error, 
            DateTime now,
            int? repeatDelay
            )
		{
			//定时器已超时
			if (timer.Expire < now)
			{
				//结束定时器
				return storage.CreateExpiredAction(
					timer,
					now,
					error?.ToString()
					);
			}
			//成功结束
			if (error == null && repeatDelay==null)
				return storage.CreateSuccessAction(timer);
			else
			{
				//发生异常，但未超时
				return storage.CreateRetryAction(
						timer,
						ConstantTimes.InitTime
							.Add(timer.CallTime.Subtract(ConstantTimes.ExecutingStartTime))
							.AddSeconds(repeatDelay??timer.DelaySecondsOnError),
						false,
						error?.ToString(),
						error is RepeatCallException repeat? repeat.NewCallArgument:null
						);
			}
		}
		async Task Save(ICallPlanStorage storage, ICallInstance timer, Exception error, DateTime now,int? repeatDelay)
		{
			var action = CreateStorageAction(storage,timer, error, now, repeatDelay);
			await storage.ExecuteActions(new[] { action });
		}
		
	}
}
