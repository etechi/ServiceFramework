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
