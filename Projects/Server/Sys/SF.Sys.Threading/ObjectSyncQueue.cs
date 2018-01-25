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
using System.Collections.Concurrent;

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
		abstract class Item
		{
			static int seed = 0;
			int id;
			public Item() {
				Next = Prev = this;
				id = Interlocked.Increment(ref seed);
			}
			public Item Next;
			public Item Prev;
			public abstract Task Execute();
		}
		class Item<T> : Item
		{
			public Func<Task<T>> Callback;
			public TaskCompletionSource<T> TCS;
			public override async Task Execute()
			{
				try
				{
					var re = await Callback();
					TCS.TrySetResult(re);
				}
				catch(Exception ex)
				{
					TCS.TrySetException(ex);
				}
				Callback = null;
			}
		}
		ConcurrentDictionary<K, Item> Dict { get; } = new ConcurrentDictionary<K, Item>();
		bool _Disposed;
		public Task<T> Queue<T>(K key, Func<Task<T>> callback)
		{
			TaskCompletionSource<T> tcs = null;
			Item head = null;
			for (; ; )
			{
				if (!Dict.TryGetValue(key, out head))
					head = Dict.GetOrAdd(key, new Item<T>() {Callback= callback });

				lock (head)
				{
					if (head.Next == null)
						continue;

					var curItem = head as Item<T>;
					if (curItem==null || curItem.Callback!=callback)
					{
						var item = new Item<T> { Callback = callback };
						var tail = head.Prev;
						item.Next = head;
						item.Prev = tail;
						tail.Next = item;
						head.Prev = item;
						item.TCS = tcs = new TaskCompletionSource<T>();
					}
					break;
				}
			}
			if (tcs != null)
				return tcs.Task;
			
			var re = callback();
			re.ContinueWith(async t =>
			{
				for (; ; )
				{
					Item item;
					lock (head)
					{
						item = head.Next;
						if (item == head)
						{
							head.Next = null;
							Dict.TryRemove(key, out item);
							if (item != head)
								throw new InvalidOperationException();
							break;
						}
						head.Next = item.Next;
						item.Next.Prev = head;
					}
					try
					{
						await item.Execute();
					}
					catch { }
				}
				
			});
			return re;
		}

		public void Dispose()
		{
			lock (Dict)
				_Disposed = true;
			Task.Run(async () =>
			{
				for (; ; )
				{
					lock (Dict)
					{
						if (Dict.Count == 0)
							break;
					}
					await Task.Delay(10);
				}
			}).Wait();
		}
	}
}
