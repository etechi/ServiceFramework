using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Data.Storage;

namespace SF.Core.CallGuarantors.Storage
{
	public class CallGuarantorStorage :
		ICallGuarantorStorage
	{
		public IDataSet<DataModels.CallInstance> CallInstances { get; }
		public Lazy<IDataSet<DataModels.CallExpired>> CallExpireds { get; }

		public CallGuarantorStorage(
			IDataSet<DataModels.CallInstance> CallInstances,
			Lazy<IDataSet<DataModels.CallExpired>> CallExpireds
			)
		{
			this.CallInstances = CallInstances;
			this.CallExpireds = CallExpireds;
		}
		public async Task<bool> Create(
			string Callable,
			string Argument,
			string Error,
			string Title, 
			DateTime Now, 
			DateTime CallTime, 
			DateTime ExpireTime, 
			int DelaySecondsOnError
			)
		{
            try
            {
                this.CallInstances.Add(new DataModels.CallInstance
                {
                    Callable = Callable,
                    CallArgument = Argument,
                    CallError = Error.Limit(200),
                    Title = Title,
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
        public async Task Remove(string Callable)
        {
            var c = await CallInstances.FindAsync(Callable);
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
		class ExpiredAction : BaseAction, ICallStorageAction
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
					Callable = Instance.Callable,
					ExecError = ExecError?.Limit(200)??Instance.ExecError,
					ExecCount = ExecError==null?Instance.ErrorCount: Instance.ErrorCount + 1,
					Expired = Now,
					Title = Instance.Title,
					LastExecTime = Instance.LastExecTime,
					CreateTime = Instance.CreateTime,
					CallError=Instance.CallError,
					CallArgument=Instance.CallArgument
				});
			}
		}
		class RetryAction : BaseAction, ICallStorageAction
		{
			public DateTime NewTarget { get; set; }
			public bool Expired { get; set; }
			public string ExecError { get; set; }
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
				CallInstances.Update(Instance);
			}
		}
		class SuccessAction : BaseAction, ICallStorageAction
		{
			public override void Update(
				IDataSet<DataModels.CallInstance> CallInstances,
				Lazy<IDataSet<DataModels.CallExpired>> CallExpireds
				)
			{
				CallInstances.Remove(Instance);
			}
		}
		public ICallStorageAction CreateExpiredAction(ICallInstance Instance, DateTime Now, string Error)
		{
			return new ExpiredAction
			{
				ExecError = Error,
				Now = Now,
				Instance =(DataModels.CallInstance) Instance
			};
		}

		public ICallStorageAction CreateRetryAction(ICallInstance Instance, DateTime NewTarget, bool Expired, string Error)
		{
			return new RetryAction
			{
				Instance = (DataModels.CallInstance)Instance,
				NewTarget = NewTarget,
				Expired = Expired,
				ExecError = Error
			};
		}

		public ICallStorageAction CreateSuccessAction(ICallInstance Instance)
		{
			return new SuccessAction
			{
				Instance = (DataModels.CallInstance)Instance
			};
		}

		public async Task ExecuteActions(IEnumerable<ICallStorageAction> Actions)
		{
			foreach (var action in Actions)
				((BaseAction)action).Update(
					CallInstances,
					CallExpireds
					);
			await CallInstances.Context.SaveChangesAsync();
		}
		public async Task<ICallInstance> GetInstance(string Id)
		{
			return await CallInstances.FindAsync(Id);
		}
		public async Task<ICallInstance[]> GetInstancesForCleanup(DateTime ExecutingStartTime)
		{
			var timers = await CallInstances.LoadListAsync(t => t.CallTime > ExecutingStartTime);
			return timers;
		}

		public async Task<string[]> GetOnTimeInstances(int Count,DateTime Now, DateTime ExecutingStartTime, DateTime InitTime)
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
			return instances.Select(t=>t.Callable).ToArray();
		}
	}
}
