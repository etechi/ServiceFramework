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
