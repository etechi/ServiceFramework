using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
namespace SF.Core.Caching
{
	public class SystemMemoryCache<T> : ILocalCache<T>
		where T:class
	{
		readonly System.Runtime.Caching.MemoryCache cache;
		public SystemMemoryCache()
		{
			cache = new System.Runtime.Caching.MemoryCache(typeof(T).FullName);
		}
		public T AddOrGetExisting(string key, T value, DateTime expires)
		{
			return (T)cache.AddOrGetExisting(key, value, expires);
		}

		public T AddOrGetExisting(string key, T value, TimeSpan keepalive)
		{
			return (T)cache.AddOrGetExisting(
				key, 
				value, 
				new System.Runtime.Caching.CacheItemPolicy { SlidingExpiration = keepalive }
				);
		}


		public bool Contains(string key)
		{
			return cache.Contains(key);
		}


		public T Get(string key)
		{
			return (T)cache.Get(key);
		}

		public bool Remove(string key)
		{
			return cache.Remove(key)!=null;
		}


		public void Set(string key, T value, DateTime expires)
		{
			cache.Set(key, value, expires);
		}

		public void Set(string key, T value, TimeSpan keepalive)
		{
			cache.Set(key, value, new System.Runtime.Caching.CacheItemPolicy { SlidingExpiration = keepalive });
		}

	}
}
