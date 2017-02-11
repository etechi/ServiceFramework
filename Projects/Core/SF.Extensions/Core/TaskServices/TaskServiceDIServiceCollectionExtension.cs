using SF.Core.TaskServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Core.DI
{
	public static class TaskServiceDIServiceCollectionExtension
	{
		class TaskServiceDefination : ITaskServiceDefination
		{
			public Func<IServiceProvider, ITaskServiceState, CancellationToken, Task> Entry { get; set; }
			public string Name { get; set; }
		}
		public static IDIServiceCollection AddTaskService(
			this IDIServiceCollection sc,
			string Name,
			Func<IServiceProvider, ITaskServiceState, CancellationToken, Task> Entry
			)
		{
			sc.AddSingleton<ITaskServiceDefination>(new TaskServiceDefination
			{
				Name = Name,
				Entry = Entry
			});
			return sc;
		}
		public static IDIServiceCollection AddTaskService(
			this IDIServiceCollection sc,
			string Name,
			Func<IServiceProvider, ITaskServiceState, CancellationToken, Task> Start,
			Func<Task> Stop
			)
		{
			sc.AddSingleton<ITaskServiceDefination>(new TaskServiceDefination
			{
				Name = Name,
				Entry = async (sp, ss, ct) =>
				{
					await Start(sp, ss, ct);
					var cs = new TaskCompletionSource<int>();
					if (!ct.IsCancellationRequested)
						using (ct.Register(() =>cs.TrySetResult(0)))
						{
							await cs.Task;
						}
					
					await Stop();
				}
			});
			return sc;
		}

		public static IDIServiceCollection AddTimerService(
			this IDIServiceCollection sc,
			string Name,
			int Period,
			Func<IServiceProvider, Task<int>> TimerCallback,
			Func<IServiceProvider, Task> StartupCallback = null,
			Func<IServiceProvider, Task> CleanupCallback = null
			)
		{
			return sc.AddTaskService(
				Name,
				 (sp,ss,ct) =>
					Timer.StartTaskTimer(
						Period,
						async () =>
						{
							return await TimerCallback(sp);
						},
						async () =>
						{
							if (StartupCallback != null)
								await StartupCallback(sp);
						},
						async () =>
						{
							if (CleanupCallback != null)
								await CleanupCallback(sp);
						},
						ct
						)
				);
		}
	}
}

