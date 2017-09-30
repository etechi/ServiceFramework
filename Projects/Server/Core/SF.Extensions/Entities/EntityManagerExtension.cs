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

		public static Task UpdateEntity<TKey,TEditable>(
			this IEntityManager<TKey,TEditable> manager,
			TKey Id,
			Action<TEditable> Editor
			) 
			where TEditable : class
			=> manager.UpdateEntity(manager, Id, Editor);

		public static async Task UpdateEntity<TKey,TEditable>(
			this IEntityUpdator<TEditable> Updator,
			IEntityEditableLoader<TKey,TEditable> Loader,
			TKey Id,
			Action<TEditable> Editor
			)
			where TEditable : class
		{
			var ins = await Loader.LoadForEdit(Id);
			if (ins == null)
				throw new ArgumentException($"找不到对象:{typeof(TEditable)}:{Id}");
			Editor(ins);
			await Updator.UpdateAsync(ins);
		}

		public static Task SetEntityState<TKey,TEditable>(
			this IEntityManager<TKey,TEditable> manager,
			TKey Id,
			EntityLogicState State
			)
			where TEditable : class, IObjectEntity,new()
			=> manager.SetEntityState(manager, Id, State);

		public static async Task SetEntityState<TKey,TEditable>(
			this IEntityUpdator<TEditable> Updator,
			IEntityEditableLoader<TKey, TEditable> Loader,
			TKey Id,
			EntityLogicState State
			) 
			where TEditable : class, IObjectEntity,new()
			=> await Updator.UpdateEntity(
				Loader,
				Id,
				e => e.LogicState = State
				);

		public static Task SetEntityName<TKey,TEditable>(
			this IEntityManager<TKey, TEditable> Manager,
			TKey Id,
			string Name
			) 
			where TEditable : class, IObjectEntity,new()
			=> Manager.SetEntityName(Manager, Id, Name);

		public static async Task SetEntityName<TKey,TEditable>(
			this IEntityUpdator<TEditable> Updator,
			IEntityEditableLoader<TKey, TEditable> Loader,
			TKey Id,
			string Name
			)
			where TEditable : class, IObjectEntity,new()
			=> await Updator.UpdateEntity<TKey,TEditable>(
				Loader,
				Id,
				(TEditable e) => e.Name = Name
				);
		public static Task SetEntityParent<TKey, TEditable, TParentKey>(
			this IEntityManager<TKey, TEditable> Manager,
			TKey Id,
			TParentKey? ParentId
			)
			where TEditable : class,  IItemEntity<TParentKey?>
			where TParentKey : struct, IEquatable<TParentKey>
				=> Manager.SetEntityParent(Manager, Id, ParentId);

		public static async Task SetEntityParent<TKey, TEditable, TParentKey>(
			this IEntityUpdator<TEditable> Updator,
			IEntityEditableLoader<TKey, TEditable> Loader,
			TKey Id,
			TParentKey? ParentId
			) 
			where TEditable : class, IItemEntity<TParentKey?>
			where TParentKey:struct,IEquatable<TParentKey>
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
			where TManager: IEntityEditableLoader<TKey, TEditable>,
							IEntityUpdator<TEditable>,
							IEntityCreator<TKey, TEditable>
		{

			var hasInstance = !Id.IsDefault();
			TEditable ins;
			if (hasInstance)
			{
				ins = await Manager
					.LoadForEdit(Id)
					.IsNotNull(() => $"不存在实体{typeof(TEditable)}实例:{Id} ");
			}
			else
				ins = Creator();
			if (Updater != null)
				Updater(ins);
			if (hasInstance)
				await Manager.UpdateAsync(ins);
			else
				Id=await Manager.CreateAsync(ins);
			return await Manager.LoadForEdit(Id);
		}
		public static Task<TEditable> EnsureEntity<TKey, TEditable>(
			this IEntityManager<TKey,TEditable> Manager,
			TKey Id,
			Action<TEditable> Updater = null
			)
			where TEditable : class,new()
			=> Manager.EnsureEntity<IEntityManager<TKey,TEditable>,TKey,TEditable>(Id, Updater);

		public static Task<TEditable> EnsureEntity<TManager,TKey, TEditable>(
			this TManager Manager,
			TKey Id,
			Action<TEditable> Updater = null
			)
			where TEditable : class, new()
			where TManager : IEntityEditableLoader<TKey, TEditable>,
							IEntityUpdator<TEditable>,
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