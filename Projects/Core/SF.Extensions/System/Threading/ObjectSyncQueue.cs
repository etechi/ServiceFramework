using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace System.Threading
{
	public class ObjectSyncQueue<K> : IDisposable
	{
		class Item : SemaphoreSlim
		{
			public Item() : base(1) {}
			public int WaitCount;
		}
        static Stack<Item> ItemCache { get; } = new Stack<Item>();
		Dictionary<K, Item> Dicts { get; } = new Dictionary<K, Item>();

		public Task Queue(K key, Func<Task> callback)
		{
			return Queue<bool>(key, async () => { await callback(); return true; });
		}
		public async Task<T> Queue<T>(K key, Func<Task<T>> callback)
		{
			Item item = null;
			try
			{
				lock (Dicts)
				{
                    if (Dicts.TryGetValue(key, out item))
                        item.WaitCount++;
                    else
                    {
						lock(ItemCache)
							item = ItemCache.Count > 0 ? ItemCache.Pop() : new Item();

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
						{
							lock(ItemCache)
								ItemCache.Push(item);
							Dicts.Remove(key);
						}
					}
			}
		}

		public void Dispose()
		{
			var items = Dicts.Values;
			Dicts.Clear();
			foreach (var it in items)
				it.Dispose();
           
		}
	}
}
