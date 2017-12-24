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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SF.Sys.Services;
using SF.Sys.Linq.Expressions;
using SF.Sys.Collections.Generic;
using SF.Sys.ADT;

namespace SF.Sys.Data
{

	public static class DataSetExtension
	{
		public static void AddRange<T>(this IDataSet<T> set, IEnumerable<T> items, Action<T> init = null) where T : class
		{
			if (items == null) return;
			if (init != null)
				foreach (var it in items)
					init(it);
			set.AddRange(items);
		}

		class EntityKeyGetter<T> where T:class
		{
			static Func<T, object[]> _EntityPrimaryKey;
			public static object[] GetIdent(IDataSet<T> set, T Entity)
			{
				var epk = _EntityPrimaryKey;
				if (epk == null)
				{
					var arg = Expression.Parameter(typeof(T), "o");
					var props = set.Metadata
						.Key
						.Select(x => x.PropertyInfo);
					_EntityPrimaryKey = epk = Expression.Lambda<Func<T, object[]>>(
						Expression.NewArrayInit(
							typeof(object),
							props.Select(
								p => Expression.Convert(
									Expression.Property(arg, p),
									typeof(object)
									)
								).ToArray()
						),
						arg)
						.Compile();
				}
				return epk(Entity);
			}
		}



		public static object[] GetIdent<T>(this IDataSet<T> set,T Entity) where T:class
		{
			return EntityKeyGetter<T>.GetIdent(set, Entity);
		}

		public static async Task<T> Update<T,K>(
			this IDataSet<T> set,
			K id,
			Action<T> updater,
			bool Save=true
			)
			where T:class
			where K:IEquatable<K>
		{
			var e = await set.FindAsync(id);
			if (e == null)
				throw new ArgumentException($"找不到对象:{typeof(T)}:{id}");
			updater(e);
			set.Update(e);
			if (Save)
				await set.Context.SaveChangesAsync();
			return e;
		}
		public static async Task<T> Update<T>(
		   this IDataSet<T> set,
		   Expression<Func<T, bool>> filter,
		   Action<T> updater,
		   bool Save = true
		   )where T:class
		{
			var e = await set.AsQueryable(false).Where(filter).SingleOrDefaultAsync();
			if(e==null)
				throw new ArgumentException($"找不到对象:{typeof(T)}:{filter}");
			updater(e);
			set.Update(e);
			if (Save)
				await set.Context.SaveChangesAsync();
			return e;
		}
		public static Task<T> FindAsync<T>(
			this IDataSet<T> set, 
			Expression<Func<T, bool>> filter
			)
			where T:class
		{
			return set.AsQueryable(false).SingleOrDefaultAsync(filter);
		}
		public static Task<V> FieldAsync<T,V>(
			this IDataSet<T> set,
			Expression<Func<T, bool>> filter,
			Expression<Func<T,V>> map
			)
			where T : class
		{
			return set.AsQueryable().Where(filter).Select(map).SingleOrDefaultAsync();
		}
		
		public static Task<T[]> LoadListAsync<T>(
			this IDataSet<T> set,
			System.Linq.Expressions.Expression<Func<T, bool>> filter
			)
			where T : class
		{
			return set.AsQueryable(false).Where(filter).ToArrayAsync();
		}
		public static Task<T[]> LoadListAndOrderByAsync<T,F>(
		   this IDataSet<T> set,
		   System.Linq.Expressions.Expression<Func<T, bool>> filter,
		   System.Linq.Expressions.Expression<Func<T, F>> order
		)
		   where T : class
		{
			return set.AsQueryable(false).Where(filter).OrderBy(order).ToArrayAsync();
		}
		public static Task<T[]> LoadListAndOrderByDescendingAsync<T, F>(
		   this IDataSet<T> set,
		   System.Linq.Expressions.Expression<Func<T, bool>> filter,
		   System.Linq.Expressions.Expression<Func<T, F>> order
		)
		   where T : class
		{
			return set.AsQueryable(false).Where(filter).OrderByDescending(order).ToArrayAsync();
		}

