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
	public class LocalCache<T> : ILocalCache<T> where T : class
	{
		IMemoryCache Cache { get; }
		static string ItemTypeName { get; } = typeof(T).FullName + ":";
		static string GetKey(string key) =>
			ItemTypeName + key;

		public LocalCache(IMemoryCache cache)
		{
			this.Cache = cache;
		}
		public T AddOrGetExisting(string key, T value, TimeSpan keepalive)
		{
			 return Cache.GetOrCreate(GetKey(key), e =>
			 {
				 e.SetSlidingExpiration(keepalive);
				 return value;
			 });
		}

		public T AddOrGetExisting(string key, T value, DateTime expires)
		{
			return Cache.GetOrCreate(GetKey(key), e =>
			{
				e.SetAbsoluteExpiration(expires);
				return value;
			});
		}

		public bool Contains(string key)
		{
			object v;
			return Cache.TryGetValue(GetKey(key), out v);
		}

		public T Get(string key)
		{
			return (T)Cache.Get(GetKey(key));
		}

		public bool Remove(string key)
		{
			Cache.Remove(GetKey(key));
			return false;
		}

		public void Set(string key, T value, TimeSpan keepalive)
		{
			 Cache.Set(GetKey(key), value, keepalive);
		}

		public void Set(string key, T value, DateTime expires)
		{
			Cache.Set(GetKey(key), value, expires);
		}
	}
}
