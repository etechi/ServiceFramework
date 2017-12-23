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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace SF.Sys.Threading
{
	
	public static class TaskUtils
	{
		public static async Task<T> Retry<T>(this Func<Task<T>> Action, int Timeout = 60000, int Retry = 5)
		{
			Random rand = null;
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
					if (rand == null)
						rand = new Random();
					await Task.Delay(rand.Next(Timeout / 10) + 1000);
				}
			}
			throw new NotSupportedException();
		}
		public static async Task<T> Retry<T>(this Func<CancellationToken, Task<T>> Action,int Timeout=60000,int Retry=5)
        {
			Random rand = null;
			for (var i = 0; i < Retry; i++)
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
					if (rand == null)
						rand = new Random();
					await Task.Delay(rand.Next(Timeout / 10) + 1000);
				}
			}
			throw new NotSupportedException();
        }
        
    }
}
