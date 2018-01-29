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

using SF.Sys.Logging;
using SF.Sys.TimeServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace SF.Sys.TaskServices
{
	public abstract class Timer: SF.Sys.ADT.TimerQueue.Timer
	{
		public abstract void OnTimer();
		public virtual void OnCancelled() { }

	}
	public interface ITimerService
	{
		void Add(int Seconds, Timer Timer);
		void Remove(Timer Timer);
		void Update(int Seconds,Timer Timer);
	}
	
	public static class TimerServiceExtension
	{
		class TaskTimer : Timer
		{
			public TaskCompletionSource<bool> TCS;
			public CancellationToken CT;
			public override void OnTimer()
			{
				TCS.TrySetResult(true);
			}
			public override void OnCancelled()
			{
				TCS.TrySetCanceled(CT);
			}
		}
		public static async Task WaitAsync(
			this ITimerService TimerService,
			int Seconds,
			CancellationToken cancellationToken=default)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				await Task.FromCanceled(cancellationToken);
				return;
			}

			if (cancellationToken.CanBeCanceled)
			{
				var tcs = new TaskCompletionSource<bool>();
				var timer = new TaskTimer { TCS = tcs, CT = cancellationToken };
				using (cancellationToken.Register(() => TimerService.Remove(timer)))
				{
					TimerService.Add(Seconds, timer);
					await tcs.Task;
				}
			}
			else
			{
				var tcs = new TaskCompletionSource<bool>();
				TimerService.Add(Seconds, new TaskTimer { TCS = tcs });
				await tcs.Task;
			}
		}

		public static Task WaitAsync(
			this ITimerService TimerService,
			TimeSpan Seconds,
			CancellationToken cancellationToken = default
			)
			=> TimerService.WaitAsync((int)Seconds.TotalSeconds, cancellationToken);
	}
	public class TimerService:ITimerService,IDisposable
	{
		ADT.TimerQueue TimerQueue { get; }
		System.Threading.Timer SysTimer { get; }
		ITimeService TimeService { get; }
		bool _Disposed;
		DateTime _NextTime;
		DateTime StartTime { get; }
		ILogger Logger { get; }
		[ThreadStatic]
		volatile bool _InTimerCallback;
		double _Offset;
		void OnTimer(object ctx)
		{
			var items = new List<ADT.TimerQueue.Timer>();
				var now = TimeService.Now;
			lock (TimerQueue)
			{
				var diff = now.Subtract(_NextTime).TotalMilliseconds;
				//如果少于1分钟，认为系统时间往前调整，直接将当前时间设置为系统时间
				if(diff<-1000*60)
				{
					diff = 0;
					_Offset = 0;
					_NextTime = now;
				}
				//偏差在10秒内，记入微调
				else if (diff > -10000 && diff < 10000)
					_Offset = _Offset * 0.95 + diff * 0.05;

				while (_NextTime<=now)
				{	
					items.AddRange(TimerQueue.NextTick());
					_NextTime = _NextTime.AddSeconds(1);
				}
				var dueTime = (int)(_NextTime.Subtract(now).TotalMilliseconds - _Offset);
				if (dueTime < 1)
					dueTime = 1;
				SysTimer.Change(dueTime, Timeout.Infinite);
			}

			_InTimerCallback = true;
			try
			{
				foreach (var item in items)
				{
					try
					{
						((Timer)item).OnTimer();
					}
					catch (Exception ex)
					{
						Logger?.Error(ex, $"定时器异常:{item}");
					}
				}
			}
			finally
			{
				_InTimerCallback = false;
			}
		}
		public TimerService(ITimeService TimeService, ILogger<TimeService> Logger)
		{
			this.TimeService = TimeService;
			TimerQueue = new ADT.TimerQueue(2, 1024);
			_NextTime = TimeService.Now.AddSeconds(1);
			SysTimer = new System.Threading.Timer(OnTimer,null,1000,Timeout.Infinite);
			this.Logger = Logger;
		}
				
		public void Add(int Period,Timer Timer)
		{
			lock (TimerQueue)
			{
				if (_InTimerCallback && Period >= 1)
					Period--;
				TimerQueue.Add(Period, Timer);
			}
		}
		public void Update(int Period, Timer Timer)
		{
			lock(TimerQueue)
			{
				if(Timer.IsInTimerQueue)
					TimerQueue.Remove(Timer);

				if (_InTimerCallback && Period >= 1)
					Period--;

				TimerQueue.Add(Period, Timer);
			}
		}
		public void Remove(Timer Timer)
		{
			bool re;
			lock (TimerQueue)
				re=TimerQueue.Remove(Timer);
			if (re)
				Timer.OnCancelled();
		}

		public void Dispose()
		{
			if (_Disposed)
				return;
			_Disposed = true;

			var items = new List<ADT.TimerQueue.Timer>();
			lock (TimerQueue)
			{
				SysTimer.Dispose();
				items.AddRange(TimerQueue.Reset());
			}
			foreach (var item in items)
			{
				try
				{
					((Timer)item).OnCancelled();
				}
				catch { }
			}
		}
	}
}

namespace SF.Sys.Services
{
	public static class TimerDIExtension
	{
		public static IServiceCollection AddTimerService(this IServiceCollection sc)
		{
			sc.AddSingleton<TaskServices.ITimerService, TaskServices.TimerService>();
			return sc;
		}
	}
}