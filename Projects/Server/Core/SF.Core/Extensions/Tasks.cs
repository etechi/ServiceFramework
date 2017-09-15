using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SF
{
	public static class Tasks
	{
        public static async Task<T> Retry<T>(Func<System.Threading.CancellationToken, Task<T>> Action,int Timeout=60000,int Retry=5)
        {
            for(var i = 0; i < Retry; i++)
			{
				try
				{
					using (var ts = new System.Threading.CancellationTokenSource(Timeout))
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
