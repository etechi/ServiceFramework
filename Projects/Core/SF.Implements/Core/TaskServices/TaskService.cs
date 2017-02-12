using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Core.TaskServices
{
	public class TaskService : ITaskService
	{
		public string Name { get; }
		public TaskServiceState State { get; private set; } = TaskServiceState.Stopped;
		SyncScope SyncScope { get; } = new SyncScope();
		IServiceProvider ServiceProvider { get; }
		Func<IServiceProvider,ITaskServiceState,CancellationToken,Task> Entry { get; }

		CancellationTokenSource _LongTaskCancellationSource;
		TaskCompletionSource<int> _StopTaskCompletionSource;
		public Exception Exception { get; private set; }

		public TaskService(string Name,IServiceProvider ServiceProvider, Func<IServiceProvider, ITaskServiceState, CancellationToken, Task> Entry)
		{
			this.Name = Name;
			this.ServiceProvider = ServiceProvider;
			this.Entry = Entry;
		}
		public async Task Start(CancellationToken cancellationToken)
		{
			using (await SyncScope.EnterAsync(cancellationToken))
			{
				if (State != TaskServiceState.Stopped && 
					State!=TaskServiceState.Error &&
					State != TaskServiceState.Exited)
					throw new InvalidOperationException("状态不正确");
				Exception = null;
				State = TaskServiceState.Starting;
				try
				{
					StartTaskProcess();
				}
				catch(Exception e)
				{
					Exception = e;
					State = TaskServiceState.Error;
					throw;
				}
			}
		}
		void StartTaskProcess()
		{
			Task.Run(TaskProcess);
		}
		public async Task Stop(CancellationToken cancellationToken)
		{
			using (await SyncScope.EnterAsync(cancellationToken))
			{
				if (State == TaskServiceState.Stopped || State == TaskServiceState.Error || State == TaskServiceState.Exited)
					return;

				if (State != TaskServiceState.Running)
					throw new InvalidOperationException("状态不正确");
				_StopTaskCompletionSource = new TaskCompletionSource<int>();
				try
				{
					State = TaskServiceState.Stopping;
					_LongTaskCancellationSource.Cancel();
					await _StopTaskCompletionSource.Task;
				}
				finally
				{
					_StopTaskCompletionSource = null;
				}
			}
		}
		void TaskEnd(Task task)
		{
			using (SyncScope.Enter())
			{
				try
				{
					if (State != TaskServiceState.Running && State != TaskServiceState.Stopping)
						throw new InvalidOperationException("状态不正确");

					if (task.IsFaulted)
					{
						Exception = task.Exception;
						State = TaskServiceState.Error;
					}
					else if (task.IsCanceled)
					{
						State = TaskServiceState.Stopped;
					}
					else if (task.IsCompleted)
					{
						State = TaskServiceState.Exited;
					}
					else
						throw new NotSupportedException();
				}
				finally
				{
					if (_StopTaskCompletionSource != null)
						_StopTaskCompletionSource.TrySetResult(0);
					if(_LongTaskCancellationSource != null)
					{
						_LongTaskCancellationSource.Dispose();
						_LongTaskCancellationSource = null;
					}
				}
			}
		}
		Task TaskProcess()
		{
			_LongTaskCancellationSource = new CancellationTokenSource();
			try
			{
				var re = Entry(ServiceProvider, this, _LongTaskCancellationSource.Token);
				re.ContinueWith(TaskEnd);
				State = TaskServiceState.Running;
				return re;
			}
			catch(Exception e)
			{
				_LongTaskCancellationSource.Dispose();
				_LongTaskCancellationSource = null;
				State = TaskServiceState.Error;
				Exception = e;
				throw;
			}
		}

	}
}

