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
	public static class TaskServiceManagerDIServiceCollectionExtension
	{
		public static IDIServiceCollection UseTaskServiceManager(
			this IDIServiceCollection sc
			)
		{
			sc.AddSingleton<ITaskServiceManager, TaskServiceManager>();
			return sc;
		}
	}
}

