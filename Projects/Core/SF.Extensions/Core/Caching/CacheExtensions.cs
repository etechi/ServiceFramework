using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Caching
{
	public static class CacheExtensions
	{
		public static async Task<object> GetOrCreateAsync<T>(this IRemoteCache<T> cache, string key, Func<Task<T>> creator, TimeSpan keepalive)
			where T:class
		{
			var re = await cache.GetAsync(key);
			if (re != null)
				return re;
			re =await creator();
			var nre = await cache.AddOrGetExistingAsync(key, re, keepalive);
			return nre ?? re;
		}
		public static async Task<T> GetOrCreateAsync<T>(this IRemoteCache<T> cache, string key, Func<Task<T>> creator, DateTime expires)
			where T:class
		{
			var re = await cache.GetAsync(key);
			if (re != null)
				return re;
			re = await creator();
			var nre = await cache.AddOrGetExistingAsync(key, re, expires);
			return nre ?? re;
		}
		public static T GetOrCreate<T>(this ILocalCache<T> cache, string key, Func<T> creator, TimeSpan keepalive)
			where T:class
		{
			var re = cache.Get(key);
			if (re != null)
				return re;
			re = creator();
			var nre =  cache.AddOrGetExisting(key, re, keepalive);
			return nre ?? re;
		}
		public static T GetOrCreate<T>(this ILocalCache<T> cache, string key, Func<T> creator, DateTime expires)
			where T:class
		{
			var re = cache.Get(key);
			if (re != null)
				return re;
			re = creator();
			var nre = cache.AddOrGetExisting(key, re, expires);
			return nre ?? re;
		}
		public static async Task<T> GetOrCreateAsync<T>(this ILocalCache<T> cache, string key, Func<Task<T>> creator, TimeSpan keepalive)
		   where T : class
		{
			var re = cache.Get(key);
			if (re != null)
				return re;
			re = await creator();
			var nre = cache.AddOrGetExisting(key, re, keepalive);
			return nre ?? re;
		}
		public static async Task<T> GetOrCreateAsync<T>(this ILocalCache<T> cache, string key, Func<Task<T>> creator, DateTime expires)
			where T : class
		{
			var re = cache.Get(key);
			if (re != null)
				return re;
			re = await creator();
			var nre = cache.AddOrGetExisting(key, re, expires);
			return nre ?? re;
		}
	}
}
