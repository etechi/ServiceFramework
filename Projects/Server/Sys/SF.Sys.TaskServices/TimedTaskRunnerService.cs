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
using SF.Sys.Logging;
using SF.Sys.Services;
using SF.Sys.TaskServices;
using SF.Sys.Threading;
using SF.Sys.TimeServices;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Sys.Services
{

	public interface ITimedTaskService
	{

		IDisposable Update<TKey>(
			TKey Key,
			DateTime Target, 
			Func<CancellationToken,Task> Task
			);

		Task LoadTasks(
			CancellationToken cancellactionToken=default
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
		public string Name { get; set; } = "定时任务服务";
		public bool AutoStartup { get; set; } = true;
		/// <summary>
		/// 任务载入间隔，
		/// |------------|-------------|-------------|
		/// |------------|-----|
		///     interval |-------------------|
		///               ahead        |------------------|
		/// </summary>
		public int PreloadIntervalSeconds { get; set; } = 120;
		public int PreloadAheadSeconds { get; set; } = 30;
	}

	public class TimedTaskService : ITimedTaskService,IDisposable
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
			public override string ToString()
			{
				return $"任务:{typeof(TKey).Name} {Key}";
			}
			public TypedTask(TypedTaskDict<TKey> Dict, TKey Key)
			{
				this.Dict = Dict;
				this.Key = Key;
			}
			async Task Run(CancellationToken ct)
			{
				var executor = Dict?.Service;
				var logger = executor?.Logger;
				logger?.Debug($"任务定时器执行开始 {typeof(TKey).Name} {Key}");
				Func<CancellationToken, Task> cb;
				lock (this)
				{
					if (Dict == null)
						return;
					var svc = Dict.Service;
					if (svc.IsDisposed)
						return;
					if (ct.IsCancellationRequested)
						svc.TimerService.Remove(this);

					cb = this.Callback;
					Dict.TryRemove(Key, out var cur);

					Dict = null;
					this.Callback = null;
				}
				if (cb == null)
					return;

				try
				{
					await cb(ct);
					logger?.Debug($"任务定时器执行完成 {typeof(TKey).Name} {Key}");
				}
				catch (Exception ex)
				{
					logger?.Error(ex, $"任务定时器执行异常 {typeof(TKey).Name} {Key}");
				}
				executor?.OnTaskCompleted();
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
			public TimedTaskService Service { get; }
			public TypedTaskDict(TimedTaskService Service)
			{
				this.Service = Service;
				
			}

		}
		ConcurrentDictionary<Type, object> TypeDict { get; } =
				   new ConcurrentDictionary<Type, object>();

		ITimerService TimerService { get; }
		ITimeService TimeService { get; }
		TimedTaskRunnerSetting Setting { get; }
		ILogger Logger { get; }
		IEnumerable<ITimedTaskSoruce> Sources { get; }
		bool _Disposed;

		public bool IsDisposed => _Disposed;
		public void Dispose()
		{
			if (_Disposed) return;
			_Disposed=true;
			TypeDict.Clear();
		}
		public TimedTaskService(
			ITimerService TimerService, 
			ITimeService TimeService,
			ILogger<TimedTaskService> Logger,
			IEnumerable<ITimedTaskSoruce> Sources,
			TimedTaskRunnerSetting Setting
			)
		{
			this.Sources = Sources;
			this.TimerService = TimerService;
			this.TimeService = TimeService;
			this.Setting = Setting;
			this.Logger = Logger;
		}
		
		TypedTaskDict<TKey> GetCategoryDict<TKey>()
		{
			var type = typeof(TKey);
			if (!TypeDict.TryGetValue(type, out var dict))
				dict = TypeDict.GetOrAdd(
					type,
					new TypedTaskDict<TKey>(this)
					);
			return (TypedTaskDict<TKey>)dict;
		}
		[Conditional("Debug")]
		void DebugTrace(string Message)
		{
			Logger.Debug(Message);
		}
		public IDisposable Update<TKey>(
			TKey Key, 
			DateTime Target, 
			Func<CancellationToken, Task> Callback
			)
		{
			DebugTrace($"任务定时器更新开始 {typeof(TKey).Name} {Key} {Target}");
			//超出载入时间，可以忽略
			var loadRequired =
				Target.Subtract(TimeService.Now).TotalSeconds <=
				Setting.PreloadIntervalSeconds + Setting.PreloadAheadSeconds;

			var dict = GetCategoryDict<TKey>();
			IDisposable result = null;
			for (; ; )
			{
				if (!dict.TryGetValue(Key, out var item))
				{
					if (!loadRequired)
						break;
					item = dict.GetOrAdd(Key, new TypedTask<TKey>(dict, Key));
				}
				else if (Callback == null || !loadRequired)
				{
					item.Dispose();
					break;
				}

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
						var offset = (int)Target.Subtract(TimeService.Now).TotalSeconds;
						if (offset < 0) offset = 0;
						TimerService.Update(offset, item);
					}
					result=item;
					break;
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
			DebugTrace($"任务定时器更新结束 {typeof(TKey).Name} {Key} {Target}");
			return result;
		}
		public void OnTaskCompleted()
		{
		}

		SemaphoreSlim LoadLock { get; } = new SemaphoreSlim(1);
		public async Task LoadTasks(
			CancellationToken cancellactionToken
			)
		{
			Logger.Info($"任务定时器载入开始: {TimeService.Now}");
			await LoadLock.WaitAsync();
			try
			{
				var now = TimeService.Now;
				var reloadTarget = now.AddSeconds(Setting.PreloadIntervalSeconds);
				var end = reloadTarget.AddSeconds(Setting.PreloadAheadSeconds);

				foreach (var src in Sources)
				{
					try
					{
						await src.LoadTimedTasks(
							end,
							cancellactionToken
							);
					}
					catch (Exception ex)
					{
						Logger.Error(ex, $"任务定时器载入异常");
					}
				}
			}
			finally
			{
				LoadLock.Release();
			}
			Logger.Info($"任务定时器载入结束 {TimeService.Now}");
		}
	}

	public static class TimedTaskRunnerExtension
	{
		
		public static IServiceCollection AddTimedTaskRunnerService(
			this IServiceCollection sc,
			TimedTaskRunnerSetting Setting
			)
		{
			sc.AddSingleton<ITimedTaskService>(sp => 
				new TimedTaskService(
				sp.Resolve<ITimerService>(),
				sp.Resolve<ITimeService>(),
				sp.Resolve<ILogger<TimedTaskService>>(),
				sp.Resolve<IEnumerable<ITimedTaskSoruce>>(),
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

					var timedTaskService = sp.Resolve<ITimedTaskService>();
					var timeService = sp.Resolve<ITimeService>();

					var reloadTarget = timeService.Now;
					var now = reloadTarget;
					for (; ; )
					{
						await Task.Delay(reloadTarget.Subtract(now), cancelToken);
						await timedTaskService.LoadTasks( cancelToken);
						now = timeService.Now;
						reloadTarget = now.AddSeconds(Setting.PreloadIntervalSeconds);
					}
				},
				Setting.AutoStartup
			);
			return sc;
		}
	}
}
