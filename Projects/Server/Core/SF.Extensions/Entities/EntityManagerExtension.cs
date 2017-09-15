using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Data;

namespace SF.Entities
{
	public static class EntityManagerExtension
	{
		public static async Task<TKey> ResolveEntity<TKey,  TQueryArgument>(
			this IEntityIdentQueryable<TKey,TQueryArgument> EntityQueryable,
			TQueryArgument QueryArgument
			)
			where TKey : IEquatable<TKey>
			where TQueryArgument : class, IQueryArgument<TKey>
		{
			var re = await EntityQueryable.QueryIdentsAsync(QueryArgument, Paging.Default);
			var res = re.Items.ToArray();
			if (res.Length == 0)
				return default(TKey);
			if (res.Length > 1)
				throw new InvalidOperationException("查询条件返回多条记录");
			return res[0];
		}
		public static async Task UpdateEntity<TManager,TKey, TEditable>(
			this TManager Manager,
			TKey Id,
			Action<TEditable> Updater
			)
			where TKey : IEquatable<TKey>
			where TEditable : class, IEntityWithId<TKey>
			where TManager : IEntityEditableLoader<TKey, TEditable>,
				IEntityUpdator<TKey, TEditable>
		{
			var ins = await Manager.LoadForEdit(Id);
			if (ins == null)
				throw new ArgumentException($"找不到对象:{typeof(TEditable)}:{Id}");
			Updater(ins);
			await Manager.UpdateAsync(ins);
		}

		public static async Task SetEntityState<TManager,TKey, TEditable>(
			this TManager Manager,
			TKey Id,
			EntityLogicState State
			) where TKey : IEquatable<TKey>
			where TEditable : class, IEntityWithId<TKey>,IObjectEntity
			where TManager : IEntityEditableLoader<TKey, TEditable>,
				IEntityUpdator<TKey, TEditable>
		=> await Manager.UpdateEntity<TManager,TKey, TEditable>(
			Id,
			e => e.LogicState = State
			);

		public static async Task SetEntityName<TManager, TKey, TEditable>(
			this TManager Manager,
			TKey Id,
			string Name
			) where TKey : IEquatable<TKey>
			where TEditable : class, IEntityWithId<TKey>, IObjectEntity
			where TManager : IEntityEditableLoader<TKey, TEditable>,
				IEntityUpdator<TKey, TEditable>,
				IEntityCreator<TKey, TEditable>
			=> await Manager.UpdateEntity<TManager,TKey, TEditable>(
				Id,
				e => e.Name = Name
				);

		public static async Task SetEntityParent<TManager,TKey, TEditable>(
			this TManager Manager,
			TKey Id,
			TKey? ParentId
			) where TKey : struct,IEquatable<TKey>
			where TEditable : class, IEntityWithId<TKey>, IItemEntity<TKey?>
			where TManager : IEntityEditableLoader<TKey, TEditable>,
				IEntityUpdator<TKey, TEditable>,
				IEntityCreator<TKey, TEditable>
			=> await Manager.UpdateEntity<TManager,TKey, TEditable>(
				Id,
				e => e.ContainerId=ParentId
				);

		public static async Task<TEditable> EnsureEntity<TManager,TKey, TEditable>(
			this TManager Manager,
			TKey Id,
			Func<TEditable> Creator,
			Action<TEditable> Updater=null
			)
			where TKey:IEquatable<TKey>
			where TEditable:class,IEntityWithId<TKey>
			where TManager: IEntityEditableLoader<TKey,TEditable>,
							IEntityUpdator<TKey,TEditable>,
							IEntityCreator<TKey,TEditable>
		{
			
			var hasInstance = !EqualityComparer<TKey>.Default.Equals(Id, default(TKey));
			TEditable ins;
			if (hasInstance)
			{
				ins = await Manager
					.LoadForEdit(Id)
					.AssertNotNull(() => $"不存在实体{typeof(TEditable)}实例:{Id} ");
			}
			else
				ins = Creator();
			if (Updater != null)
				Updater(ins);
			if (hasInstance)
				await Manager.UpdateAsync(ins);
			else
				Id = await Manager.CreateAsync(ins);
			return await Manager.LoadForEdit(Id);
		}
		public static Task<TEditable> EnsureEntity<TManager,TEditable>(
			this TManager Manager,
			long Id,
			Action<TEditable> Updater = null
			)
			where TEditable : class, IEntityWithId<long>, new()
			where TManager : IEntityEditableLoader<long, TEditable>,
							IEntityUpdator<long, TEditable>,
							IEntityCreator<long, TEditable>
		{
			return EnsureEntity<TManager,long, TEditable>(Manager, Id, Updater);
		}
		public static Task<TEditable> EnsureEntity<TManager,TKey, TEditable>(
			this TManager Manager,
			TKey Id,
			Action<TEditable> Updater = null
			)
			where TKey : IEquatable<TKey>
			where TEditable : class, IEntityWithId<TKey>,new()
			where TManager : IEntityEditableLoader<TKey, TEditable>,
							IEntityUpdator<TKey, TEditable>,
							IEntityCreator<TKey, TEditable>
		{
			return Manager.EnsureEntity<TManager,TKey,TEditable>(
				Id, 
				()=> {
					var o = new TEditable();
					Updater(o);
					return o;
				}, 
				Updater);
		}

