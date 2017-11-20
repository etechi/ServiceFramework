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

namespace SF.Sys.Data
{
	public static class IIdentGeneratorExtension
	{
		public static Task<long> GenerateAsync(this IIdentGenerator g, string Type)
			=> g.GenerateAsync(Type, 0);
		public static Task<long> GenerateAsync<T>(this IIdentGenerator g)
			=> g.GenerateAsync(typeof(T).FullName, 0);

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
