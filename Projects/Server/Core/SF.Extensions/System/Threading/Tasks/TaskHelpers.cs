using System.Runtime.InteropServices;

namespace System.Threading.Tasks
{
	public static class TaskHelpers
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct AsyncVoid
		{
		}

		private static class CancelCache<TResult>
		{
			public static readonly Task<TResult> Canceled = TaskHelpers.CancelCache<TResult>.GetCancelledTask();

			private static Task<TResult> GetCancelledTask()
			{
				TaskCompletionSource<TResult> taskCompletionSource = new TaskCompletionSource<TResult>();
				taskCompletionSource.SetCanceled();
				return taskCompletionSource.Task;
			}
		}

		private static readonly Task _defaultCompleted = Task.FromResult<TaskHelpers.AsyncVoid>(default(TaskHelpers.AsyncVoid));

		private static readonly Task<object> _completedTaskReturningNull = Task.FromResult<object>(null);

		public static Task Canceled()
		{
			return TaskHelpers.CancelCache<TaskHelpers.AsyncVoid>.Canceled;
		}

		public static Task<TResult> Canceled<TResult>()
		{
			return TaskHelpers.CancelCache<TResult>.Canceled;
		}

		public static Task Completed()
		{
			return TaskHelpers._defaultCompleted;
		}

		public static Task FromError(Exception exception)
		{
			return TaskHelpers.FromError<TaskHelpers.AsyncVoid>(exception);
		}

		public static Task<TResult> FromError<TResult>(Exception exception)
		{
			TaskCompletionSource<TResult> taskCompletionSource = new TaskCompletionSource<TResult>();
			taskCompletionSource.SetException(exception);
			return taskCompletionSource.Task;
		}

		public static Task<object> NullResult()
		{
			return TaskHelpers._completedTaskReturningNull;
		}
	}
}