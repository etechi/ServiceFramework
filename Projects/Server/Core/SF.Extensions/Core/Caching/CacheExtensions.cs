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
