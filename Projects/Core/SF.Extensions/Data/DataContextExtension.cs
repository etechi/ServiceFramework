using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Data
{
	public static class DataContextExtension
	{
		
		public static Task<T> Retry<T>(this IDataContext Context,Func<System.Threading.CancellationToken, Task<T>> Action,int Timeout=6000000,int Retry=5)
		{
			return TaskUtils.Retry(async ct =>
			{
				try
				{
					return await Action(ct);
				}
				catch
				{
					Context.Reset();
					throw;
				}
			},Timeout,Retry);
		}
    }
}
