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

namespace System.Collections.Generic
{

	public static class IEnumerableExtension
	{
		public static string Join(this IEnumerable<string> Enumerable,string separator = "")
		{
			return string.Join(separator, Enumerable);
		}
        public static string Join<T>(this IEnumerable<T> Enumerable, string separator = "")
        {
            return string.Join(separator, Enumerable);
        }
        public static Dictionary<K,V> ToDictionary<K,V>(this IEnumerable<KeyValuePair<K, V>> Enumerable)
		{
			var re = new Dictionary<K, V>();
			foreach (var p in Enumerable)
				re[p.Key] = p.Value;
			return re;
		}
		public static SortedDictionary<K, V> ToSortedDictionary<K, V>(this IEnumerable<KeyValuePair<K, V>> Enumerable)
		{
			var re = new SortedDictionary<K, V>();
			foreach (var p in Enumerable)
				re[p.Key] = p.Value;
			return re;
		}
        public static IEnumerable<string> Normalize(this IEnumerable<string> s)
        {
            return s
                .Select(i => i == null ? null : i.Trim())
                .Where(i => !string.IsNullOrEmpty(i));
        }
	}
}
