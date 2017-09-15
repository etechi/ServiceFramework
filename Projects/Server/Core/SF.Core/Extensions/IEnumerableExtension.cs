using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF
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
