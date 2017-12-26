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
using SF.Sys.AtLeastOnceTasks.Models;
using SF.Sys.Entities;
using SF.Sys.Services;
using SF.Sys.Services.Management;
using SF.Sys.Services.Management.Models;
using SF.Sys.Threading;
using SF.Sys.TimeServices;
using System;
using System.Threading.Tasks;

namespace SF.Sys.Services
{
	public class AtLeastOnceActionServiceSetting<TKey,TTask>
		where TTask : class
	{
		public string Name { get; set; }
		public int BatchCount { get; set; } = 100;
		public int ThreadCount { get; set; } = 100;
		public int Interval { get; set; } = 5000;
		public int ErrorDelayUnit { get; set; } = 10;
		public int ExecTimeoutSeconds { get; set; } = 0;
		public Func<IServiceProvider, ISyncQueue<TKey> ,Task> Init { get; set; }
		public Func<IServiceProvider, int, DateTime, Task<TKey[]>> GetIdentsToRunning { get; set; }
		public Func<IServiceProvider, Task<TTask[]>> LoadRunningTasks { get; set; }
		public Func<IServiceProvider,TTask,Task> SaveTask { get; set; }
		public Func<IServiceProvider,TTask,Task<DateTime?>> RunTask { get; set; }
		public Func<IServiceProvider, TKey, Task<TTask>> LoadTask { get; set; }
		public Func<IServiceProvider,Func<Task>,Task> UseDataScope { get; set; }
	}
	public static class AtLeastOnceTaskService
	{
		public static IServiceCollection AddAtLeastOnceTaskService<TKey, TTask>(
			this IServiceCollection sc,
			AtLeastOnceActionServiceSetting<TKey, TTask> Setting
			)
			where TKey : IEquatable<TKey>
			where TTask : class, IAtLeastOnceTask<TKey>
		{
			ITimeService timeService = null;
			sc.AddTaskRunnerService(new TaskRunnerSetting<TKey, TKey>
			{
				GetTaskKey=id=>id,
				Name = Setting.Name,
				ThreadCount = Setting.ThreadCount,
				BatchCount = Setting.BatchCount,
				Interval = Setting.Interval,
				SyncSampleIdentTask = true,
				Init = (sp,sq) =>
				{
					timeService = sp.Resolve<ITimeService>();
					return sp.WithScope(async isp =>
					{
						 if (Setting.Init != null)
							 await Setting.Init(isp, sq);

						 await Setting.UseDataScope(isp, async () =>
						 {
							 var tasks = await Setting.LoadRunningTasks(isp);
							 var now = timeService.Now;
							 foreach (var t in tasks)
							 {
								 t.TaskState = AtLeastOnceTaskState.Waiting;
								 t.TaskNextRunTime = now.AddSeconds(Setting.ErrorDelayUnit * (1 << t.TaskRunCount));
								 t.TaskLastRunError = "异常终止";
								 await Setting.SaveTask(isp, t);
							 }
						 });
					});
				},
				GetTasks = (sp, count) =>
					  Setting.GetIdentsToRunning(
						  sp,
						  count,
						  timeService.Now
					  ),
				RunTask = (sp, id) =>
					sp.WithScope(async isp =>
					{
						await Setting.UseDataScope(isp, async () =>
						{
							var task = await Setting.LoadTask(isp,id);
							DateTime? delayed = null;
							Exception error = null;
							var now = timeService.Now;
							try
							{
								delayed = await Setting.RunTask(isp, task);
							}
							catch (Exception ex)
							{
								error = ex;
							}
							if (error!=null)
							{
								if (
									Setting.ExecTimeoutSeconds > 0 &&
									now.Subtract(task.TaskStartTime.Value).TotalSeconds > Setting.ExecTimeoutSeconds
									)
									task.TaskState = AtLeastOnceTaskState.Failed;
								else
								{
									task.TaskState = AtLeastOnceTaskState.Waiting;
									task.TaskNextRunTime = timeService.Now.AddSeconds(
										Setting.ErrorDelayUnit * (1 << (task.TaskRunCount - 1))
										);
								}
								task.TaskLastRunError = error.Message;
							}
							else if (delayed == null)
							{
								task.TaskState = AtLeastOnceTaskState.Completed;
								task.TaskLastRunError = null;
							}
							else
							{
								task.TaskState = AtLeastOnceTaskState.Waiting;
								task.TaskNextRunTime = delayed.Value;
								task.TaskLastRunError = null;
							}

							task.TaskLastRunTime = now;
							task.TaskRunCount++;

							await Setting.SaveTask(isp, task);
						});
					}
					)
				}
			);
			return sc;
		}

	}
}
