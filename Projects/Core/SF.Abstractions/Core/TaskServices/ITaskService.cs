using SF.Core.ServiceManagement;
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
	[UnmanagedService]
	public interface ITaskServiceState
	{
		ITaskServiceDefination Defination { get; }
		TaskServiceState State { get; }
		Exception Exception { get; }
	}
	[UnmanagedService]
	public interface ITaskService : ITaskServiceState
	{
		Task Start(CancellationToken cancellationToken);
		Task Stop(CancellationToken cancellationToken);
	}
	[UnmanagedService]
	public interface ITaskServiceDefination
	{
		string Name { get; }
		Func<IServiceProvider,ITaskServiceState,CancellationToken,Task> Entry { get; }
		bool AutoStartup { get; }
	}
	[UnmanagedService]
	public interface ITaskServiceManager
	{
		IEnumerable<ITaskService> Services { get; }
		ITaskService GetService(string Name);
	}
}
