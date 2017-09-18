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
		System.Runtime.Caching.MemoryCache _cache;
		public SystemMemoryCache()
		{
			_cache = new System.Runtime.Caching.MemoryCache(typeof(T).FullName);
		}
		public T AddOrGetExisting(string key, T value, DateTime expires)
		{
			return (T)_cache.AddOrGetExisting(key, value, expires);
		}

		public T AddOrGetExisting(string key, T value, TimeSpan keepalive)
		{
			return (T)_cache.AddOrGetExisting(
				key, 
				value, 
				new System.Runtime.Caching.CacheItemPolicy { SlidingExpiration = keepalive }
				);
		}

		public void Clear()
		{
			_cache = new System.Runtime.Caching.MemoryCache(typeof(T).FullName);
		}

		public bool Contains(string key)
		{
			return _cache.Contains(key);
		}


		public T Get(string key)
		{
			return (T)_cache.Get(key);
		}

		public bool Remove(string key)
		{
			return _cache.Remove(key)!=null;
		}


		public void Set(string key, T value, DateTime expires)
		{
			_cache.Set(key, value, expires);
		}

		public void Set(string key, T value, TimeSpan keepalive)
		{
			_cache.Set(key, value, new System.Runtime.Caching.CacheItemPolicy { SlidingExpiration = keepalive });
		}

	}
}