		public static async Task RemoveRangeAsync<T>(
			this IDataSet<T> set,
			System.Linq.Expressions.Expression<Func<T, bool>> filter,
			bool Save=true
		   )
			where T:class
		{

			var items = await set.LoadListAsync(filter);
			if (items.Length > 0)
			{
				set.RemoveRange(items);
				if(Save)
					await set.Context.SaveChangesAsync();
			}
		}
		public static Task<M[]> QueryAsync<T,M>(
		   this IDataSet<T> set,
		   System.Linq.Expressions.Expression<Func<T, bool>> filter,
		   System.Linq.Expressions.Expression<Func<T, M>> map
		   )
		   where T : class
		{
			return set.AsQueryable(true).Where(filter).Select(map).ToArrayAsync();
		}
		public static Task<T[]> QueryAsync<T>(
		   this IDataSet<T> set,
		   System.Linq.Expressions.Expression<Func<T, bool>> filter
		   )
		   where T : class
		{
			return set.AsQueryable(true).Where(filter).ToArrayAsync();
		}
		public static Task<M> FirstOrDefaultAsync<T, M>(
		   this IDataSet<T> set,
		   System.Linq.Expressions.Expression<Func<T, bool>> filter,
		   System.Linq.Expressions.Expression<Func<T, M>> map = null
		   )
		   where T : class
		{
			return set.AsQueryable(true).Where(filter).Select(map).FirstOrDefaultAsync();
		}
		public static Task<T> FirstOrDefaultAsync<T>(
		   this IDataSet<T> set,
		   System.Linq.Expressions.Expression<Func<T, bool>> filter
		   )
		   where T : class
		{
			return set.AsQueryable(true).Where(filter).FirstOrDefaultAsync();
		}
		public static Task<M> SingleOrDefaultAsync<T, M>(
		   this IDataSet<T> set,
		   System.Linq.Expressions.Expression<Func<T, bool>> filter,
		   System.Linq.Expressions.Expression<Func<T, M>> map = null
		   )
		   where T : class
		{
			return set.AsQueryable(true).Where(filter).Select(map).SingleOrDefaultAsync();
		}
		public static Task<T> SingleOrDefaultAsync<T>(
		   this IDataSet<T> set,
		   System.Linq.Expressions.Expression<Func<T, bool>> filter
		   )
		   where T : class
		{
			return set.AsQueryable(true).Where(filter).SingleOrDefaultAsync();
		}

		public static async Task<T> GetOrCreateAtomEntity<T>(
			this IDataSet<T> set, 
			IScoped<IDataContext> DataContex, 
			Expression<Func<T, bool>> Filter,
			Func<Task<T>> Creater
			)
			where T:class
		{
			var re = await set.AsQueryable().Where(Filter).SingleOrDefaultAsync();
			if (re != null)
				return re;
			for (; ; )
			{
				try
				{
					return await DataContex.Use(async ctx =>
					{
						var iset = ctx.Set<T>();
						for (; ; )
						{
							var ire = await iset.AsQueryable().Where(Filter).SingleOrDefaultAsync();
							if (ire != null)
								return re;

							iset.Add(await Creater());
							await ctx.SaveChangesAsync();
						}
					});
				}
				catch (DbDuplicatedKeyException)
				{
					continue;
				}
			}
		}

		public static async Task<T> EnsureAsync<T>(
			this IDataSet<T> set,
			T item,
			bool UpdateRequired = false
			)where T:class
		{
			var id = set.GetIdent(item);
			var e = await set.FindAsync(id);
			if (e == null)
				set.Add(item);
			else if (!UpdateRequired)
				return e;
			else
				set.Update(item);
			await set.Context.SaveChangesAsync();
			return item;
		}
		public static Task<T> EnsureAsync<T>(
			this IDataSet<T> set,
			System.Linq.Expressions.Expression<Func<T, bool>> filter,
			Func<Task<T>> creator
			) where T : class
		{
			return set.AddOrUpdateAsync(filter, creator, null);
		}

        public static async Task<T> AddOrUpdateAsync<T>(
			this IDataSet<T> set,
			System.Linq.Expressions.Expression<Func<T, bool>> filter,
			Func<Task<T>> creator,
			Func<T,Task> updater
			) where T : class
		{
			var e = await set.AsQueryable(false).Where(filter).SingleOrDefaultAsync();
			if (e == null)
				e=set.Add(await creator());
			else if (updater == null)
				return e;
			else
			{
				await updater(e);
				set.Update(e);
			}
			await set.Context.SaveChangesAsync();
			return e;
		}

