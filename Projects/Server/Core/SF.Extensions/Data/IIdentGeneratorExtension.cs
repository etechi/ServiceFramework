using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data
{
	public static class IIdentGeneratorExtension
	{
		public static Task<long> GenerateAsync(this IIdentGenerator g, string Type)
			=> g.GenerateAsync(Type, 0);

		public static async Task<Queue<long>> BatchGenerateAsync<T>(this IIdentGenerator<T> g, int Count, int Section = 0)
		{
			var q = new Queue<long>();
			for (var i = 0; i < Count; i++)
				q.Enqueue(await g.GenerateAsync(Section));
			return q;
		}
		public static async Task<Queue<long>> BatchGenerateAsync(this IIdentGenerator g, string Type,int Count,int Section=0)
		{
			var q = new Queue<long>();
			for (var i = 0; i < Count; i++)
				q.Enqueue(await g.GenerateAsync(Type,Section));
			return q;
		}
	}
}
