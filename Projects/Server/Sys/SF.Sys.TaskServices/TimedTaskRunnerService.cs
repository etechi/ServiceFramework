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

using SF.Sys.ADT;
using SF.Sys.Services;
using SF.Sys.Threading;
using SF.Sys.TimeServices;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Sys.Services
{
	
	public interface ITimedTaskExecutor
	{
		Task Enqueue(IEnumerable<KeyValuePair<DateTime, Func<CancellationToken, Task>>> Tasks);
	}
	
	
	public interface ITimedTaskSoruce
	{
		Task Startup(CancellationToken cancellationToken);
		Task<bool> LoadTimedTasks(
			DateTime EndTargetTime, 
			int MaxCount, 
			CancellationToken cancellactionToken
			);
	}
	public class TimedTaskRunnerSetting
	{
		public string Name { get; set; }
		public bool AutoStartup { get; set; } = true;
		/// <summary>
		/// 任务载入间隔，
		/// |------------|-------------|-------------|
		/// |------------|-----|
		///     interval |-------------------|
		///               ahead        |------------------|
		/// </summary>
		public int PreloadIntervalSeconds { get; set; } = 60;
		public int PreloadAheadSeconds { get; set; } = 10;
		public int LoadBatchCount { get; set; } = 100;
	}
	public static class TimedTaskRunnerExtension
	{
	
		enum WaitResult
		{
			Timeout,
			TimerChanged,
			Exit

		}


		class Timer : ADT.Timer<DateTime, Func<CancellationToken,Task>>
		{
			static void WaitTimeout(TimeSpan span, TaskCompletionSource<WaitResult> tcs)
			{
				Task.Delay(span)
					.ContinueWith(t => tcs.TrySetResult(WaitResult.Timeout));
			}
			TaskCompletionSource<WaitResult> CurCompletionSource { get; set; }
			public void Enqueue(IEnumerable<KeyValuePair<DateTime, Func<CancellationToken, Task>>> Callbacks)
			{
				Func<CancellationToken, Task> func
					;
				func.Invoke()
				TaskCompletionSource<WaitResult> tcs;
				lock (this)
				{
					foreach(var pair in Callbacks)
						base.Enqueue(pair.Key, pair.Value);
					tcs = CurCompletionSource;
				}
				if (tcs!= null)
					tcs.TrySetResult(WaitResult.TimerChanged);
			}
			public void RemoveTaskItem(base.Item Item)
			{
				TaskCompletionSource<WaitResult> tcs;
				lock (this)
				{
					base.Remove(Item);
					tcs = CurCompletionSource;
				}
				if (tcs != null)
					tcs.TrySetResult(WaitResult.TimerChanged);
			}
			public async Task<IEnumerable<TaskItem>> Wait(ITimeService timeService, CancellationToken token)
			{
				for (; ; )
				{
					if (token.IsCancellationRequested)
						return null;
					DateTime target = default;
					var now = timeService.Now;
					var waitResult = WaitResult.Timeout;
					TaskCompletionSource<WaitResult> tcs;
					lock (this)
					{
						if (KeyCount > 0)
							target = Peek().Key;
						if (target == default && target > now)
							CurCompletionSource = tcs = new TaskCompletionSource<WaitResult>();
						else
							CurCompletionSource = tcs = null;
					}

					if (tcs != null)
					{
						if (target != default)
							WaitTimeout(target.Subtract(now), tcs);
						using (token.Register(() => tcs.SetResult(WaitResult.Exit)))
						{
							waitResult = await tcs.Task;
						}
					}

					if (waitResult == WaitResult.Exit)
						return null;
					else if (waitResult == WaitResult.TimerChanged)
						continue;

					IEnumerable<Item> items;
					lock (this)
					{
						items = Dequeue().Value;
					}

					return items.Select(i => i.Value);
				}
			}
			public TaskCompletionSource<WaitResult> WaitTaskCompletionSource { get; set; }
		}

		class TaskCollection
		{
			ConcurrentDictionary<Type, object> ItemCategoryDict { get; } =
				new ConcurrentDictionary<Type, object>();

			ConcurrentDictionary<T,TaskItemWithIdent<T>> GetCategoryDict<T>()
			{
				var type = typeof(T);
				if (!ItemCategoryDict.TryGetValue(type, out var dict))
					dict=ItemCategoryDict.GetOrAdd(
						type, 
						new ConcurrentDictionary<T, TaskItemWithIdent<T>>()
						);
				return (ConcurrentDictionary<T, TaskItemWithIdent<T>>)dict;
			}

			public void Add(TaskCallback Callback)
			{
				if (Task == null)
					throw new ArgumentNullException(nameof(Task));

				if (IdDict.TryGetValue(Task.Ident, out var item))
				{
					var OrgTask = item.Task;
					item.Task = Task;

					//如果有旧任务
					if (OrgTask != null)
					{
						//如果没有在运行，且和新任务时间不同，需要删除定时器
						if (item.IsRunning && OrgTask.Target != Task.Target)
							RemoveFromTaskDict(item);
						OrgTask.Cancel();
					}


					//假如没有在运行，没有旧任务或者两者时间不同，需要重新插入定时器
					if (item.IsRunning && (OrgTask == null || OrgTask.Target != Task.Target))
						AddToTimeDict(item);
				}
				else
				{
					item = new TaskItem
					{
						Ident=Task.Ident,
						Task = Task
					};
					IdDict[Task.Ident] = item;

					//新任务总是加入定时器
					AddToTimeDict(item);
				}
			}
			public bool Remove(string Ident)
			{
				if (!IdDict.TryGetValue(Ident, out var item))
					return false;

				var orgItem = item.Task;
				item.Task = null;
				if (orgItem != null)
					orgItem.Cancel();

				//如果已经在运行
				if (item.IsRunning)
					return true;

				RemoveFromTaskDict(item);
				IdDict.Remove(Ident);
				return true;
			}
			public DateTime GetNextTargetTime()
			{
				foreach (var pair in TimeDict)
					return TargetOffset.AddSeconds(pair.Key);
				return DateTime.MaxValue;
			}
			static IEnumerable<TaskItem> GetItems(TaskItem Head)
			{
				var end = Head;
				do
				{
					yield return Head;
					Head = Head.TimerListNext;
				}
				while (Head != end) ;
			}
			public void TaskComplete(TaskItem item)
			{
				//运行期间没有新任务，直接删除
				if(item.Task==null)
				{
					item.TCS.SetResult(0);
					IdDict.Remove(item.Ident);
				}
				//运行期间有新任务，重新加入定时器
				else
				{
					AddToTimeDict(item);
				}
			}
			async Task RunTask(TaskItem item,CancellationToken cancelTask)
			{
				try
				{
					await item.Task.Execute(cancelTask);
				}
				catch {
				}
				finally
				{
					TaskComplete(item);
				}
			}
			public void RunTasks(DateTime now,CancellationToken cancelTask)
			{
				var LastIndex = (int)now.Subtract(TargetOffset).TotalSeconds;
				KeyValuePair<int, TaskItem>[] Lists;
				Lists = TimeDict.TakeWhile(i => i.Key <= LastIndex).ToArray();
				foreach (var list in Lists)
				{
					TimeDict.Remove(list.Key);
					foreach (var n in GetItems(list.Value))
					{
						if (n.TCS == null)
							n.TCS = new TaskCompletionSource<int>();
						Task.Run(() =>
							RunTask(n, cancelTask)
							);
					}
				}
			}

		}


		class TimedTaskRunner : ITimedTaskExecutor
		{
			public TaskCollection Collection { get; }
			public TimedTaskRunnerSetting Setting { get; }
			public ITimeService TimeService { get; }

			DateTime NextLoadTime;

			public TimedTaskRunner(
				TimedTaskRunnerSetting Setting, 
				TaskCollection Collection,
				ITimeService TimeService
				)
			{
				this.TimeService = TimeService;
				this.Collection = Collection;
				this.Setting = Setting;
			}
			public bool Execute(ITimedTask Task)
			{
				if (Task.Target > NextLoadTime)
					return false;
				lock (Collection)
					Collection.Add(Task);
				return true;
			}

			public bool Remove(string Task)
			{
				return Collection.Remove(Task);
			}
		}

		
		static async Task LoadTasks(
			TaskCollection collection,
			IServiceScopeFactory scopeFactory,
			ITimeService timeService,
			CancellationToken cancelToken,
			int PreloadIntervalSeconds,
			int PreloadAheadSeconds,
			int LoadBatchCount
			)
		{
			var reloadTarget = timeService.Now;
			for (; ; )
			{
				var now = timeService.Now;
				if (reloadTarget > now)
					await Task.Delay(reloadTarget.Subtract(now), cancelToken);
				reloadTarget = now.AddSeconds(
					PreloadIntervalSeconds
					);
				var end = reloadTarget.AddSeconds(PreloadAheadSeconds);
				await scopeFactory.WithScope(async isp => {
					foreach (var src in isp.Resolve<IEnumerable<ITimedTaskSoruce>>())
					{
						for (; ; )
						{
							var hasMoreTasks = await src.LoadTimedTasks(
								end, 
								LoadBatchCount, 
								cancelToken
								);
							if (!hasMoreTasks)
								break;
						}
					}
				});
			}
		}


		static async Task RunTasks(
			Timer timer,
			ITimeService timeService,
			CancellationToken cancelToken
			)
		{
			while (!cancelToken.IsCancellationRequested)
			{
				var items=await timer.Wait(timeService, cancelToken);
				if (items == null)
					break;
				collection.RunTasks(now, cancelToken);
			}
		}

		static async Task SourceStartup(
			IServiceScopeFactory scopeFactory,
			CancellationToken cancelToken
			)
		{
			await scopeFactory.WithScope(async isp =>
			{
				var sources = isp.Resolve<IEnumerable<ITimedTaskSoruce>>().ToArray();
				foreach (var src in sources)
					await src.Startup(cancelToken);
			});

		}

		public static IServiceCollection AddTimedTaskRunnerService(
			this IServiceCollection sc,
			TimedTaskRunnerSetting Setting
			)
		{
			var collection = new TaskCollection();
			sc.AddSingleton<ITimedTaskExecutor>(sp =>
				new TimedTaskRunner(
					Setting,
					collection,
					sp.Resolve<ITimeService>()
					)
				);


			sc.AddTaskService(
				Setting.Name,
				sp=>Task.CompletedTask,
				async (sp, ss, cancelToken) =>
				{
					var scopeFactory = sp.Resolve<IServiceScopeFactory>();

					await SourceStartup(scopeFactory, cancelToken);

					var timeService = sp.Resolve<ITimeService>();

					var taskLoadTask = Task.Run(()=>
						LoadTasks(
							collection,
							scopeFactory,
							timeService,
							cancelToken,
							Setting.PreloadIntervalSeconds,
							Setting.PreloadAheadSeconds,
							Setting.LoadBatchCount
							)
						);

					var taskRunTask = Task.Run(()=>
						RunTasks(
							collection,
							timeService,
							cancelToken
							)
						);

					await Task.WhenAll(taskLoadTask, taskRunTask);
				},
				Setting.AutoStartup
			);
			return sc;
		}
	}
}
