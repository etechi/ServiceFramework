using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Caching
{
	public class SystemMemoryCache : ILocalCache
	{
		readonly System.Runtime.Caching.MemoryCache cache;
		public SystemMemoryCache(string name)
		{
			cache = new System.Runtime.Caching.MemoryCache(name);
		}
		public object AddOrGetExisting(string key, object value, DateTime expires)
		{
			return cache.AddOrGetExisting(key, value, expires);
		}

		public object AddOrGetExisting(string key, object value, TimeSpan keepalive)
		{
			return cache.AddOrGetExisting(
				key, 
				value, 
				new System.Runtime.Caching.CacheItemPolicy { SlidingExpiration = keepalive }
				);
		}


		public bool Contains(string key)
		{
			return cache.Contains(key);
		}


		public object Get(string key)
		{
			return cache.Get(key);
		}

		public bool Remove(string key)
		{
			return cache.Remove(key)!=null;
		}


		public void Set(string key, object value, DateTime expires)
		{
			cache.Set(key, value, expires);
		}

		public void Set(string key, object value, TimeSpan keepalive)
		{
			cache.Set(key, value, new System.Runtime.Caching.CacheItemPolicy { SlidingExpiration = keepalive });
		}

	}
}
