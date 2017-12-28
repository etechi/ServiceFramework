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
using System.Threading.Tasks;
using System.Threading;

namespace SF.Sys.Threading
{
	public interface ISyncScope
	{
		Task ExecuteAsync(Func<Task> callback, CancellationToken cancellationToken);
		void Execute(Action callback, CancellationToken cancellationToken);
	}
	public static class SyncScopeExtension
	{
		public static Task SyncAsync(this ISyncScope Scope,Func<Task> callback, CancellationToken cancellationToken)
			=>Scope.ExecuteAsync(callback, cancellationToken);

		public static void Sync(this ISyncScope Scope,Action callback, CancellationToken cancellationToken)
			=>Scope.Execute(callback, cancellationToken);

		public static Task SyncAsync(this ISyncScope Scope, Func<Task> callback)
			=> Scope.ExecuteAsync(callback, CancellationToken.None);
		public static void Sync(this ISyncScope Scope, Action callback)
			=> Scope.Execute(callback, CancellationToken.None);

		public static async Task<T> SyncAsync<T>(this ISyncScope Scope, Func<Task<T>> callback, CancellationToken cancellationToken)
		{
			T re = default;
			await Scope.ExecuteAsync(async () =>
			{ 
				re = await callback();
			},
			cancellationToken
			);
			return re;
		}
		public static T Sync<T>(this ISyncScope Scope, Func<T> callback, CancellationToken cancellationToken)
		{
			T re = default;
			Scope.Execute(() =>
			{
				re = callback();
			},
			cancellationToken
			);
			return re;
		}
		public static Task<T> SyncAsync<T>(this ISyncScope Scope, Func<Task<T>> callback)
			=> Scope.SyncAsync<T>(callback, CancellationToken.None);
		public static T Sync<T>(this ISyncScope Scope, Func<T> callback)
			=> Scope.Sync(callback, CancellationToken.None);

	}
	public class SyncScope : ISyncScope, IDisposable
	{
		SemaphoreSlim _semaphore = new SemaphoreSlim(1);
		
		public async Task ExecuteAsync(Func<Task> callback,CancellationToken cancellationToken)
		{
			await _semaphore.WaitAsync(cancellationToken);
			try { 
				await callback();
			}
			finally
			{
				_semaphore.Release();
			}
		}
		public void Execute(Action callback, CancellationToken cancellationToken)
		{
			_semaphore.Wait(cancellationToken);
			try
			{
				callback();
			}
			finally
			{
				_semaphore.Release();
			}
		}

		public void Dispose()
		{
			if (_semaphore != null)
			{
				_semaphore.Dispose();
				_semaphore = null;
			}
		}
	}
}
