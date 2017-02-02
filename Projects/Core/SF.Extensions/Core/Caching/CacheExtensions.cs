using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Caching
{
	public static class CacheExtensions
	{
		public static async Task<object> GetOrCreateAsync(this IRemoteCache cache, string key, Func<Task<object>> creator, TimeSpan keepalive)
		{
			var re = await cache.GetAsync(key);
			if (re != null)
				return re;
			re = creator();
			var nre = await cache.AddOrGetExistingAsync(key, re, keepalive);
			return nre ?? re;
		}
		public static async Task<object> GetOrCreateAsync(this IRemoteCache cache, string key, Func<Task<object>> creator, DateTime expires)
		{
			var re = await cache.GetAsync(key);
			if (re != null)
				return re;
			re = creator();
			var nre = await cache.AddOrGetExistingAsync(key, re, expires);
			return nre ?? re;
		}
		public static object GetOrCreate(this ILocalCache cache, string key, Func<object> creator, TimeSpan keepalive)
		{
			var re = cache.Get(key);
			if (re != null)
				return re;
			re = creator();
			var nre =  cache.AddOrGetExisting(key, re, keepalive);
			return nre ?? re;
		}
		public static object GetOrCreate(this ILocalCache cache, string key, Func<object> creator, DateTime expires)
		{
			var re = cache.Get(key);
			if (re != null)
				return re;
			re = creator();
			var nre = cache.AddOrGetExisting(key, re, expires);
			return nre ?? re;
		}
	}
}
