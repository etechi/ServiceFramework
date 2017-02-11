using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Core.TaskServices
{
	//public class TaskServiceWithScope : TaskService
	//{
	//	public IServiceProvider ServiceProvider { get; }
	//	public string HandlerName { get; }
	//	public Func<IServiceProvider, ITaskServiceState, CancellationToken, Task> LongTaskCallback { get; }
	//	public Func<IServiceProvider, ITaskServiceState, Task<IDisposable>> StartupCallback { get; }
	//	public TaskServiceWithScope(
	//		IServiceProvider ServiceProvider,
	//		Func<IServiceProvider, ITaskServiceState, Task<IDisposable>> StartupCallback,
	//		Func<IServiceProvider, ITaskServiceState, CancellationToken, Task> LongTaskCallback
	//		)
	//	{
	//		this.ServiceProvider = ServiceProvider;
	//		this.StartupCallback = StartupCallback;
	//		this.LongTaskCallback = LongTaskCallback;
	//	}

	//	IDisposable _disposable;
	//	protected override async Task OnStart(CancellationToken cancellationToken)
	//	{
	//		_disposable = StartupCallback == null ? null : await StartupCallback(ServiceProvider, this);
	//		await base.OnStart(cancellationToken);
	//	}

	//	protected override Task OnStop(CancellationToken cancellationToken)
	//	{
	//		Disposable.Release(ref _disposable);
	//		return Task.CompletedTask;
	//	}
	//	protected override async Task OnRunLongTask(CancellationToken cancellationToken)
	//	{
	//		if (LongTaskCallback == null)
	//		{
	//			await base.OnRunLongTask(cancellationToken);
	//			return;
	//		}
	//		await LongTaskCallback(ServiceProvider, this, cancellationToken);
	//	}
	//}
}

