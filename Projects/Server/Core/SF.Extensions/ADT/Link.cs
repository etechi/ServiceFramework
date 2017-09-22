using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
namespace SF.ADT
{
	public static class Link
	{
		public static IEnumerable<T> ToEnumerable<T>(T head, Func<T, T> Next)
			where T : class
		{
			if (head == null)
				yield break;
			yield return head;
			while (null != (head = Next(head)))
				yield return head;
		}
	}
}

