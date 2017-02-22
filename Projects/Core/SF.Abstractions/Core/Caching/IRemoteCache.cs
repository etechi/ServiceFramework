﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Caching
{
    public interface IRemoteCache
	{
		Task<object> GetAsync(string key);
		Task SetAsync(string key, object value, TimeSpan keepalive);
		Task SetAsync(string key, object value, DateTime expires);
		//如果缓存已存在，返回缓存对象，否则返回null;
		Task<object> AddOrGetExistingAsync(string key, object value, TimeSpan keepalive);
		//如果缓存已存在，返回缓存对象，否则返回null;
		Task<object> AddOrGetExistingAsync(string key, object value, DateTime expires);
		Task<bool> RemoveAsync(string key);
		Task<bool> ContainsAsync(string key);
	}
}