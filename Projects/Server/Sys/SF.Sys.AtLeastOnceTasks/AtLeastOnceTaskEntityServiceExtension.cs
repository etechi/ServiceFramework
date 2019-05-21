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

using SF.Sys;
using SF.Sys.AtLeastOnceTasks;
using SF.Sys.AtLeastOnceTasks.Models;
using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys.Entities.DataModels;
using SF.Sys.Linq;
using SF.Sys.Services;
using SF.Sys.Services.Management;
using SF.Sys.Services.Management.Models;
using SF.Sys.Threading;
using SF.Sys.TimeServices;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Sys.Services
{
	
	//public class AtLeastOnceActionEntityServiceSetting<TKey, TSyncKey, TEntity> {
	//	public string Name { get; set; }
	//	public int BatchCount { get; set; } = 100;
	//	public int ThreadCount { get; set; } = 100;
	//	public int Interval { get; set; } = 5000;
	//	public int ErrorDelayUnit { get; set; } = 10;
	//	public int ExecTimeoutSeconds { get; set; } = 0;
	//	public Func<TEntity,TKey> GetKey { get; set; }
	//	public Func<TKey,Expression<Func<TEntity,bool>>> KeyEqual { get; set; }
	//	public Func<TKey,TSyncKey> GetSyncKey { get; set; }
	//	public Func<IServiceProvider, ISyncQueue<TSyncKey>, Task> Init { get; set; }
	//	public Func<IServiceProvider,TEntity, Task<DateTime?> > RunTask { get; set; }
	//}
	public class AtLeastOnceTaskSetting<TKey,TSyncKey> 
		where TKey : IEquatable<TKey>
	{
		public TKey Id { get; set; }
		public TSyncKey SyncKey { get; set; }
		public DateTime TaskNextTryTime { get; set; }
	}
	public class AtLeastOnceTaskExecuteResult
	{
		public string Message { get; set; }
		public DateTime? NextExecTime { get; set; }
	}
	public class AtLeastOnceTaskEntityServiceSetting<TKey, TSyncKey, TEntity>
		where TKey : IEquatable<TKey>
	{
		public ISyncQueue<TSyncKey> SyncQueue { get; set; }
		public int StartRescheduleDuetime { get; set; } = 5 * 60;
		public Expression<Func<TEntity, AtLeastOnceTaskSetting<TKey, TSyncKey>>> TaskSettingSelector { get; set; }
		public Func<IServiceProvider,TEntity,object,DateTime?,Task> TaskExecutor { get; set; }
	}

	public interface IAtLeastOnceTaskExecutor<TKey,TEntity,TSyncKey> 
		where TKey : IEquatable<TKey>
		where TEntity : class, IAtLeastOnceTask
	{
		Task Execute(TKey Id, TSyncKey SyncKey, object Argument,CancellationToken cancellationToken);
		IDisposable UpdateTimedTaskExecutor(TKey Id, TSyncKey SyncKey, DateTime Time);
		void RemoveTimedTaskExecutor(TKey Id);
	}
	public class ProgramAbortException : InvalidOperationException
	{
		public ProgramAbortException(string Message) :base(Message??"程序异常结束")
		{ }
	}
	public static class AtLeastOnceTaskEntityService
	{
		class TaskSoruce<TKey, TEntity, TSyncKey> : ITimedTaskSoruce, IAtLeastOnceTaskExecutor<TKey, TEntity, TSyncKey>
			where TKey : IEquatable<TKey>
			where TEntity : class, IAtLeastOnceTask
		{
			ITimedTaskService TimedTaskExecutor { get; }
			AtLeastOnceTaskEntityServiceSetting<TKey, TSyncKey, TEntity> Setting { get; }
			IServiceScopeFactory ScopeFactory { get; }
			public TaskSoruce(
				AtLeastOnceTaskEntityServiceSetting<TKey, TSyncKey, TEntity> Setting,
				ITimedTaskService TimedTaskExecutor,
				IServiceScopeFactory ScopeFactory
				)
			{
				this.TimedTaskExecutor = TimedTaskExecutor;
				this.Setting = Setting;
				this.ScopeFactory = ScopeFactory;
			}

			public async Task LoadTimedTasks(
				DateTime EndTargetTime, 
				CancellationToken cancellactionToken
				)
			{
				var tasks = await ScopeFactory.WithScope(sp =>
						sp.Resolve<IDataScope>().Use(
							"查找任务",
							ctx =>
							ctx.Queryable<TEntity>()
							.Where(e =>
								
								e.TaskState == AtLeastOnceTaskState.Waiting &&
								e.TaskNextExecTime < EndTargetTime
								)
							.Select(Setting.TaskSettingSelector)
							.ToArrayAsync()
						)
					);

				foreach (var task in tasks)
					TimedTaskExecutor.Update(
						(typeof(TEntity),task.Id),
						task.TaskNextTryTime,
						ct => Execute(task.Id, task.SyncKey,null,ct)
						);
			}

            public Task Execute(TKey Id, TSyncKey SyncKey,object Argument,CancellationToken cancelToken)
			{
				if (cancelToken.IsCancellationRequested)
					return Task.CompletedTask;

				if (Setting.SyncQueue == null)
					return ScopeFactory.WithScope(
						sp => 
							Execute(sp, Id, SyncKey, Argument)
						);
				else
					return Setting.SyncQueue.Queue(
						SyncKey,
						()=>
							ScopeFactory.WithScope(sp => 
								Execute(sp, Id, SyncKey, Argument)
							)
						);
			}
			async Task Execute(IServiceProvider sp, TKey Id, TSyncKey SyncKey,object Argument)
			{ 
				var timeService = sp.Resolve<ITimeService>();
				var dataScope = sp.Resolve<IDataScope>();
				var task = await dataScope.Use("载入任务",
						ctx => ctx.Set<TEntity>().FindAsync(Id)
						);
				if (task == null)
					return;
				Exception error = null;
				var now = timeService.Now;
				task.TaskLastExecTime = now;
				task.TaskExecCount++;
                var curNextExecTime = task.TaskNextExecTime;
				task.TaskNextExecTime = null;
				task.TaskState = AtLeastOnceTaskState.Running;
				try
				{
					await Setting.TaskExecutor(sp, task, Argument, curNextExecTime);
				}
				catch (Exception ex)
				{
					error = ex;
				}
				if (error != null)
				{
					task.TaskState = AtLeastOnceTaskState.Failed;
					task.TaskMessage = error.Message;
				}
				else
					task.TaskMessage = null;

				if (task.TaskState == AtLeastOnceTaskState.Waiting)
				{
					if (!task.TaskNextExecTime.HasValue)
					{
						task.TaskMessage = "再次执行任务没有指定下次执行时间";
						task.TaskState = AtLeastOnceTaskState.Failed;
					}
				}
				else if(task.TaskNextExecTime.HasValue)
				{ 
					task.TaskMessage = "必须要再次执行的任务指定了下次执行时间";
					task.TaskState = AtLeastOnceTaskState.Failed;
				}
				
				await dataScope.Use("载入任务", async ctx =>
				{
					if (task.TaskState == AtLeastOnceTaskState.Removing)
						ctx.Remove(task);
					else
						ctx.Update(task);
					await ctx.SaveChangesAsync();
				});
				if (task.TaskState == AtLeastOnceTaskState.Waiting)
				{
					TimedTaskExecutor.Update(
						(typeof(TEntity),Id),
						task.TaskNextExecTime.Value,
						ct => Execute(Id, SyncKey, null,ct)
						);
				}
			}
			
			public Task Startup(CancellationToken cancellationToken)
			{
				return ScopeFactory.WithScope(async sp =>
				{
					await sp.Resolve<IDataScope>().Use("清理异常任务", async ctx =>
					 {
						 var tasks = await ctx.Set<TEntity>().AsQueryable()
							 .Where(e => e.TaskState == AtLeastOnceTaskState.Running)
							 .ToArrayAsync();
						 var timeService = sp.Resolve<ITimeService>();
						 var now = timeService.Now;
						 var rand = new Random();
						 var error = new ProgramAbortException(null);
						 foreach (var t in tasks)
						 {
							t.TaskExecCount++;
							t.TaskLastExecTime = now.AddSeconds(rand.Next(Setting.StartRescheduleDuetime));
 							t.TaskState = AtLeastOnceTaskState.Waiting;
 							t.TaskMessage = "系统异常终止";
							ctx.Update(t);
						 }
						 await ctx.SaveChangesAsync();
					 });
				});
			}

			public IDisposable UpdateTimedTaskExecutor(TKey Id,TSyncKey SyncKey,DateTime Time)
			{
				return TimedTaskExecutor.Update(
					(typeof(TEntity), Id),
					Time,
					ct => Execute(Id, SyncKey, null, ct)
					);
			}
			public void RemoveTimedTaskExecutor(TKey Id)
			{
				TimedTaskExecutor.Update(
					(typeof(TEntity), Id),
					DateTime.MaxValue,
					null
					);
			}
		}
		public static IServiceCollection AddAtLeastOnceEntityTaskService<TKey, TEntity, TSyncKey>(
			this IServiceCollection sc,
			AtLeastOnceTaskEntityServiceSetting<TKey, TSyncKey, TEntity> Setting
			)
			where TKey : IEquatable<TKey>
			where TSyncKey : IEquatable<TSyncKey>
			where TEntity : class, IAtLeastOnceTask
		{

			sc.AddSingleton<IAtLeastOnceTaskExecutor<TKey, TEntity, TSyncKey>>(sp =>
				  new TaskSoruce<TKey, TEntity, TSyncKey>(
					  Setting,
					  sp.Resolve<ITimedTaskService>(),
					  sp.Resolve<IServiceScopeFactory>()
				  )
			);

			sc.AddSingleton(sp =>
				(ITimedTaskSoruce)sp.Resolve<IAtLeastOnceTaskExecutor<TKey, TEntity, TSyncKey>>()
			);
			return sc;
		}
			
	}
}
