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
	public interface ISyncQueue<K> {
		Task<T> Queue<T>(K key, Func<Task<T>> callback);
	}
	public static class SyncQueueExtension
	{
		public static Task Queue<K>(this ISyncQueue<K> Queue, K key, Func<Task> callback)
		{
			return Queue.Queue(key,
				async () => {
					await callback();
					return true;
				}
			);
		}
	}
	public class NoneSyncQueue<K> : ISyncQueue<K>
	{
		public static ISyncQueue<K> Instance { get; } = new NoneSyncQueue<K>();
		public Task<T> Queue<T>(K key, Func<Task<T>> callback)
		{
			return callback();
		}
	}
	public class ObjectSyncQueue<K> : ISyncQueue<K>,IDisposable
	{
		class Item : SemaphoreSlim
		{
			public Item() : base(1) {}
			public int WaitCount;
		}
        static Stack<Item> ItemCache { get; } = new Stack<Item>();
		Dictionary<K, Item> Dicts { get; } = new Dictionary<K, Item>();
		bool _Disposed;
		
		public async Task<T> Queue<T>(K key, Func<Task<T>> callback)
		{
			Item item = null;
			try
			{
				lock (Dicts)
				{
					if (_Disposed)
						throw new ObjectDisposedException("同步操作队列");

                    if (Dicts.TryGetValue(key, out item))
                        item.WaitCount++;
                    else
                    {
						lock (ItemCache)
						{
							if(ItemCache.Count > 0)
								item = ItemCache.Pop();
						}
						if (item == null)
							item = new Item();

						item.WaitCount = 1;
                        Dicts.Add(key, item);
                    }

				}
				await item.WaitAsync();
				try
				{
					return await callback();
				}
				finally
				{
					item.Release();
				}
			}
			finally
			{
				if (item != null)
					lock (Dicts)
					{
						item.WaitCount--;
						if (item.WaitCount == 0)
							Dicts.Remove(key);
						else
							item = null;
					}
				if(item!=null)
					lock (ItemCache)
						ItemCache.Push(item);

			}
		}

		public void Dispose()
		{
			lock (Dicts)
				_Disposed = true;
			Task.Run(async () =>
			{
				for (; ; )
				{
					lock (Dicts)
					{
						if (Dicts.Count == 0)
							break;
					}
					await Task.Delay(10);
				}
			}).Wait();
		}
	}
}
