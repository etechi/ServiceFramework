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

namespace SF.Sys.Caching.MicrosoftExtensions
{
	public class LocalCache<T> : ILocalCache<T> where T : class
	{
		IMemoryCache Cache { get; }
		static ConcurrentDictionary<Type, (CancellationChangeToken,CancellationTokenSource)> CancellationChangeTokens { get; } = new ConcurrentDictionary<Type, (CancellationChangeToken,CancellationTokenSource)>();

		static string ItemTypeName { get; } = typeof(T).FullName + ":";
		static string GetKey(string key) =>
			ItemTypeName + key;

		public LocalCache(IMemoryCache cache)
		{
			this.Cache = cache;
		}

		static (CancellationChangeToken,CancellationTokenSource) GetChangeToken()
		{
			return CancellationChangeTokens.GetOrAdd(typeof(T), type =>
				 {
					 var cts = new CancellationTokenSource();
					 return (new CancellationChangeToken(cts.Token), cts);
				 }
				);
		}
		public void Clear()
		{
			if (CancellationChangeTokens.TryRemove(typeof(T), out var cts))
			{
				cts.Item2.Cancel();
				cts.Item2.Dispose();
			}
		}
		public T AddOrGetExisting(string key, T value, TimeSpan keepalive)
		{
			 return Cache.GetOrCreate(GetKey(key), e =>
			 {
				 e.SetSlidingExpiration(keepalive);
				 e.AddExpirationToken(GetChangeToken().Item1);
				 return value;
			 });
		}

		public T AddOrGetExisting(string key, T value, DateTime expires)
		{
			return Cache.GetOrCreate(GetKey(key), e =>
			{
				e.SetAbsoluteExpiration(expires);
				e.AddExpirationToken(GetChangeToken().Item1);
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
