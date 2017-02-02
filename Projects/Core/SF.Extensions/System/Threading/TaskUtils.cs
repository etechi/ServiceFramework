using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace System.Threading
{
	public static class TaskUtils
	{
		public static async Task<T> Retry<T>(this Func<Task<T>> Action, int Timeout = 60000, int Retry = 5)
		{
			for (var i = 0; i < Retry; i++)
			{
				try
				{
						return await Action();
				}
				catch
				{
					if (i == Retry - 1)
						throw;
					await Task.Delay(new Random().Next(Timeout / 10) + 1000);
				}
			}
			throw new NotSupportedException();
		}
		public static async Task<T> Retry<T>(this Func<CancellationToken, Task<T>> Action,int Timeout=60000,int Retry=5)
        {
            for(var i = 0; i < Retry; i++)
			{
				try
				{
					using (var ts = new CancellationTokenSource(Timeout))
					{
						return await Action(ts.Token);
					}
				}
				catch
				{
					if (i == Retry - 1)
						throw;
					await Task.Delay(new Random().Next(Timeout / 10) + 1000);
				}
			}
			throw new NotSupportedException();
        }
        
    }
}
