﻿
using SF.Sys.Entities;
using SF.Sys.AtLeastOnceActions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Sys.Data;
using SF.Sys.Services;
using SF.Sys.Caching;
using SF.Sys.TimeServices;

namespace SF.Sys.AtLeastOnceActions
{
	class AtLeastOnceActionProvider :
		IAtLeastOnceActionProvider
	{
		IEnumerable<IAtLeastOnceActionSource> ActionSources { get; }
		AtLeastOnceActionSyncQueue SyncQueue { get; }
		ILocalCache<CacheItem> ContextCache { get; }
		Lazy<IIdentGenerator<DataModels.AtLeastOnceAction>> IdentGenerator { get; }
		ITimeService TimeService { get; }
		NamedServiceResolver<IAtLeastOnceActionSource> AtLeastOnceActionSourceResolver { get; }
		IDataScope DataScope { get; }
		public class CacheItem
		{
			public string Ident;
			public string Type;
			public object Context;
		}
		public AtLeastOnceActionProvider(
			IDataScope DataScope,
			IEnumerable<IAtLeastOnceActionSource> ActionSources,
			AtLeastOnceActionSyncQueue SyncQueue,
			ILocalCache<CacheItem> ContextCache,
			Lazy<IIdentGenerator<DataModels.AtLeastOnceAction>> IdentGenerator,
			ITimeService TimeService,
			NamedServiceResolver<IAtLeastOnceActionSource> AtLeastOnceActionSourceResolver
			) 
		{
			this.TimeService = TimeService;
			this.DataScope = DataScope;
			this.ActionSources = ActionSources;
			this.SyncQueue = SyncQueue;
			this.ContextCache = ContextCache;
			this.IdentGenerator = IdentGenerator;
			this.AtLeastOnceActionSourceResolver = AtLeastOnceActionSourceResolver;
		}

		public async Task Active(long Id)
		{
			var cacheItem = ContextCache.Get(Id.ToString());
			if (cacheItem == null)
				return;
			var source = AtLeastOnceActionSourceResolver(cacheItem.Type);
			if (source == null)
				return;
			await SyncQueue.Queue(cacheItem.Type + "/" + cacheItem.Ident, async () =>
				{
					await source.ExecAction(cacheItem.Type, cacheItem.Ident, cacheItem.Context);
					await DataScope.Use("删除任务", ctx =>
					ctx.Set<DataModels.AtLeastOnceAction>().RemoveRangeAsync(a => a.Id == Id)
					);
					return 0;
				});
		}
		public async Task<DateTime?> ActiveByTimer(DataModels.AtLeastOnceAction Action)
		{
			var source = AtLeastOnceActionSourceResolver(Action.Type);
			if (source == null)
				throw new InvalidOperationException("找不到至少一次调用原对象:" + Action.Type);
			var ctx=await source.LoadActionContext(Action.Type, Action.Ident);

			return await source.ExecAction(Action.Type, Action.Ident, ctx);
		}
		public async Task<long> Register(string Type, string Ident, string Name, object Context, TimeSpan? RetryDelay)
		{
			return await SyncQueue.Queue(Type+"/"+Ident, async () =>
			{
				var id = await IdentGenerator.Value.GenerateAsync();
				var cacheKey = id.ToString();
				ContextCache.Set(
					cacheKey, 
					new CacheItem
					{
						Context = Context,
						Type = Type,
						Ident = Ident
					}, 
					TimeSpan.FromMinutes(5));
				var now = TimeService.Now;
				var retryTime = now.Add(RetryDelay ?? TimeSpan.FromMinutes(5));

				await DataScope.Use("添加任务",ctx =>
					ctx.Set< DataModels.AtLeastOnceAction>().EnsureAsync(
						a => a.Type == Type && a.Ident == Ident,
						() => Task.FromResult(
							new DataModels.AtLeastOnceAction
							{
								Id = id,
								CreatedTime = now,
								LogicState = EntityLogicState.Enabled,
								Name = Name.Limit(100),
								TaskStartTime = now,
								TaskNextExecTime = retryTime,
								Ident = Ident,
								Type = Type,
								TaskState = AtLeastOnceTasks.AtLeastOnceTaskState.Waiting,
							})
					)
				);
				return id;
			});
		}
	}
}
