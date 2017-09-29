using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Data;

namespace SF.Entities
{
	public static class EntityManagerExtension
	{
		public static async Task<TKey> QuerySingleEntityIdent<TKey, TQueryArgument>(
			this IEntityIdentQueryable<TKey, TQueryArgument> EntityQueryable,
			TQueryArgument QueryArgument
			)
			where TQueryArgument : class
		{
			var re = await EntityQueryable.QueryIdentsAsync(QueryArgument, Paging.Default);
			var res = re.Items.ToArray();
			if (res.Length == 0)
				return default(TKey);
			if (res.Length > 1)
				throw new InvalidOperationException("查询条件返回多条记录");
			return res[0];
		}

		public static Task<TEditable> UpdateEntity<TKey,TEditable>(
			this IEntityManager<TEditable> manager,
			TKey Id,
			Action<TEditable> Editor
			) where TKey : IEquatable<TKey>
			where TEditable : class
			=> manager.UpdateEntity(manager, Id, Editor);

		public static async Task<TEditable> UpdateEntity<TKey,TEditable>(
			this IEntityUpdator<TEditable> Updator,
			IEntityEditableLoader<TEditable> Loader,
			TKey Id,
			Action<TEditable> Editor
			)
			where TKey:IEquatable<TKey>
			where TEditable : class
		{
			var ins = await Loader.LoadForEdit(Entity<TEditable>.WithKey(Id));
			if (ins == null)
				throw new ArgumentException($"找不到对象:{typeof(TEditable)}:{Id}");
			Editor(ins);
			return await Updator.UpdateAsync(ins);
		}

		public static Task SetEntityState<TKey,TEditable>(
			this IEntityManager<TEditable> manager,
			TKey Id,
			EntityLogicState State
			)
			where TKey : IEquatable<TKey>
			where TEditable : class, IObjectEntity,new()
			=> manager.SetEntityState(manager, Id, State);

		public static async Task SetEntityState<TKey,TEditable>(
			this IEntityUpdator<TEditable> Updator,
			IEntityEditableLoader<TEditable> Loader,
			TKey Id,
			EntityLogicState State
			) 
			where TEditable : class, IObjectEntity,new()
			where TKey : IEquatable<TKey>
			=> await Updator.UpdateEntity(
				Loader,
				Id,
				e => e.LogicState = State
				);

		public static Task SetEntityName<TKey,TEditable>(
			this IEntityManager<TEditable> Manager,
			TKey Id,
			string Name
			) 
			where TEditable : class, IObjectEntity,new()
			where TKey:IEquatable<TKey>
			=> Manager.SetEntityName(Manager, Id, Name);

		public static async Task SetEntityName<TKey,TEditable>(
			this IEntityUpdator<TEditable> Updator,
			IEntityEditableLoader<TEditable> Loader,
			TKey Id,
			string Name
			)where TKey:IEquatable<TKey>
			where TEditable : class, IObjectEntity,new()
			=> await Updator.UpdateEntity<TKey,TEditable>(
				Loader,
				Id,
				(TEditable e) => e.Name = Name
				);
		public static Task SetEntityParent<TEditable,TKey>(
			this IEntityManager<TEditable> Manager,
			TKey Id,
			TKey ParentId
			) where TKey : struct, IEquatable<TKey>
			where TEditable : class, IEntityWithId<TKey>, IItemEntity<TKey?>
				=> Manager.SetEntityParent(Manager, Id, ParentId);

		public static async Task SetEntityParent<TKey, TEditable>(
			this IEntityUpdator<TEditable> Updator,
			IEntityEditableLoader<TEditable> Loader,
			TKey Id,
			TKey? ParentId
			) where TKey : struct,IEquatable<TKey>
			where TEditable : class, IItemEntity<TKey?>
			=> await Updator.UpdateEntity(
				Loader,
				Id,
				(TEditable e) => e.ContainerId=ParentId
				);

		public static async Task<TEditable> EnsureEntity<TManager,TKey, TEditable>(
			this TManager Manager,
			TKey Id,
			Func<TEditable> Creator,
			Action<TEditable> Updater=null
			)
			where TEditable:class
			where TManager: IEntityEditableLoader<TEditable>,
							IEntityUpdator<TEditable>,
							IEntityCreator<TEditable>
		{
			
			var hasInstance = !EqualityComparer<TKey>.Default.Equals(Id, default(TKey));
			TEditable ins;
			if (hasInstance)
			{
				ins = await Manager
					.LoadForEdit(Entity<TEditable>.WithKey(Id))
					.IsNotNull(() => $"不存在实体{typeof(TEditable)}实例:{Id} ");
			}
			else
				ins = Creator();
			if (Updater != null)
				Updater(ins);
			if (hasInstance)
				await Manager.UpdateAsync(ins);
			else
				ins = await Manager.CreateAsync(ins);
			return await Manager.LoadForEdit(ins);
		}
		public static Task<TEditable> EnsureEntity<TKey, TEditable>(
			this IEntityManager<TEditable> Manager,
			TKey Id,
			Action<TEditable> Updater = null
			)
			where TKey : IEquatable<TKey>
			where TEditable : class, IEntityWithId<TKey>, new()
			=> Manager.EnsureEntity<IEntityManager<TEditable>,TKey,TEditable>(Id, Updater);

		public static Task<TEditable> EnsureEntity<TManager,TKey, TEditable>(
			this TManager Manager,
			TKey Id,
			Action<TEditable> Updater = null
			)
			where TEditable : class, new()
			where TManager : IEntityEditableLoader< TEditable>,
							IEntityUpdator<TEditable>,
							IEntityCreator<TEditable>
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