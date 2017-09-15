using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SF.Threading
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
