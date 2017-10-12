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

