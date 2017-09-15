using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Core
{
	public static class Timer
	{
		static async Task<bool> Delay(int Timeout, CancellationToken cancelToken)
		{
			try
			{
				await Task.Delay(Timeout, cancelToken);
				return true;
			}
			catch
			{
				return false;
			}
		}
		public static async Task StartTaskTimer(
		   int Period,
		   Func<Task<int>> TimerCallback,
		   Func<Task> StartupCallback,
		   Func<Task> CleanupCallback,
		   CancellationToken cancellationToken
		   )
		{
			if (Period > 0 && !await Delay(Period, cancellationToken))
				return;

			if (StartupCallback != null)
				await StartupCallback();
			try
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					var interval = await TimerCallback();
					if (interval == int.MaxValue)
						break;
					if (interval > 0 && !await Delay(interval, cancellationToken))
						return;
				}
			}
			finally
			{
				if (CleanupCallback != null)
					await CleanupCallback();
			}
		}
		public static Task StartTaskTimer(
		  int Period,
		  Func<Task<int>> TimerCallback,
		  Func<Task> StartupCallback = null,
		  Func<Task> CleanupCallback = null
		  ) =>
			StartTaskTimer(
				Period,
				TimerCallback,
				StartupCallback,
				CleanupCallback,
				CancellationToken.None
				);
	}
}
