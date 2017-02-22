﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Data.Storage;

namespace SF.Data.Entity
{
	public static class EntityManagerExtension
	{
		public static async Task<TKey> ResolveEntity<TKey, TEntity, TQueryArgument>(
			this IEntityQueryable<TKey,TEntity,TQueryArgument> EntityQueryable,
			TQueryArgument QueryArgument
			)
			where TKey : IEquatable<TKey>
			where TQueryArgument : class, IQueryArgument<TKey>
			where TEntity : class, IObjectWithId<TKey>
		{
			var re = await EntityQueryable.QueryAsync(QueryArgument, Data.Paging.Default);
			var res = re.Items.ToArray();
			if (res.Length == 0)
				return default(TKey);
			if (res.Length > 1)
				throw new InvalidOperationException("查询条件返回多条记录");
			return res[0].Id;
		}
		public static async Task<TEditable> EnsureEntity<TKey, TEditable>(
			this IEntityManager<TKey,TEditable> Manager,
			TKey Id,
			Func<TEditable> Creator,
			Action<TEditable> Updater=null
			)
			where TKey:IEquatable<TKey>
			where TEditable:class,IObjectWithId<TKey>
		{
			
			var hasInstance = !EqualityComparer<TKey>.Default.Equals(Id, default(TKey));
			TEditable ins;
			if (hasInstance)
				ins = await Manager.LoadForEdit(Id);
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
		public static async Task<TEditable> EnsureEntityEx<TManager, TKey, TEditable,TPublic,TQueryArgument>(
			this TManager Manager,
			TQueryArgument QueryArgument,
			Func<TEditable> Creator,
			Action<TEditable> Updater = null
			)
			where TKey : IEquatable<TKey>
			where TEditable : class, IObjectWithId<TKey>
			where TPublic : class,IObjectWithId<TKey>
			where TQueryArgument:class,IQueryArgument<TKey>
			where TManager: IEntityQueryable<TKey, TPublic, TQueryArgument>,IEntityManager<TKey,TEditable>
		{
			return await Manager.EnsureEntity(
				await Manager.ResolveEntity(QueryArgument),
				Creator,
				Updater
				);
		}
			
	}
}