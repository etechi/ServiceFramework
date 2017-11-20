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

namespace SF.Sys.Caching
{
   
	public interface ILocalCache<T>
		where T:class
	{
		void Clear();
		T Get(string key);
		void Set(string key, T value, TimeSpan keepalive);
		void Set(string key, T value, DateTime expires);
		/// <summary>
		/// 添加或返回已存在的值
		/// </summary>
		/// <param name="key">键值</param>
		/// <param name="value">缓存内容</param>
		/// <param name="keepalive">存活期</param>
		/// <returns>如果缓存已存在，返回缓存对象，否则返回null</returns>
		T AddOrGetExisting(string key, T value, TimeSpan keepalive);
		/// <summary>
		/// 添加或返回已存在的值
		/// </summary>
		/// <param name="key">键值</param>
		/// <param name="value">缓存内容</param>
		/// <param name="expires">超时时间</param>
		/// <returns>如果缓存已存在，返回缓存对象，否则返回null</returns>
		T AddOrGetExisting(string key, T value, DateTime expires);
		bool Remove(string key);
		bool Contains(string key);
	}
}