		public static async Task ValidateTreeParentAsync<T,TKey>(
			this IDataSet<T> set,
			string Title,
			TKey Id,
			TKey NewParentId,
			Func<TKey ,IContextQueryable<TKey>> GetParentId
			)
			where TKey:IEquatable<TKey>
			where T:class
		{
			//检查父节点不能是自身，或自身的子节点
			if (Id.Equals(default(TKey)) || NewParentId.Equals(default(TKey)))
				return;
			if (Id.Equals(NewParentId))
				throw new PublicArgumentException($"父{Title}不能是当前{Title}自己");

			for (;;)
			{
				var npid = await GetParentId(NewParentId).SingleOrDefaultAsync();
				if (npid.Equals(default(TKey)))
					break;
				if (npid.Equals(Id))
					throw new PublicArgumentException($"父{Title}不能是作为{Title}的子{Title}");
				NewParentId= npid;
			}
		}
        public static List<M> Merge<M, E>(
			this IDataSet<M> set,
			IEnumerable<M> orgItems,
            IEnumerable<E> newItems,
            Func<M, E, bool> Equals,
            Action<M, E> updater,
            Action<M> remover = null
            )
            where M : class, new()
            => set.Merge(
                orgItems,
                newItems,
                Equals,
                e =>
                {
                    var m = new M();
                    updater(m, e);
                    return m;
                },
                updater,
                remover
                );


        public static List<M> Merge<M,E>(
			this IDataSet<M> set,
			IEnumerable<M> orgItems,
			IEnumerable<E> newItems,
			Func<M, E, bool> Equals,
			Func<E,M> newItem,
			Action<M, E> updater=null,
			Action<M> remover = null
			)
			where M : class
		{
			if (orgItems != null)
			{
				orgItems = orgItems.ToArray();
				foreach (var i in orgItems.Where(i => newItems == null || !newItems.Any(ni => Equals(i, ni))).ToArray())
				{
					if (remover == null)
                        set.Remove(i);
                    else
                        remover(i);				
				}
			}

			if (newItems == null)
				return new List<M>();

			return newItems.Select(
				i =>
				{
					var org_item = orgItems?.Where(oi => Equals(oi, i)).SingleOrDefault();
					if (org_item == null)
					{
						return set.Add(newItem(i));
					}
					else
					{
						if (updater != null)
						{
							updater(org_item, i);
							set.Update(org_item);
						}
						return org_item;
					}
				}).ToList();
		}

		public static async Task<List<M>> MergeAsync<M, E>(
			this IDataSet<M> set,
			IEnumerable<M> orgItems,
			IEnumerable<E> newItems,
			Func<M, E, bool> Equals,
			Func<E, Task<M>> newItem,
			Func<M, E,Task> updater = null,
			Func<M,Task> remover = null
			)
			where M : class
		{
			if (orgItems != null)
			{
				orgItems = orgItems.ToArray();
				foreach (var i in orgItems)
				{
					if (newItems != null && newItems.Any(ni => Equals(i, ni)))
						continue;
					if (remover != null)
						await remover(i);
					set.Remove(i);
						
				}
			}

			if (newItems == null)
				return new List<M>();

			var re = new List<M>();
			foreach (var i in newItems) {
				var org_item = orgItems?.Where(oi => Equals(oi, i)).SingleOrDefault();
				if (org_item == null)
					re.Add(set.Add(await newItem(i)));
				else
				{
					if (updater != null)
					{
						await updater(org_item, i);
						set.Update(org_item);
					}
					re.Add(org_item);
				}
			}
			return re;
		}
		public static async Task<bool> TrySetDefaultItem<M>(
			this IDataSet<M> DataSet,
			M Model,
			bool NewDefaultMode,
			Func<M,bool> IsModelDefaultItem,
			Action<M,bool> SetDefault,
			Expression<Func<M,bool>> ScopeLimit,
			Expression<Func<M,bool>> IsCurRecord,
			Expression<Func<M,bool>> IsDefaultRecord
			)
			where M:class
		{
			if (IsModelDefaultItem(Model))
				return false;
			var curDefaultEntity = await DataSet.AsQueryable(false)
				.Where(ScopeLimit)
				.Where(Expression.Lambda<Func<M,bool>>(
					IsCurRecord.Body.Not(),
					IsCurRecord.Parameters
					))
				.Where(IsDefaultRecord)
				.SingleOrDefaultAsync();
			if (curDefaultEntity == null || NewDefaultMode)
			{
				SetDefault(Model,true);
				DataSet.Update(Model);
				if (curDefaultEntity != null)
				{
					SetDefault(curDefaultEntity,false);
					DataSet.Update(curDefaultEntity);
				}
				return true;
			}
			return false;
		}

		
		public static async Task<bool> ModifyPosition<M>(
			this IDataSet<M> DataSet,
			M Model,
			PositionModifyAction Mode,
			Expression<Func<M, bool>> ScopeLimit,
			Predicate<M> IsCurRecord,
			Func<M,int> GetPosition,
			Action<M,int> SetPosition
			)
			where M : class
		{
			var items = await DataSet.AsQueryable(false)
				.Where(ScopeLimit)
				.ToListAsync();
			items=items.OrderBy(GetPosition).ToList();

			return items.ModifyPosition(
				Model,
				Mode,
				IsCurRecord,
				GetPosition,
				(m, i) =>
				{
					SetPosition(m, i);
					DataSet.Update(items[i]);
				}
			);
		}
		public static Task<M[]> LoadTreeChildren<M>(
			this IDataSet<M> set,
			M cur,
			bool ReadOnly,
			Func<IContextQueryable<M>, M, IContextQueryable<M>> Query
			)where M:class

