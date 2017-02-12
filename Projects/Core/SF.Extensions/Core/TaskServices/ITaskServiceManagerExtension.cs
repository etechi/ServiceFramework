using SF.Core.TaskServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Core.TaskServices
{
	public static class ITaskServiceExtension
	{
		public static bool IsRunning(this ITaskService ts)
		{
			return ts.State == TaskServiceState.Running;
		}
		public static bool IsStopped(this ITaskService ts)
		{
			return ts.State == TaskServiceState.Stopped ||
				ts.State==TaskServiceState.Error ||
				ts.State==TaskServiceState.Exited
				;
		}
		public static Task Start(this ITaskService ts)
		{
			return ts.Start(CancellationToken.None);
		}
		public static Task Stop(this ITaskService ts)
		{
			return ts.Stop(CancellationToken.None);
		}
	}
	public static class ITaskServiceManagerExtension
	{
		public static async Task StopAll(this ITaskServiceManager tsm)
		{
			await Task.WhenAll(
				tsm.Services.Where(s => s.IsRunning()).Select(s => s.Stop()).ToArray()
				);
		}
		public static async Task StartAll(this ITaskServiceManager tsm)
		{
			foreach (var s in tsm.Services.Where(s => s.IsStopped()))
				await s.Start();
		}
	}
}

