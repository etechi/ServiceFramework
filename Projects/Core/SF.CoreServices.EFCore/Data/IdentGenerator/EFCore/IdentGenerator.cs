using SF.Data.Entity;
using System;
using System.Threading.Tasks;
using SF.Caching;

namespace SF.Data.IdentGenerator.EFCore
{
	class IdentBatch
	{
		public long Current { get; set; }
		public long End { get; set; }
	}
	public class IdentGeneratorSetting
	{
		public IDataContext DataContext { get; set; }
		public SF.Services.IServiceIdent Ident { get; set; }
		public int CountPerBatch { get; set; } = 100;
	}
	public class IdentGenerator : IIdentGenerator
	{
		static System.Collections.Concurrent.ConcurrentDictionary<string, IdentBatch> Cache { get; } = new System.Collections.Concurrent.ConcurrentDictionary<string, IdentBatch>();
		static Threading.ObjectSyncQueue<string> SyncQueue { get; } = new Threading.ObjectSyncQueue<string>();
		public IdentGeneratorSetting Setting { get; }
		public IdentGenerator(IdentGeneratorSetting Setting)
		{
			this.Setting = Setting;
		}
		async Task<long> GetNextBatchStart(string Type)
		{
			var re = await Setting.DataContext.RetryForConcurrencyException(() =>
				  Setting.DataContext.AddOrUpdate(
					  Type,
					  () => new DataModels.IdentSeed { NextValue = 1, Type = Type },
					  s => s.NextValue += Setting.CountPerBatch
					  )
				);
			return re.NextValue - Setting.CountPerBatch;
		}
		public async Task<long> Generate(string Type)
		{
			var cacheKey = Setting.Ident.Value + "/" + Type;
			if(!Cache.TryGetValue(cacheKey,out var v))
				v = Cache.GetOrAdd(cacheKey, new IdentBatch());
			return await SyncQueue.Queue(cacheKey,async () =>
			{
				if (v.Current == v.End)
				{
					v.Current = await GetNextBatchStart(Type);
					v.End = v.Current + Setting.CountPerBatch;
				}
				var re = v.Current;
				v.Current++;
				return re;
			});

		}
	}
}