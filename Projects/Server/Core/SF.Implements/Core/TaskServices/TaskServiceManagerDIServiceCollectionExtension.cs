using SF.Core.ServiceManagement;
using SF.Core.TaskServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Core.ServiceManagement
{
	public static class TaskServiceManagerDIServiceCollectionExtension
	{
		public static IServiceCollection AddTaskServiceManager(
			this IServiceCollection sc
			)
		{
			sc.AddSingleton<ITaskServiceManager, TaskServiceManager>();
			sc.AddBootstrap(async sp =>
			{
				foreach (var tss in sp.Resolve<ITaskServiceManager>().Services)
					if (tss.Defination.AutoStartup)
						await tss.Start();
				return Disposable.FromAction(() =>
				{
					sp.Resolve<ITaskServiceManager>().StopAll().Wait();
				});
			});
			return sc;
		}
	}
}

