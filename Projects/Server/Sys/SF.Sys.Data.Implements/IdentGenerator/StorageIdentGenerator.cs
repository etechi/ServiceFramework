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

using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using SF.Sys.Services;
using SF.Sys.Threading;

namespace SF.Sys.Data.IdentGenerator
{
	/// <summary>
	/// 默认对象标识生成器
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class StorageIdentGenerator<T> : StorageIdentGenerator,IIdentGenerator<T>
	{
		public StorageIdentGenerator(IDataScope DataScope) : base(DataScope)
		{
		}

		public Task<long[]> GenerateAsync(int Section,int Count=1)
		{
			return GenerateAsync(typeof(T).FullName, Section,Count);
		}
	}
	/// <summary>
	/// 默认对象标识生成器
	/// </summary>
	public class StorageIdentGenerator : IIdentGenerator
	{
		class IdentBatch
		{
			public long Current { get; set; }
			public long End { get; set; }
			public int Section { get; set; }
		}
		static ConcurrentDictionary<string, IdentBatch> Cache { get; } = new ConcurrentDictionary<string, IdentBatch>();
		static ObjectSyncQueue<string> SyncQueue { get; } = new ObjectSyncQueue<string>();

		public IDataScope DataScope { get; }
		const int CountPerBatch = 100;

		public StorageIdentGenerator(
			IDataScope DataScope
			)
		{
			this.DataScope = DataScope;
		}
		Task<long> GetNextBatchStartAsync(string Scope, int Section)
		{
			var type = Scope;
			return DataScope.Retry(
				"批量生成标识",
				async ctx =>
			{
				var re = await ctx.Set<DataModels.IdentSeed>().AddOrUpdateAsync(
					e => e.Type == type,
					() => Task.FromResult(new DataModels.IdentSeed
					{
						NextValue = 1 + CountPerBatch,
						Section = Section,
						Type = type
					}),
					s =>
					{
						if (s.Section == Section)
						{
							s.NextValue += CountPerBatch;
						}
						else
						{
							s.NextValue = 1 + CountPerBatch;
							s.Section = Section;
						}
						return Task.CompletedTask;
					});
				return re.NextValue - CountPerBatch;
			});
		}
		
		public async Task<long[]> GenerateAsync(string Type, int Count, int Section)
		{
			var cacheKey = Type;
			IdentBatch v;
			if (!Cache.TryGetValue(cacheKey,out v))
				v = Cache.GetOrAdd(cacheKey, new IdentBatch());
			return await SyncQueue.Queue(cacheKey,async () =>
			{
				var re = new long[Count];
				for (var i = 0; i < Count; i++)
				{
					if (v.Current == v.End || v.Section != Section)
					{
						v.Current = await GetNextBatchStartAsync(cacheKey, Section);
						v.End = v.Current + CountPerBatch;
						v.Section = Section;
					}
					re[i] = v.Current;
					v.Current++;
				}
				return re;
			});

		}
	}
}