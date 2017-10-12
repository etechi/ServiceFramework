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
	public static class DictExtension
	{
        public static V Get<K, V>(this Dictionary<K, V> dict, K key, V defaultValue = default(V))
        {
            V v;
            if (dict.TryGetValue(key, out v))
                return v;
            return defaultValue;
        }
  //      public static V Get<K,V>(this IDictionary<K,V> dict,K key,V defaultValue=default(V))
		//{
		//	V v;
		//	if (dict.TryGetValue(key, out v))
		//		return v;
		//	return defaultValue;
		//}
        public static V Get<K, V>(this IReadOnlyDictionary<K, V> dict, K key, V defaultValue = default(V))
        {
            V v;
            if (dict.TryGetValue(key, out v))
                return v;
            return defaultValue;
        }
		public static bool Contains<K, V>(this Dictionary<K, V> src, Dictionary<K, V> dst)
			where V : IEquatable<V>
			=> ((IReadOnlyDictionary<K, V>)src).Contains(dst);
		public static bool Equals<K, V>(this Dictionary<K, V> src, Dictionary<K, V> dst)
			where V : IEquatable<V>
			=> ((IReadOnlyDictionary<K, V>)src).Equals(dst);

		public static bool Contains<K, V>(this IReadOnlyDictionary<K, V> src, IReadOnlyDictionary<K, V> dst)
			where V : IEquatable<V>
		{
			foreach(var p in dst)
			{
				V v;
				if (!src.TryGetValue(p.Key, out v))
					return false;
				if (!v.Equals(p.Value))
					return false;
			}
			return true;
		}
		public static bool Equal<K,V>(this IReadOnlyDictionary<K,V> src, IReadOnlyDictionary<K, V> dst)
			where V:IEquatable<V>
		{
			return src.Contains(dst) && dst.Contains(src);
		}
    }
}
