using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Core.TaskServices
{
	public class TaskServiceManager : ITaskServiceManager
	{
		Dictionary<string,TaskService> ServiceDict { get; }
		public TaskServiceManager(IEnumerable<ITaskServiceDefination> Services,IServiceProvider ServiceProvider)
		{
			this.ServiceDict = Services.ToDictionary(
				s => s.Name, 
				s =>new TaskService(s, ServiceProvider, s.Entry));
		}
		public IEnumerable<ITaskService> Services
		{
			get
			{
				return ServiceDict.Values;
			}
		}

		public ITaskService GetService(string Name)
		{
			return ServiceDict.Get(Name);
		}
	}
}

