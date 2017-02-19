using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Core.TaskServices
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
		Func<IServiceProvider,ITaskServiceState,CancellationToken,Task> Entry { get; }
		bool AutoStartup { get; }
	}
	public interface ITaskServiceManager
	{
		IEnumerable<ITaskService> Services { get; }
		ITaskService GetService(string Name);
	}
}
