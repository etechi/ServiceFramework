using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SF
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
        public static V Get<K,V>(this IDictionary<K,V> dict,K key,V defaultValue=default(V))
		{
			V v;
			if (dict.TryGetValue(key, out v))
				return v;
			return defaultValue;
		}
        public static V Get<K, V>(this IReadOnlyDictionary<K, V> dict, K key, V defaultValue = default(V))
        {
            V v;
            if (dict.TryGetValue(key, out v))
                return v;
            return defaultValue;
        }
    }
}
