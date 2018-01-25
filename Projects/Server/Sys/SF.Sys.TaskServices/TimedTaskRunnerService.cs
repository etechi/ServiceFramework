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
using SF.Sys.TaskServices;
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
		IDisposable Enqueue<TKey>(
			TKey Key,
			DateTime Target, 
			Func<CancellationToken,Task> Task,
			bool IgnoreLongWaitTask=true
			);
	}
	
	public interface ITimedTaskSoruce
	{
		Task Startup(CancellationToken cancellationToken);
		Task LoadTimedTasks(
			DateTime EndTargetTime, 
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
	}

	public class TimedTaskExecutor : ITimedTaskExecutor
	{
		class TypedTask<TKey>:SF.Sys.TaskServices.Timer,IDisposable
		{
			public static CancellationToken CancelledCancellationToken { get; }
			static TypedTask()
			{
				var tcs = new CancellationTokenSource();
				tcs.Cancel();
				CancelledCancellationToken = tcs.Token;
			}
			public TypedTaskDict<TKey> Dict { get; set; }
			public Func<CancellationToken, Task> Callback { get; set; }
			public TKey Key { get; }
			public TypedTask(TypedTaskDict<TKey> Dict, TKey Key)
			{
				this.Dict = Dict;
				this.Key = Key;
			}
			async Task Run(CancellationToken ct)
			{
				Func<CancellationToken, Task> Callback;
				lock (this)
				{
					if (Dict == null)
						return;
					if (ct.IsCancellationRequested)
						Dict.TimerService.Remove(this);

					Callback = this.Callback;
					Dict.TryRemove(Key, out var cur);

					Dict = null;
					this.Callback = null;
				}
				if (Callback == null)
					return;

				try
				{
					await Callback(ct);
				}
				catch (Exception ex)
				{

				}
			}
			public override void OnTimer()
			{
				Task.Run(() =>Run(CancellationToken.None));
			}
			public override void OnCancelled()
			{
				Task.Run(() => Run(CancelledCancellationToken));
			}

			public void Dispose()
			{
				OnCancelled();
			}

		}

		//字典中保存的是为运行的任务，一旦运行，从字典中删除
		class TypedTaskDict<TKey>:
			ConcurrentDictionary<TKey, TypedTask<TKey>>
		{
			public ITimeService TimeService { get; }
			public ITimerService TimerService { get; }
			public TypedTaskDict(ITimeService TimeService, ITimerService TimerService)
			{
				this.TimeService = TimeService;
				this.TimerService = TimerService;
			}

		}
		ConcurrentDictionary<Type, object> ItemCategoryDict { get; } =
				   new ConcurrentDictionary<Type, object>();

		ITimerService TimerService { get; }
		ITimeService TimeService { get; }
		TimedTaskRunnerSetting Setting { get; }
		public TimedTaskExecutor(ITimerService TimerService, ITimeService TimeService, TimedTaskRunnerSetting Setting)
		{
			this.TimerService = TimerService;
			this.TimeService = TimeService;
			this.Setting = Setting;
		}
		TypedTaskDict<TKey> GetCategoryDict<TKey>()
		{
			var type = typeof(TKey);
			if (!ItemCategoryDict.TryGetValue(type, out var dict))
				dict = ItemCategoryDict.GetOrAdd(
					type,
					new TypedTaskDict<TKey>(TimeService, TimerService)
					);
			return (TypedTaskDict<TKey>)dict;
		}
		public IDisposable Enqueue<TKey>(
			TKey Key, 
			DateTime Target, 
			Func<CancellationToken, Task> Callback,
			bool IgnoreLongWaitTask=true
			)
		{
			//超出载入时间，可以忽略
			if (IgnoreLongWaitTask && 
				Target.Subtract(TimeService.Now).TotalSeconds >
				Setting.PreloadIntervalSeconds + Setting.PreloadAheadSeconds)
				return null;

			var dict = GetCategoryDict<TKey>();
			
			for (; ; )
			{
				if (!dict.TryGetValue(Key, out var item))
					item = dict.GetOrAdd(Key, new TypedTask<TKey>(dict, Key));

				Func<CancellationToken, Task> OrgCallback=null;
				try
				{
					lock (item)
					{
						//在从字典中获取后，刚好遇上定时器执行，已经从字典中删除，需要重新获取
						if (item.Dict == null)
							continue;

						OrgCallback = item.Callback;
						item.Callback = Callback;
						var offset = (int)Target.Subtract(TimeService.Now).TotalMilliseconds;
						if (offset < 0) offset = 0;
						TimerService.Update(offset, item);
					}
					return item;
				}
				finally
				{
					if (OrgCallback != null)
					{
						Task.Run(async () =>
						{
							try
							{
								await OrgCallback(TypedTask<TKey>.CancelledCancellationToken);
							}
							catch { }
						});
					}
				}
			}
		}
	}

	public static class TimedTaskRunnerExtension
	{
		
		public static IServiceCollection AddTimedTaskRunnerService(
			this IServiceCollection sc,
			TimedTaskRunnerSetting Setting
			)
		{
			sc.AddSingleton<ITimedTaskExecutor>(sp => 
				new TimedTaskExecutor(
				sp.Resolve<ITimerService>(),
				sp.Resolve<ITimeService>(),
				Setting
				));

			sc.AddTaskService(
				Setting.Name,
				sp=>Task.CompletedTask,
				async (sp, ss, cancelToken) =>
				{
					var sources = sp.Resolve<IEnumerable<ITimedTaskSoruce>>();

					foreach (var src in sources)
						await src.Startup(cancelToken);

					var timeService = sp.Resolve<ITimeService>();

					var reloadTarget = timeService.Now;
					for (; ; )
					{
						var now = timeService.Now;
						if (reloadTarget > now)
							await Task.Delay(reloadTarget.Subtract(now), cancelToken);
						reloadTarget = now.AddSeconds(
							Setting.PreloadIntervalSeconds
							);
						var end = reloadTarget.AddSeconds(Setting.PreloadAheadSeconds);
						foreach (var src in sources)
						{
							try
							{
								await src.LoadTimedTasks(
									end,
									cancelToken
									);
							}
							catch(Exception ex)
							{

							}
						}
					}
				},
				Setting.AutoStartup
			);
			return sc;
		}
	}
}
