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

using SF.Sys.Services;
using SF.Sys.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Sys.TaskServices
{
	public class TaskService : ITaskService
	{
		public ITaskServiceDefination Defination { get; }
		public TaskServiceState State { get; private set; } = TaskServiceState.Stopped;
		SyncScope SyncScope { get; } = new SyncScope();
		IServiceScopeFactory ScopeFactory { get; }
		Func<IServiceProvider,Task> Init { get; }
		Func<IServiceProvider,ITaskServiceState,CancellationToken,Task> Entry { get; }

		CancellationTokenSource _LongTaskCancellationSource;
		TaskCompletionSource<int> _StopTaskCompletionSource;
		public Exception Exception { get; private set; }

		public TaskService(
			ITaskServiceDefination Defination,
			IServiceProvider ServiceProvider, 
			Func<IServiceProvider,Task> Init,
			Func<IServiceProvider, ITaskServiceState, CancellationToken, Task> Entry
			)
		{
			this.Defination= Defination;
			this.ScopeFactory = ServiceProvider.Resolve< IServiceScopeFactory>();
			this.Entry = Entry;
			this.Init = Init;
		}
		public async Task Start(CancellationToken cancellationToken)
		{
			await SyncScope.Sync(async () =>
			{
				if (State != TaskServiceState.Stopped &&
					State != TaskServiceState.Error &&
					State != TaskServiceState.Exited)
					throw new InvalidOperationException("状态不正确");
				Exception = null;
				State = TaskServiceState.Starting;
				try
				{
					if (Init != null)
					{
						await ScopeFactory.WithScope(sp =>Init(sp));
					}
					StartTaskProcess();
				}
				catch (Exception e)
				{
					Exception = e;
					State = TaskServiceState.Error;
					throw;
				}
			});
		}
		void StartTaskProcess()
		{
			Task.Run(TaskProcess);
		}
		public async Task Stop(CancellationToken cancellationToken)
		{
			Task task=null;
			await SyncScope.Sync(() =>
			{
				if (State == TaskServiceState.Stopped || State == TaskServiceState.Error || State == TaskServiceState.Exited)
					return Task.CompletedTask;

				if (State != TaskServiceState.Running)
					throw new InvalidOperationException("状态不正确");
				_StopTaskCompletionSource = new TaskCompletionSource<int>();
				State = TaskServiceState.Stopping;
				_LongTaskCancellationSource.Cancel();
				task = _StopTaskCompletionSource.Task;
				return Task.CompletedTask;
			});
			try
			{
				await task;
			}
			finally
			{
				await SyncScope.Sync(() =>
				{
					_StopTaskCompletionSource = null;
					return Task.CompletedTask;
				});
			}
		}
		void TaskEnd(Task task)
		{
			TaskCompletionSource<int> stcs = null;
			Task.Run(()=>SyncScope.Sync(() =>
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
					stcs = _StopTaskCompletionSource;
					if (_LongTaskCancellationSource != null)
					{
						_LongTaskCancellationSource.Dispose();
						_LongTaskCancellationSource = null;
					}
				}
				return Task.CompletedTask;
			})).Wait();
			if(stcs!=null)
				stcs.TrySetResult(0);
		}
		Task TaskProcess()
		{
			_LongTaskCancellationSource = new CancellationTokenSource();
			try
			{
				var re = ScopeFactory.WithScope(isp =>
						Entry(isp, this, _LongTaskCancellationSource.Token)
					);
				re.ContinueWith(
					t=>
						Task.Run(()=>
							TaskEnd(t)
						)
					);
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

