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
using System.Threading.Tasks;
using System.Collections.Concurrent;
using SF.Data;
using System.Threading;
using SF.Metadata;
using SF.Data.IdentGenerator.DataModels;
using SF.Core.ServiceManagement;

namespace SF.Data.IdentGenerator
{
	[Comment("默认对象标识生成器")]
	public class StorageIdentGenerator<T> : StorageIdentGenerator,IIdentGenerator<T>
	{
		public StorageIdentGenerator(IScoped<IDataSet<DataModels.IdentSeed>> IdentSeedSet) : base(IdentSeedSet)
		{
		}

		public Task<long> GenerateAsync(int Section)
		{
			return GenerateAsync(typeof(T).FullName, Section);
		}
	}
	[Comment("默认对象标识生成器")]
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

		public IScoped<IDataSet<DataModels.IdentSeed>> IdentSeedSet { get; }
		const int CountPerBatch = 100;

		public StorageIdentGenerator(
			IScoped<IDataSet<DataModels.IdentSeed>> IdentSeedSet
			)
		{
			this.IdentSeedSet = IdentSeedSet;
		}
		Task<long> GetNextBatchStartAsync(string Scope, int Section)
		{
			return IdentSeedSet.Use(async (dataSet) =>
			{
				var type = Scope;
				var re = await dataSet.RetryForConcurrencyExceptionAsync(() =>
					  dataSet.AddOrUpdateAsync(
						  e => e.Type == type,
						  () => new DataModels.IdentSeed
						  {
							  NextValue = 1 + CountPerBatch,
							  Section = Section,
							  Type = type
						  },
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
						  })
					);
				return re.NextValue - CountPerBatch;
			});
		}
		
		public async Task<long> GenerateAsync(string Type,int Section)
		{
			var cacheKey = Type;
			IdentBatch v;
			if (!Cache.TryGetValue(cacheKey,out v))
				v = Cache.GetOrAdd(cacheKey, new IdentBatch());
			return await SyncQueue.Queue(cacheKey,async () =>
			{
				if (v.Current == v.End || v.Section!=Section )
				{
					v.Current = await GetNextBatchStartAsync(cacheKey,Section);
					v.End = v.Current + CountPerBatch;
					v.Section = Section;
				}
				var re = v.Current;
				v.Current++;
				return re;
			});

		}
	}
}