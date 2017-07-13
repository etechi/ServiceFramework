using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using SF.Data.Storage;
using System.Threading;
using SF.Metadata;

namespace SF.Data.IdentGenerator
{
	class IdentBatch
	{
		public long Current { get; set; }
		public long End { get; set; }
	}
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
		static ConcurrentDictionary<string, IdentBatch> Cache { get; } = new ConcurrentDictionary<string, IdentBatch>();
		static ObjectSyncQueue<string> SyncQueue { get; } = new ObjectSyncQueue<string>();
		public IdentGeneratorSetting Setting { get; }
		public StorageIdentGenerator(IdentGeneratorSetting Setting)
		{
			this.Setting = Setting;
		}
		async Task<long> GetNextBatchStartAsync(string Type)
		{
			var re = await Setting.IdentSeedSet.RetryForConcurrencyExceptionAsync(() =>
				  Setting.IdentSeedSet.AddOrUpdateAsync(
					  Type,
					  () => new DataModels.IdentSeed { NextValue = 1, Type = Type },
					  s => s.NextValue += Setting.CountPerBatch
					  )
				);
			return re.NextValue - Setting.CountPerBatch;
		}
		public async Task<long> GenerateAsync(string Type)
		{
			var cacheKey = Setting.ServiceDescriptor.InstanceId+ "/" + Type;
			IdentBatch v;
			if (!Cache.TryGetValue(cacheKey,out v))
				v = Cache.GetOrAdd(cacheKey, new IdentBatch());
			return await SyncQueue.Queue(cacheKey,async () =>
			{
				if (v.Current == v.End)
				{
					v.Current = await GetNextBatchStartAsync(Type);
					v.End = v.Current + Setting.CountPerBatch;
				}
				var re = v.Current;
				v.Current++;
				return re;
			});

		}
	}
}