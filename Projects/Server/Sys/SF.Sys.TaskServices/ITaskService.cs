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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Sys.TaskServices
{
	public enum TaskServiceState
	{
		Stopped,
		Starting,
		Running,
		Stopping,
		Exited,
		Error
	}
	public interface ITaskServiceState
	{
		ITaskServiceDefination Defination { get; }
		TaskServiceState State { get; }
		Exception Exception { get; }
	}
	public interface ITaskService : ITaskServiceState
	{
		Task Start(CancellationToken cancellationToken);
		Task Stop(CancellationToken cancellationToken);
	}
	public interface ITaskServiceDefination
	{
		string Name { get; }
		Func<IServiceProvider,Task> Init { get; }
		Func<IServiceProvider,ITaskServiceState,CancellationToken,Task> Entry { get; }
		bool AutoStartup { get; }
	}
	public interface ITaskServiceManager
	{
		IEnumerable<ITaskService> Services { get; }
		ITaskService GetService(string Name);
	}
}
