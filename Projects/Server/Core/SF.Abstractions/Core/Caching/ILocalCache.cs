using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Caching
{
   
	public interface ILocalCache<T>
		where T:class
	{
		void Clear();
		T Get(string key);
		void Set(string key, T value, TimeSpan keepalive);
		void Set(string key, T value, DateTime expires);
		//如果缓存已存在，返回缓存对象，否则返回null;
		T AddOrGetExisting(string key, T value, TimeSpan keepalive);
		//如果缓存已存在，返回缓存对象，否则返回null;
		T AddOrGetExisting(string key, T value, DateTime expires);
		bool Remove(string key);
		bool Contains(string key);
	}
}
