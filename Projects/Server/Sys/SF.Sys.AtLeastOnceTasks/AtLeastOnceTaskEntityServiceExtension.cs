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
	public class AtLeastOnceTaskTaskIdent<TKey,TSyncKey> 
		where TKey : IEquatable<TKey>
	{
		public TKey Id { get; set; }
		public TSyncKey SyncKey { get; set; }
		public DateTime TaskNextTryTime { get; set; }
	}
	public class AtLeastOnceTaskEntityServiceSetting<TKey, TSyncKey, TEntity>
		where TKey : IEquatable<TKey>
	{
		public int ErrorDelayUnit { get; set; } = 10;
		public int ExecTimeoutSeconds { get; set; } = 0;
		public ISyncQueue<TSyncKey> SyncQueue { get; set; }
		public Expression<Func<TEntity, AtLeastOnceTaskTaskIdent<TKey, TSyncKey>>> TaskIdentSelector { get; set; }
		public Func<IServiceProvider,TEntity,Task<DateTime?>> TaskExecutor { get; set; }
	}

	public static class AtLeastOnceTaskEntityService
	{
		class TaskSoruce<TKey, TEntity, TSyncKey> : ITimedTaskSoruce
			where TKey : IEquatable<TKey>
			where TEntity : class, IAtLeastOnceTask
		{
			ITimedTaskExecutor TimedTaskExecutor { get; }
			AtLeastOnceTaskEntityServiceSetting<TKey, TSyncKey, TEntity> Setting { get; }
			IServiceScopeFactory ScopeFactory { get; }
			public TaskSoruce(
				AtLeastOnceTaskEntityServiceSetting<TKey, TSyncKey, TEntity> Setting,
				ITimedTaskExecutor TimedTaskExecutor,
				IServiceScopeFactory ScopeFactory
				)
			{
				this.TimedTaskExecutor = TimedTaskExecutor;
				this.Setting = Setting;
				this.ScopeFactory = ScopeFactory;
			}

			public Task LoadTimedTasks(
				DateTime EndTargetTime, 
				CancellationToken cancellactionToken
				)
			{
				return ScopeFactory.WithScope(async sp =>
				{
					var query = sp.Resolve<IDataContext>()
					.Set<TEntity>()
					.AsQueryable()
					.Where(e =>
						e.TaskState == AtLeastOnceTaskState.Waiting &&
						e.TaskNextTryTime < EndTargetTime
						)
					.Select(Setting.TaskIdentSelector)
					;
					var tasks = await query.ToArrayAsync();
					foreach (var task in tasks)
						TimedTaskExecutor.Enqueue(
							task.Id,
							task.TaskNextTryTime,
							ct => RunTask(task.Id, task.SyncKey)
							);
				});
			}

			Task RunTask(TKey Id, TSyncKey SyncKey)
			{
				if (Setting.SyncQueue == null)
					return ScopeFactory.WithScope(
						sp => 
							RunTask(sp, Id, SyncKey)
						);
				else
					return Setting.SyncQueue.Queue(
						SyncKey,
						()=>
							ScopeFactory.WithScope(sp => 
								RunTask(sp, Id, SyncKey)
							)
						);
			}
			async Task RunTask(IServiceProvider sp, TKey Id, TSyncKey SyncKey)
			{ 
				var timeService = sp.Resolve<ITimeService>();
				var ctx = sp.Resolve<IDataContext>();
				var task = await ctx.Set<TEntity>().FindAsync(Id);
				if (task == null)
					return;
				DateTime? delayed = null;
				Exception error = null;
				var now = timeService.Now;
				try
				{
					delayed = await Setting.TaskExecutor(sp, task);
				}
				catch (Exception ex)
				{
					error = ex;
				}
				if (error != null)
				{
					if (
						Setting.ExecTimeoutSeconds > 0 &&
						now.Subtract(task.TaskStartTime.Value).TotalSeconds > Setting.ExecTimeoutSeconds
						)
						task.TaskState = AtLeastOnceTaskState.Failed;
					else
					{
						task.TaskState = AtLeastOnceTaskState.Waiting;
						task.TaskNextTryTime = timeService.Now.AddSeconds(
							Setting.ErrorDelayUnit * (1 << (task.TaskTryCount - 1))
							);
					}
					task.TaskLastError = error.Message;
				}
				else if (delayed == null)
				{
					task.TaskState = AtLeastOnceTaskState.Completed;
					task.TaskLastError = null;
				}
				else
				{
					task.TaskState = AtLeastOnceTaskState.Waiting;
					task.TaskNextTryTime = delayed.Value;
					task.TaskLastError = null;
				}

				task.TaskLastTryTime = now;
				task.TaskTryCount++;
				ctx.Update(task);
				await ctx.SaveChangesAsync();
				if (task.TaskState == AtLeastOnceTaskState.Waiting)
				{
					TimedTaskExecutor.Enqueue(
						Id,
						task.TaskNextTryTime.Value,
						ct => RunTask(Id, SyncKey)
						);
				}
			}
			
			public Task Startup(CancellationToken cancellationToken)
			{
				return ScopeFactory.WithScope(async sp =>
				{
					var ctx = sp.Resolve<IDataContext>();
					var tasks = await ctx.Set<TEntity>().AsQueryable()
						.Where(e => e.TaskState == AtLeastOnceTaskState.Running)
						.ToArrayAsync();
					var timeService = sp.Resolve<ITimeService>();
					var now = timeService.Now;
					foreach (var t in tasks)
					{
						t.TaskState = AtLeastOnceTaskState.Waiting;
						t.TaskNextTryTime = now.AddSeconds(Setting.ErrorDelayUnit * (1 << t.TaskTryCount));
						t.TaskLastError = "异常终止";
						ctx.Update(t);
					}
					await ctx.SaveChangesAsync();
				});
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
			sc.AddSingleton<ITimedTaskSoruce>(sp => 
				new TaskSoruce<TKey, TEntity, TSyncKey>(
					Setting,
					sp.Resolve<ITimedTaskExecutor>(),
					sp.Resolve<IServiceScopeFactory>()
				)
			);
			return sc;
		}
			//public static IServiceCollection AddAtLeastOnceEntityTaskService<TKey, TSyncKey, TEntity>(
			//	this IServiceCollection sc,
			//	AtLeastOnceActionEntityServiceSetting<TKey, TSyncKey, TEntity> Setting
			//	)
			//	where TKey : IEquatable<TKey>
			//	where TSyncKey: IEquatable<TSyncKey>
			//	where TEntity : class, IAtLeastOnceTask
			//{



			//	sc.AddAtLeastOnceTaskService(
			//		new AtLeastOnceActionServiceSetting<TKey, TSyncKey, TEntity>
			//		{
			//			Name = Setting.Name,
			//			GetSyncKey=Setting.GetSyncKey,
			//			BatchCount = Setting.BatchCount,
			//			ThreadCount = Setting.ThreadCount,
			//			Interval = Setting.Interval,
			//			ErrorDelayUnit = Setting.ErrorDelayUnit,
			//			ExecTimeoutSeconds = Setting.ExecTimeoutSeconds,
			//			Init=Setting.Init,
			//			GetIdentsToRunning = async (isp, count, time) =>
			//			  {
			//				  var set = isp.Resolve<IDataSet<TEntity>>();
			//				  var tasks = await set.AsQueryable()
			//					  .Where(e => e.TaskState == AtLeastOnceTaskState.Waiting && e.TaskNextTryTime <= time)
			//					  .OrderBy(e=>e.TaskNextTryTime)
			//					  .Take(count)
			//					  .ToArrayAsync();
			//				  foreach (var t in tasks)
			//				  {
			//					  t.TaskState = AtLeastOnceTaskState.Running;
			//					  set.Update(t);
			//				  }
			//				  await set.Context.SaveChangesAsync();
			//				  var ids = tasks.Select(Setting.GetKey).ToArray();
			//				  return ids;
			//			  },
			//			LoadRunningTasks = async (isp) =>
			//			  {
			//				  var set = isp.Resolve<IDataSet<TEntity>>();
			//				  var re = await set.AsQueryable()
			//					  .Where(e => e.TaskState == AtLeastOnceTaskState.Running)
			//					  .ToArrayAsync();
			//				  return re;
			//			  },
			//			LoadTask = async (isp, id) =>
			//			{
			//				var set = isp.Resolve<IDataSet<TEntity>>();
			//				var re = await set.AsQueryable()
			//					.Where(e => e.TaskState == AtLeastOnceTaskState.Running)
			//					.Where(Setting.KeyEqual(id))
			//					.SingleOrDefaultAsync();
			//				return re;
			//			},
			//			RunTask = Setting.RunTask,
			//			SaveTask = async (isp, task) =>
			//			{
			//				var set = isp.Resolve<IDataSet<TEntity>>();
			//				set.Update(task);
			//				await set.Context.SaveChangesAsync();
			//			},
			//			UseDataScope = async (isp, action) =>
			//			 {
			//				 var ctx = isp.Resolve<IDataContext>();
			//				 await ctx.UseTransaction(
			//					 Setting.Name,
			//					 async ts => {
			//						 await action();
			//						 return 0;
			//					 }
			//					 );
			//			 }
			//		}
			//		);
			//	return sc;
			//}
		}
}
