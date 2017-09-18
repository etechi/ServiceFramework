using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;
using System.Threading;

namespace SF.Core.Caching.MicrosoftExtensions
{
	public class LocalCache<T> : ILocalCache<T> where T : class
	{
		IMemoryCache Cache { get; }
		static ConcurrentDictionary<Type, CancellationTokenSource> CancellationChangeTokens { get; } = new ConcurrentDictionary<Type, CancellationTokenSource>();

		static string ItemTypeName { get; } = typeof(T).FullName + ":";
		static string GetKey(string key) =>
			ItemTypeName + key;

		public LocalCache(IMemoryCache cache)
		{
			this.Cache = cache;
		}

		static CancellationTokenSource GetChangeToken()
		{
			return CancellationChangeTokens.GetOrAdd(typeof(T), type =>
				 new CancellationTokenSource()
				);
		}
		public void Clear()
		{
			if (CancellationChangeTokens.TryRemove(typeof(T), out var cts))
			{
				cts.Cancel();
				cts.Dispose();
			}
		}
		public T AddOrGetExisting(string key, T value, TimeSpan keepalive)
		{
			 return Cache.GetOrCreate(GetKey(key), e =>
			 {
				 e.SetSlidingExpiration(keepalive);
				 e.AddExpirationToken(new CancellationChangeToken(GetChangeToken().Token));
				 return value;
			 });
		}

		public T AddOrGetExisting(string key, T value, DateTime expires)
		{
			return Cache.GetOrCreate(GetKey(key), e =>
			{
				e.SetAbsoluteExpiration(expires);
				e.AddExpirationToken(new CancellationChangeToken(GetChangeToken().Token));
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
