using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SF
{
	public static class IListExtension
	{
		public static void Shuffle<T>(this IList<T> array)
		{
			var rand = new Random();
			for (int i = array.Count - 1; i > 0; i--)
			{
				var j = rand.Next(0, i);
				var t = array[j];
				array[j] = array[i];
				array[i] = t;
			}
		}
		public static int Remove<T>(this IList<T> list,Predicate<T> predicate)
		{
			var i = list.Count-1;
			var c = 0;
			while(i>=0)
			{
				if (predicate(list[i]))
				{
					list.RemoveAt(i);
					c++;
				}
				i--;
			}
			return c;
		}
	}
}
