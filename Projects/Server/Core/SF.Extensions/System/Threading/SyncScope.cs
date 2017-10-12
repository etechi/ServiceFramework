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

namespace System.Threading
{
	public class SyncScope : IDisposable
	{
		SemaphoreSlim _semaphore = new SemaphoreSlim(1);
		public Task<Releaser> EnterAsync()
		{
			return EnterAsync(CancellationToken.None);
		}
		public async Task<Releaser> EnterAsync(CancellationToken cancellationToken)
		{
			if (_semaphore == null)
				throw new InvalidOperationException();
			await _semaphore.WaitAsync(cancellationToken);
			return new Releaser(_semaphore);
		}
		public Releaser Enter()
		{
			if (_semaphore == null)
				throw new InvalidOperationException();
			_semaphore.Wait();
			return new Releaser(_semaphore);
		}
		public Task Sync(Func<Task> callback)
		{
			return Sync(callback, CancellationToken.None);
		}
		public async Task Sync(Func<Task> callback,CancellationToken cancellationToken)
		{
			using (await EnterAsync())
			{
				await callback();
			}
		}
		public Task<T> Sync<T>(Func<Task<T>> callback)
		{
			return Sync(callback, CancellationToken.None);
		}
		public async Task<T> Sync<T>(Func<Task<T>> callback, CancellationToken cancellationToken)
		{
			using (await EnterAsync())
			{
				return await callback();
			}
		}
		public struct Releaser : IDisposable
		{
			SemaphoreSlim _semaphore;
			internal Releaser(SemaphoreSlim semaphore)
			{
				_semaphore = semaphore;
			}

			public void Dispose()
			{
				if (_semaphore == null)
					throw new InvalidOperationException();
				_semaphore.Release();
				_semaphore = null;
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
