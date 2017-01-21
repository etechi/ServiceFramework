using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SF
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
