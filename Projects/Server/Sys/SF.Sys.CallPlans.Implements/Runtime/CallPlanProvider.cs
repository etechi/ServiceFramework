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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.CallPlans.Runtime
{
	public class CallPlanProvider :
		ICallPlanProvider
	{
		public ICallPlanStorage Storage { get; }
		public TimeServices.ITimeService TimeService { get; }
		public Lazy<ICallDispatcher> CallScheduler { get; }
		public CallableFactory CallableFactory { get; }
		public CallPlanProvider(
			ICallPlanStorage Storage,
			TimeServices.ITimeService TimeService, 
			Lazy<ICallDispatcher> Scheduler,
			CallableFactory CallableFactory
			)
		{
			this.Storage = Storage;
			this.TimeService = TimeService;
			this.CallScheduler = Scheduler;
			this.CallableFactory = CallableFactory;
		}

        public async Task<bool> Schedule(
            string CallableName,
            string CallContext,
            string CallArgument,
            Exception CallError,
            string Title,
            DateTime CallTime,
            int ExpireSeconds,
            int DelaySecondsOnError,
            bool SkipExecute = false,
			object CallData=null
			)
		{
			if (!CallableFactory.Exists(CallableName))
				throw new ArgumentException("找不到调用:" + CallableName);

			DelaySecondsOnError = Math.Max(DelaySecondsOnError, 5 * 60);

			string error = null;
			if (CallError != null)
			{
				var error_type = CallError.GetType();
				ExceptionFactory.VerifyErrorType(error_type);
				error = error_type.FullName + ":" + CallError.Message;
			}
            if (CallableName.Contains(':'))
                throw new ArgumentException("调用名中不允许包含':'");
			var id = CallableName + ":" + CallContext;
			var now = TimeService.Now;
			if (CallTime == DateTime.MinValue)
				CallTime = now;

			var immediately = false;
			if (CallTime == now)
			{
				immediately = true;
				CallTime = CallTime.AddSeconds(DelaySecondsOnError);
			}
			var re=await this.Storage.Create(
				id,
				CallArgument,
				error,
				Title,
				now,
				CallTime,
				ExpireSeconds > 0 ? CallTime.AddSeconds(ExpireSeconds) : ConstantTimes.NeverExpire,
				DelaySecondsOnError
			);
            if(re && immediately && !SkipExecute)
				await Execute(CallableName,CallContext,CallData);
            return true;
		}
        public async Task Cancel(
            string CallableName,
            string CallContext
            )
        {
            var id = CallableName + ":" + CallContext;
            await this.Storage.Remove(id);
        }
        public async Task Execute(
           string CallableName,
           string CallContext,
		   object CallData
           )
        {
            var id = CallableName + ":" + CallContext;
            await ExecuteDelayed(id,CallData);
        }
        async Task ExecuteDelayed(string id,object CallData)
        {
            try
            {
                await CallScheduler.Value.Execute(id,CallData);
            }
            catch { }
        }
    }
}
