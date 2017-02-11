using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Core.TaskServices
{
	//public abstract class TaskService : ITaskService
	//{
	//	public TaskServiceState State { get; private set; } = TaskServiceState.Init;
	//	public SyncScope SyncScope { get; } = new SyncScope();

	//	void EnsureNotDisposed()
	//	{
	//		if (State == TaskServiceState.Disposed)
	//			throw new ObjectDisposedException(GetType().FullName);
	//	}
	//	public Task Start()
	//	{
	//		return Start(CancellationToken.None);
	//	}
	//	public async Task Start(CancellationToken cancellationToken)
	//	{
	//		EnsureNotDisposed();
	//		using (await SyncScope.EnterAsync(cancellationToken))
	//		{
	//			if (State != TaskServiceState.Init && State != TaskServiceState.Stopped)
	//				throw new InvalidOperationException("状态不正确");
	//			State = TaskServiceState.Starting;
	//			try
	//			{
	//				await OnStart(cancellationToken);
	//				State = TaskServiceState.Running;
	//			}
	//			catch
	//			{
	//				State = TaskServiceState.Error;
	//				throw;
	//			}
	//		}
	//	}
	//	public Task Stop()
	//	{
	//		return Stop(CancellationToken.None);
	//	}
	//	public async Task Stop(CancellationToken cancellationToken)
	//	{
	//		EnsureNotDisposed();
	//		using (await SyncScope.EnterAsync(cancellationToken))
	//		{
	//			if (State != TaskServiceState.Running)
	//				throw new InvalidOperationException("状态不正确");
	//			State = TaskServiceState.Stopping;
	//			try
	//			{
	//				await OnStop(cancellationToken);
	//				State = TaskServiceState.Stopped;
	//			}
	//			catch
	//			{
	//				State = TaskServiceState.Error;
	//				throw;
	//			}
	//		}
	//	}

	//	Task LongTask;
	//	protected virtual Task OnStart(CancellationToken cancellationToken)
	//	{
	//		LongTask = Task.Run(RunLongTask);
	//		return Task.CompletedTask;
	//	}
	//	protected virtual Task OnStop(CancellationToken cancellationToken)
	//	{
	//		if (LongTaskCancellationSource != null)
	//			LongTaskCancellationSource.Cancel(true);
	//		if (LongTask != null)
	//		{
	//			LongTask.Wait();
	//			LongTask = null;
	//		}
	//		return Task.CompletedTask;
	//	}

	//	CancellationTokenSource LongTaskCancellationSource;
	//	protected virtual async Task RunLongTask()
	//	{
	//		using (SyncScope.Enter())
	//		{
	//			if (State != TaskServiceState.Running && State != TaskServiceState.Starting)
	//				return;
	//			LongTaskCancellationSource = new CancellationTokenSource();
	//		}
	//		try
	//		{
	//			await OnRunLongTask(LongTaskCancellationSource.Token);
	//		}
	//		finally
	//		{
	//			LongTaskCancellationSource.Dispose();
	//			LongTaskCancellationSource = null;
	//		}
	//	}
	//	protected virtual Task OnRunLongTask(CancellationToken cancellationToken)
	//	{
	//		return Task.CompletedTask;
	//	}

	//	protected virtual void OnDispose() { }

	//	public void Dispose()
	//	{
	//		if (State == TaskServiceState.Disposed)
	//			return;
	//		if (State == TaskServiceState.Running)
	//			Stop().Wait();
	//		else if (State == TaskServiceState.Starting || State == TaskServiceState.Stopping)
	//			throw new InvalidOperationException();
	//		OnDispose();
	//		State = TaskServiceState.Disposed;
	//		SyncScope.Dispose();
	//	}
	//}
}