		public static async Task<TEditable> EnsureEntityEx<TManager, TEditable, TQueryArgument>(
			this TManager Manager,
			TQueryArgument QueryArgument,
			Func<TEditable> Creator,
			Action<TEditable> Updater = null
			)
			where TEditable : class, IEntityWithId<long>
			where TQueryArgument : class, IQueryArgument<long>
			where TManager : IEntityIdentQueryable<long, TQueryArgument>,
							IEntityEditableLoader<long,TEditable>,
							IEntityUpdator<long, TEditable>,
							IEntityCreator<long, TEditable>
		{
			return await Manager.EnsureEntity<TManager,long,TEditable>(
				await Manager.ResolveEntity(QueryArgument),
				Creator,
				Updater
				);
		}
		public static async Task<TEditable> EnsureEntityEx<TManager, TKey, TEditable,TQueryArgument>(
			this TManager Manager,
			TQueryArgument QueryArgument,
			Func<TEditable> Creator,
			Action<TEditable> Updater = null
			)
			where TKey : IEquatable<TKey>
			where TEditable : class, IEntityWithId<TKey>
			where TQueryArgument:class,IQueryArgument<TKey>
			where TManager : IEntityIdentQueryable<TKey, TQueryArgument>,
							IEntityEditableLoader<TKey, TEditable>,
							IEntityUpdator<TKey, TEditable>,
							IEntityCreator<TKey, TEditable>
		{
			return await Manager.EnsureEntity<TManager,TKey, TEditable>(
				await Manager.ResolveEntity(QueryArgument),
				Creator,
				Updater
				);
		}
		public static Task<TEditable> EnsureEntityEx<TManager,  TEditable, TQueryArgument>(
		   this TManager Manager,
		   TQueryArgument QueryArgument,
		   Action<TEditable> Updater
		   )
		   where TEditable : class, IEntityWithId<long>, new()
		   where TQueryArgument : class, IQueryArgument<long>
			where TManager : IEntityIdentQueryable<long, TQueryArgument>,
							IEntityEditableLoader<long, TEditable>,
							IEntityUpdator<long, TEditable>,
							IEntityCreator<long, TEditable>
		{
			return EnsureEntityEx<TManager,long,TEditable,TQueryArgument>(Manager, QueryArgument, Updater);
		}
		public static async Task<TEditable> EnsureEntityEx<TManager, TKey, TEditable,  TQueryArgument>(
		   this TManager Manager,
		   TQueryArgument QueryArgument,
		   Action<TEditable> Updater 
		   )
		   where TKey : IEquatable<TKey>
		   where TEditable : class, IEntityWithId<TKey>,new()
		   where TQueryArgument : class, IQueryArgument<TKey>
			where TManager : IEntityIdentQueryable<TKey, TQueryArgument>,
							IEntityEditableLoader<TKey, TEditable>,
							IEntityUpdator<TKey, TEditable>,
							IEntityCreator<TKey, TEditable>
		{
			return await Manager.EnsureEntity<TManager,TKey, TEditable>(
				await Manager.ResolveEntity(QueryArgument),
				()=>
				{
					var o = new TEditable();
					Updater(o);
					return o;
				},
				Updater
				);
		}
		public static async Task QueryAndRemoveAsync<TManager, TKey, TQueryArgument>(
			this TManager Manager,
			ITransactionScopeManager transScopeManager,
			TQueryArgument QueryArgument =null,
			int BatchCount=100
			)
			where TKey : IEquatable<TKey>
			where TQueryArgument : class, IQueryArgument<TKey>,new()
			where TManager : IEntityIdentQueryable<TKey, TQueryArgument>, IEntityRemover<TKey>
		{
			var paging = new Paging { Count = BatchCount };
			var arg = QueryArgument ?? new TQueryArgument();
			for(;;)
			{
				using (var scope = await transScopeManager.CreateScope("批量删除", TransactionScopeMode.RequireTransaction))
				{
					var re = await Manager.QueryIdentsAsync(arg, paging);
					var ids = re.Items.ToArray();
					foreach (var id in ids)
						await Manager.RemoveAsync(id);

					await scope.Commit();
					if (ids.Length == 0)
						break;
				}
			}
		}
	
	}
}