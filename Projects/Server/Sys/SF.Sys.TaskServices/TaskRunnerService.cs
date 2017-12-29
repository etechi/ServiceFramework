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

using SF.Sys.Services;
using SF.Sys.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Sys.Services
{
	public class TaskRunnerSetting<TTask,TSyncKey>
	{
		public string Name { get; set; }
		public Func<IServiceProvider, ISyncQueue<TSyncKey>, Task> Init { get; set; }
		public Func<IServiceProvider, int, Task<TTask[]>> GetTasks { get; set; }
		public Func<IServiceProvider, TTask, Task> RunTask { get; set; }
		public Func<TTask, TSyncKey> GetSyncKey { get; set; }
		public int BatchCount { get; set; } = 100;
		public int ThreadCount { get; set; } = 1000;
		public int Interval { get; set; } = 1000;
	}
	public static class TaskRunnerExtension
	{
		class Context<TTask, TSyncKey>
		{
			int _RunningTaskCount;
			public TaskRunnerSetting<TTask, TSyncKey> Setting { get; }
			public ISyncQueue<TSyncKey> ExecQueue { get; } 

			public Context (TaskRunnerSetting<TTask, TSyncKey> Setting)
			{
				this.Setting = Setting;
				ExecQueue = Setting.GetSyncKey==null ?
					NoneSyncQueue<TSyncKey>.Instance:
					new ObjectSyncQueue<TSyncKey>() 
					;
			}
			public async Task Init(IServiceProvider ServiceProvider)
			{
				await Setting.Init(ServiceProvider, ExecQueue);
			}
			public async Task StartTasks(IServiceProvider ServiceProvider)
			{
				for (; ; )
				{
					var count = Setting.ThreadCount - _RunningTaskCount;
					if (count <= 0)
						break;
					if (count >Setting.BatchCount)
						count = Setting.BatchCount;

					var ids = await Setting.GetTasks(ServiceProvider, Setting.BatchCount);
					if (ids.Length == 0)
						break;

					foreach (var id in ids)
					{
						Interlocked.Increment(ref _RunningTaskCount);
						try
						{
							RunTask(ServiceProvider,id);
						}
						catch
						{
							Interlocked.Decrement(ref _RunningTaskCount);
						}
					}
				}
			}

			void RunTask(
				IServiceProvider ServiceProvider,
				TTask task
				)
			{
				Task.Run(async () =>
				{
					try
					{
						if (Setting.GetSyncKey == null)
							await Setting.RunTask(ServiceProvider, task);
						else
							await ExecQueue.Queue(
								Setting.GetSyncKey(task),
								() =>
									Setting.RunTask(ServiceProvider, task)
							);
					}
					catch
					{
						Interlocked.Decrement(ref _RunningTaskCount);
					}
				});
			}
		}

		public static IServiceCollection AddTaskRunnerService<TKey,TTask>(
			this IServiceCollection sc,
			TaskRunnerSetting<TKey, TTask> Setting
			)
		{
			var ctx = new Context<TKey, TTask>(Setting);
			sc.AddTimerService(
				Setting.Name,
				Setting.Interval,
				async sp =>
				{
					await ctx.StartTasks(sp);
					return Setting.Interval;
				},
				async sp =>
					await ctx.Init(sp)
				);
			return sc;
		}
	}
}
