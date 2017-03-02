using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
namespace SF.Core.Caching.MicrosoftExtensions
{
	public class LocalCache : ILocalCache
	{
		IMemoryCache Cache { get; }
		public LocalCache(IMemoryCache cache)
		{
			this.Cache = cache;
		}
		public object AddOrGetExisting(string key, object value, TimeSpan keepalive)
		{
			 return Cache.GetOrCreate(key, e =>
			 {
				 e.SetSlidingExpiration(keepalive);
				 return value;
			 });
		}

		public object AddOrGetExisting(string key, object value, DateTime expires)
		{
			return Cache.GetOrCreate(key, e =>
			{
				e.SetAbsoluteExpiration(expires);
				return value;
			});
		}

		public bool Contains(string key)
		{
			object v;
			return Cache.TryGetValue(key, out v);
		}

		public object Get(string key)
		{
			return Cache.Get(key);
		}

		public bool Remove(string key)
		{
			Cache.Remove(key);
			return false;
		}

		public void Set(string key, object value, TimeSpan keepalive)
		{
			 Cache.Set(key, value, keepalive);
		}

		public void Set(string key, object value, DateTime expires)
		{
			Cache.Set(key, value, expires);
		}
	}
}
