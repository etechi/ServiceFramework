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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Sys.TaskServices
{
	public static class EnsuerExtension
	{
		static void StartTask<TKey>(
			IServiceProvider sp,
			SF.Sys.Threading.FlowController fc,
			TKey id,
			Func<IServiceProvider, TKey, Task> Process
			)
		{
			Task.Run(async () =>
			{
				try {
					await sp.WithScope(isp =>
						Process(isp, id)
					);
				}catch(Exception e)
				{
					fc.Complete();
				}
			});
		}
		public static IServiceCollection AddTaskRunnner<TKey>(
			this IServiceCollection sc,
			string Name,
			Func<IServiceProvider, Task> StartupCleanup,
			Func<IServiceProvider, int, Task<TKey[]>> GetIdents,
			Func<IServiceProvider, TKey, Task> Process,
			int BatchCount=100,
			int ThreadCount =1000,
			int Interval=1000
			)
		{
			var fc = new SF.Sys.Threading.FlowController(ThreadCount);
			sc.AddTimerService(
				Name,
				Interval,
				async sp =>
				{
					for (; ; )
					{
						var count = fc.ThreadCount - fc.CurrentThreadCount;
						if (count <= 0)
							break;
						if (count > BatchCount)
							count = BatchCount;

						var ids = await sp.WithScope(isp =>
							GetIdents(isp, BatchCount)
							);
						if (ids.Length == 0)
							break;

						foreach (var id in ids)
						{
							await fc.Wait();
							try
							{
								StartTask(sp, fc, id, Process);
							}
							catch
							{
								fc.Complete();
							}
						}
					}
					return Interval;
				},
				async sp =>
					await sp.WithScope(isp =>
						StartupCleanup(isp)
					)
				);
			return sc;
		}
	}
}
