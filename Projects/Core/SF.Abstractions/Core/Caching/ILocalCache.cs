using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Caching
{
   
	public interface ILocalCache
	{
		object Get(string key);
		void Set(string key, object value, TimeSpan keepalive);
		void Set(string key, object value, DateTime expires);
		//如果缓存已存在，返回缓存对象，否则返回null;
		object AddOrGetExisting(string key, object value, TimeSpan keepalive);
		//如果缓存已存在，返回缓存对象，否则返回null;
		object AddOrGetExisting(string key, object value, DateTime expires);
		bool Remove(string key);
		bool Contains(string key);
	}
}
