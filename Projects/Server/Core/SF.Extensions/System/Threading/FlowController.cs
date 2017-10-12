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

using System.Collections.Generic;
namespace System.Threading.Tasks
{
	public class FlowController
	{
		public int ThreadCount { get; }
		int _CurrentThreadCount;
		public int CurrentThreadCount => _CurrentThreadCount;
		public int WaitingCount => WaitQueue.Count;
		Queue<TaskCompletionSource<int>> WaitQueue { get; } = new Queue<TaskCompletionSource<int>>();
		public FlowController(int ThreadCount)
		{
			this.ThreadCount = ThreadCount;
		}
		public Task Wait()
		{
			lock (WaitQueue)
			{
				if (_CurrentThreadCount < ThreadCount)
				{
					_CurrentThreadCount++;
					return Task.CompletedTask;
				}
				var tcs = new TaskCompletionSource<int>();
				WaitQueue.Enqueue(tcs);
				return tcs.Task;
			}
		}

		public void Complete()
		{
			TaskCompletionSource<int> tcs;
			lock (WaitQueue)
			{
				if (WaitQueue.Count == 0)
				{
					_CurrentThreadCount--;
					return;
				}
				tcs = WaitQueue.Dequeue();
			}
			try
			{
				tcs.TrySetResult(0);
			}
			catch { }
		}
	}
}