			=> Tree.LoadAllChildren(
				cur,
				i =>
					Query(set.AsQueryable(ReadOnly), i).ToArrayAsync()
				);

		static M CreateOrUpdateTreeNode<M,E,K>(
			this IDataSet<M> set,
			Dictionary<K,M> exists,
			M parent,
			E cur,
			Func<E,K> get_editable_ident,
			Func<E, M, M> new_item, //新节点在子节点创建前调用
			Action<M, E, M> updater,
			Func<E, IEnumerable<E>> get_children,
			List<M> results
			)
			where M : class
			where K : IEquatable<K>
		{

			if (exists.TryGetValue(get_editable_ident(cur), out var model))
			{
				updater(model, cur, parent);
				set.Update(model);
			}
			else
			{ 
				model = new_item(cur, parent);
				set.Add(model);
			}

			var children = get_children(cur);
			if(children!=null)
				foreach (var chd in children)
					CreateOrUpdateTreeNode<M, E, K>(
						set,
						exists,
						model,
						chd,
						get_editable_ident,
						new_item,
						updater,
						get_children,
						results
					);

			results.Add(model);
			return model;
		}
		public static List<M> MergeTree<M, E, K>(
			this IDataSet<M> set,
			M parent, //当前结点
			IEnumerable<M> org_items,	//旧节点，包含整颗树的节点
			IEnumerable<E> new_items,	//新节点，只包含树的顶层节点
			Func<M, K> get_model_ident,
			Func<E, K> get_editable_ident,
			Func<E, K> get_parent_ident,
			Func<E, IEnumerable<E>> get_children,   //获取新树节点的子节点
			Func<E, M, M> new_item, //新节点在子节点创建前调用，必须
			Action<M, E, M> updater = null,
			Action<M> remover = null
			)
			where M : class
			where K : IEquatable<K>
		{
			var org_item_dict = org_items.ToDictionary(i => get_model_ident(i));
			
			var updated_items = 
				ADT.Tree.AsEnumerable(new_items,i=>get_children(i))
				.Select(i=>(get_editable_ident(i),i))
				.Where(it => org_item_dict.ContainsKey(it.Item1))
				.ToDictionary(i =>i.Item1,i=>i.Item2);

			//删除节点
			foreach (var it in org_items.Where(i => !updated_items.ContainsKey(get_model_ident(i))))
			{
				remover?.Invoke(it);
				set.Remove(it);
			}

			var results = new List<M>();
			foreach (var ic in new_items)
				CreateOrUpdateTreeNode(
					set,
					org_item_dict,
					parent,
					ic,
					get_editable_ident,
					new_item,
					updater,
					get_children,
					results
					);


			////顶层新节点，需要新增
			//foreach (var it in new_items.Where(it => get_editable_ident(it).Equals(default(K))))
			//{
			//	var nm = NewTree(it, new_item(it, null);
			//	set.Add(nm);
			//	result.Add(nm);
			//}


			////非顶层新节点，如果父节点是已存在节点，需要新增
			////如果是未存在的节点，EF会自动新增
			//foreach (var it in all_items
			//	.Where(it => 
			//		get_editable_ident(it).Equals(default(K)) && 
			//		!get_parent_ident(it).Equals(default(K)) && 
			//		!new_items.Contains(it)
			//		)
			//	)
			//{
			//	var pnt = org_item_dict[get_parent_ident(it)];
			//	var nm=new_item(it, pnt);
			//	ctx.Add(nm);
			//	result.Add(nm);
			//	TreeCollect(nm,get_children(it), get_children, (i,p)=> new_item(i,p), result);
			//}
			
			////更新节点
			//foreach (var it in updated_items.Values)
			//{
			//	var oit = org_item_dict[get_editable_ident(it)];
			//	if (updater != null)
			//	{
			//		updater(oit, it);
			//		ctx.Update(oit);
			//	}
			//	result.Add(oit);
			//}
			return results;
		}

