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
		static ConcurrentDictionary<(long,string), IdentBatch> Cache { get; } = new ConcurrentDictionary<(long, string), IdentBatch>();
		static ObjectSyncQueue<(long,string)> SyncQueue { get; } = new ObjectSyncQueue<(long,string)>();

		public IDataSet<DataModels.IdentSeed> IdentSeedSet { get; }
		public SF.Core.ServiceManagement.IServiceInstanceDescriptor ServiceDescriptor { get;  }
		public int CountPerBatch { get; }

		public StorageIdentGenerator(
			IDataSet<DataModels.IdentSeed> IdentSeedSet,
			SF.Core.ServiceManagement.IServiceInstanceDescriptor ServiceDescriptor,
			[Comment("预分配数量", "每次预分配的数量")]
			int CountPerBatch= 100
			)
		{
			this.IdentSeedSet = IdentSeedSet;
			this.ServiceDescriptor = ServiceDescriptor;
			this.CountPerBatch = CountPerBatch;
		}
		async Task<long> GetNextBatchStartAsync((long,string) Scope, int Section)
		{
			var (sid, type) = Scope;
			var re = await IdentSeedSet.RetryForConcurrencyExceptionAsync(() =>
				  IdentSeedSet.AddOrUpdateAsync(
					  e=>e.ScopeId==sid && e.Type==type,
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
			var iid = ServiceDescriptor.ParentInstanceId??0;
			var cacheKey = (iid,Type);
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