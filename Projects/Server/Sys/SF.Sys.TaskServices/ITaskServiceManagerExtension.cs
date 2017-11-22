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

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Sys.TaskServices
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

