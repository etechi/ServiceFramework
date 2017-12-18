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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Sys.Data;

namespace SF.Sys.Plans.Manager
{
	public class CallPlanStorage :
		ICallPlanStorage
	{
		public IDataSet<DataModels.CallInstance> CallInstances { get; }
		public Lazy<IDataSet<DataModels.CallExpired>> CallExpireds { get; }

		public CallPlanStorage(
			IDataSet<DataModels.CallInstance> CallInstances,
			Lazy<IDataSet<DataModels.CallExpired>> CallExpireds
			)
		{
			this.CallInstances = CallInstances;
			this.CallExpireds = CallExpireds;
		}
		public async Task<bool> Create(
			string Type,
			string Ident,
			string Argument,
			string Error,
			string Title, 
			DateTime Now, 
			DateTime CallTime, 
			DateTime ExpireTime, 
			int DelaySecondsOnError,
			long? ServiceScopeId
			)
		{
            try
            {
                this.CallInstances.Add(new DataModels.CallInstance
                {
                    Type = Type,
					Ident=Ident,
					ServiceScopeId=ServiceScopeId,
                    Argument = Argument,
                    Error = Error.Limit(200),
                    Name = Title,
                    CreateTime = Now,
                    Expire = ExpireTime,
                    DelaySecondsOnError = DelaySecondsOnError,
                    CallTime = CallTime
                });
                await CallInstances.Context.SaveChangesAsync();
                return true;
            }
            catch (DbDuplicatedKeyException)
            {
                //主键冲突时不抛出异常
                return false;
            }
		}
        public async Task Remove(string Type,string Ident)
        {
            var c = await CallInstances.FindAsync(Type,Ident);
            if (c == null) return;
			CallInstances.Remove(c);
            await CallInstances.Context.SaveChangesAsync();
        }

        abstract class BaseAction 
		{
			public DataModels.CallInstance Instance { get; set; }
			public abstract void Update(
				IDataSet<DataModels.CallInstance> CallInstances,
				Lazy<IDataSet<DataModels.CallExpired>> CallExpireds
				);
		}
		class ExpiredAction : BaseAction, ICallPlanStorageAction
		{
			public DateTime Now { get; set; }
			public string ExecError { get; set; }
			public override void Update(
				IDataSet<DataModels.CallInstance> CallInstances,
				Lazy<IDataSet<DataModels.CallExpired>> CallExpireds
				)
			{
				CallInstances.Remove(Instance);
				CallExpireds.Value.Add(new DataModels.CallExpired
				{
					Type = Instance.Type,
					Ident=Instance.Type,
					ExecError = ExecError?.Limit(200)??Instance.ExecError,
					ExecCount = ExecError==null?Instance.ErrorCount: Instance.ErrorCount + 1,
					Expired = Now,
					Title = Instance.Name,
					LastExecTime = Instance.LastExecTime,
					CreateTime = Instance.CreateTime,
					CallError=Instance.Error,
					CallArgument=Instance.Argument
				});
			}
		}
		class RetryAction : BaseAction, ICallPlanStorageAction
		{
			public DateTime NewTarget { get; set; }
			public bool Expired { get; set; }
			public string ExecError { get; set; }
			public string NewArgument { get; set; }
			public override void Update(
				IDataSet<DataModels.CallInstance> CallInstances,
				Lazy<IDataSet<DataModels.CallExpired>> CallExpireds
				)
			{
				if (ExecError != null)
				{
					Instance.ExecError = ExecError.Limit(200);
					Instance.ErrorCount++;
				}
				Instance.CallTime = NewTarget;
				if (NewArgument != null)
					Instance.Argument = NewArgument;
				CallInstances.Update(Instance);
			}
		}
		class SuccessAction : BaseAction, ICallPlanStorageAction
		{
			public override void Update(
				IDataSet<DataModels.CallInstance> CallInstances,
				Lazy<IDataSet<DataModels.CallExpired>> CallExpireds
				)
			{
				CallInstances.Remove(Instance);
			}
		}
		public ICallPlanStorageAction CreateExpiredAction(ICallInstance Instance, DateTime Now, string Error)
		{
			return new ExpiredAction
			{
				ExecError = Error,
				Now = Now,
				Instance =(DataModels.CallInstance) Instance
			};
		}

		public ICallPlanStorageAction CreateRetryAction(ICallInstance Instance, DateTime NewTarget, bool Expired, string Error,string NewArgument)
		{
			return new RetryAction
			{
				Instance = (DataModels.CallInstance)Instance,
				NewTarget = NewTarget,
				Expired = Expired,
				ExecError = Error,
				NewArgument= NewArgument
			};
		}

		public ICallPlanStorageAction CreateSuccessAction(ICallInstance Instance)
		{
			return new SuccessAction
			{
				Instance = (DataModels.CallInstance)Instance
			};
		}

		public async Task ExecuteActions(IEnumerable<ICallPlanStorageAction> Actions)
		{
			foreach (var action in Actions)
				((BaseAction)action).Update(
					CallInstances,
					CallExpireds
					);
			await CallInstances.Context.SaveChangesAsync();
		}
		public async Task<ICallInstance> GetInstance(string Type,string Ident)
		{
			return await CallInstances.FindAsync(Type,Ident);
		}
		public async Task<ICallInstance[]> GetInstancesForCleanup(DateTime ExecutingStartTime)
		{
			var timers = await CallInstances.LoadListAsync(t => t.CallTime > ExecutingStartTime);
			return timers;
		}

		public async Task<(string Type,string Ident)[]> GetOnTimeInstances(int Count,DateTime Now, DateTime ExecutingStartTime, DateTime InitTime)
		{
			var q = from t in CallInstances.AsQueryable(false)
					where t.CallTime <= Now
					orderby t.CallTime
					select t;

			var instances = await q.Take(Count).ToArrayAsync();
			//logger.trace("定时器", "执行定时器调度： {0} limit:{1} count:{2}", now, count, timers.Length);
			foreach (var t in instances)
			{
				t.LastExecTime = Now;
				t.CallTime = ExecutingStartTime.Add(t.CallTime.Subtract(InitTime));
				CallInstances.Update(t);
			}
			await CallInstances.Context.SaveChangesAsync();
			return instances.Select(t=>(t.Type,t.Ident)).ToArray();
		}
	}
}
