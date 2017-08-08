using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using SF.Data.Storage;
using System.Threading;
using SF.Metadata;

namespace SF.Data.IdentGenerator
{
	
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

		public IDataSet<DataModels.IdentSeed> IdentSeedSet { get; }
		const int CountPerBatch = 100;

		public StorageIdentGenerator(
			IDataSet<DataModels.IdentSeed> IdentSeedSet
			)
		{
			this.IdentSeedSet = IdentSeedSet;
		}
		async Task<long> GetNextBatchStartAsync(string Scope, int Section)
		{
			var type = Scope;
			var re = await IdentSeedSet.RetryForConcurrencyExceptionAsync(() =>
				  IdentSeedSet.AddOrUpdateAsync(
					  e=> e.Type==type,
					  () => new DataModels.IdentSeed {
						  NextValue = 1+ CountPerBatch,
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
					  }
					  )
				);
			return re.NextValue - CountPerBatch;
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