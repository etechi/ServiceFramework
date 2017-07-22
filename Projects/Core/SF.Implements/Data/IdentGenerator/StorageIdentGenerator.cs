using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using SF.Data.Storage;
using System.Threading;
using SF.Metadata;

namespace SF.Data.IdentGenerator
{
	
	public class IdentGeneratorSetting
	{
		public IDataSet<DataModels.IdentSeed> IdentSeedSet { get;private set; }
		public SF.Core.ServiceManagement.IServiceInstanceDescriptor ServiceDescriptor { get; private set; }
		[Comment("预分配数量","每次预分配的数量")]
		public int CountPerBatch { get; private set; } = 100;
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
		static ConcurrentDictionary<(long,string), IdentBatch> Cache { get; } = new ConcurrentDictionary<(long, string), IdentBatch>();
		static ObjectSyncQueue<(long,string)> SyncQueue { get; } = new ObjectSyncQueue<(long,string)>();
		public IdentGeneratorSetting Setting { get; }
		public StorageIdentGenerator(IdentGeneratorSetting Setting)
		{
			this.Setting = Setting;
		}
		async Task<long> GetNextBatchStartAsync((long,string) Scope, int Section)
		{
			var (sid, type) = Scope;
			var re = await Setting.IdentSeedSet.RetryForConcurrencyExceptionAsync(() =>
				  Setting.IdentSeedSet.AddOrUpdateAsync(
					  e=>e.ScopeId==sid && e.Type==type,
					  () => new DataModels.IdentSeed { NextValue = 1, Section= Section},
					  s =>
					  {
						  if (s.Section == Section)
						  {
							  s.NextValue += Setting.CountPerBatch;
						  }
						  else
						  {
							  s.NextValue = 1 + Setting.CountPerBatch;
							  s.Section = Section;
						  }
					  }
					  )
				);
			return re.NextValue - Setting.CountPerBatch;
		}
		public async Task<long> GenerateAsync(string Type,int Section)
		{
			var cacheKey = (Setting.ServiceDescriptor.InstanceId,Type);
			IdentBatch v;
			if (!Cache.TryGetValue(cacheKey,out v))
				v = Cache.GetOrAdd(cacheKey, new IdentBatch());
			return await SyncQueue.Queue(cacheKey,async () =>
			{
				if (v.Current == v.End || v.Section!=Section )
				{
					v.Current = await GetNextBatchStartAsync(cacheKey,Section);
					v.End = v.Current + Setting.CountPerBatch;
					v.Section = Section;
				}
				var re = v.Current;
				v.Current++;
				return re;
			});

		}
	}
}