		//public static bool IsCollectionLoaded<T, C>(this IDataContext context, T user, System.Linq.Expressions.Expression<Func<T, ICollection<C>>> collection)
		//	where T : class
		//	where C : class
		//{
		//	return context.Entry(user).Collection(collection).IsLoaded;
		//}
		//public static async Task LoadCollection<T, C>(this IDataContext context, T entity, System.Linq.Expressions.Expression<Func<T, ICollection<C>>> collection)
		//	where T : class
		//	where C : class
		//{
		//	var col = context.Entry(entity).Collection(collection);
		//	if (col.IsLoaded) return;
		//	await col.LoadAsync();
		//	col.IsLoaded = true;
		//}
		//public static void RemoveRange<T>(this IDataContext Context, IEnumerable<T> items)
		//	where T:class
		//{
		//	Context.Set<T>().RemoveRange(items);

		//}
		
		public static async Task RetryForConcurrencyExceptionAsync<T>(
			this IDataSet<T> set,
			Func<Task> Action,
			int Retry = 5,
			int DelayMilliseconds = 500
			) where T : class
		{
			await set.RetryForConcurrencyExceptionAsync(
				async () =>
				{
					await Action();
					return 0;
				},
				Retry,
				DelayMilliseconds
				);
		}
		public static async Task<R> RetryForConcurrencyExceptionAsync<T,R>(
			this IDataSet<T> set,
			Func<Task<R>> Action,
            int Retry=5,
            int DelayMilliseconds=500
			) where T : class
		{
			//如果当前上下文有显示事务，无法进行通过重置上下文进行并发重试
			if (set.Context.Provider.Transaction != null)
				return await Action();

			var i = -1;
			Random rand = null;
            for(;;)
            {
                try
                {
                    return await Action();
                }
                catch(DbUpdateConcurrencyException)
                {
                    i++;
                    if (i >=Retry)
                        throw;
					set.Context.Reset();
                }
				if (rand == null)
					rand = RandomFactory.Create();
				await Task.Delay(DelayMilliseconds/2+rand.Next(DelayMilliseconds));
            }
        }
		public static async Task<TEntity> AddOrUpdateAsync<TKey,TEntity>(
			this IDataSet<TEntity> set,
			TKey key,
			Func<TEntity> Creator,
			Action<TEntity> Updater
			)
			where TEntity:class
		{
			var obj = await set.FindAsync(key);
			if (obj == null)
			{
				obj = Creator();
				Updater(obj);
				set.Add(obj);
			}
			else
			{
				Updater(obj);
				set.Update(obj);
			}
			await set.Context.SaveChangesAsync();
			return obj;
		}
		public static Task ValidateTreeParent<TKey>(
			this IDataSet context,
			string Title,
			TKey Id,
			TKey NewParentId,
			Func<TKey, IContextQueryable<TKey>> GetParentId
			) where TKey : IEquatable<TKey>
		{
			return Tree.ValidateTreeParent(
				Title,
				Id, 
				NewParentId, 
				id => GetParentId(id).SingleOrDefaultAsync()
				);
		}
	}
}
