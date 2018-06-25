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

using SF.Sys.Linq;
using SF.Sys.Threading;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Sys.Data
{
	
	public static class DataContextExtension
	{
		class CommitTracker : ITransactionCommitTracker
		{
			public TransactionCommitNotifyType TrackNotifyTypes { get; }
			public Func<TransactionCommitNotifyType, Exception, Task> Callback { get; }
			public CommitTracker(TransactionCommitNotifyType TrackNotifyTypes, Func<TransactionCommitNotifyType, Exception, Task> Callback)
			{
				this.TrackNotifyTypes = TrackNotifyTypes;
				this.Callback = Callback;
			}
			public Task Notify(TransactionCommitNotifyType Type, Exception Exception)
			{
				return Callback(Type, Exception);
			}
		}
		public static void AddCommitTracker(
			this IDataContext Context,
			TransactionCommitNotifyType TrackNotifyTypes,
			Func<TransactionCommitNotifyType, Exception, Task> Callback
			)
		{
			Context.AddCommitTracker(new CommitTracker(TrackNotifyTypes, Callback));
		}
		public static void AddCommitTracker(
			this IDataContext Context,
			TransactionCommitNotifyType TrackNotifyTypes,
			Action<TransactionCommitNotifyType, Exception> Callback
			)
		{
			Context.AddCommitTracker(
				new CommitTracker(
					TrackNotifyTypes,
					(t, e) => {
						Callback(t, e);
						return Task.CompletedTask;
					}
				)
			);
		}

		public static IQueryable<TModel> Queryable<TModel>(this IDataContext Context,bool ReadOnly=true) where TModel:class
			=> Context.Set<TModel>().AsQueryable(ReadOnly);

		
		public static T Update<T>(this IDataContext Context, T Entity) where T : class
			=> Context.Set<T>().Update(Entity);

		public static T Add<T>(this IDataContext Context, T Entity) where T : class
			=> Context.Set<T>().Add(Entity);

		public static void AddRange<T>(this IDataContext Context, IEnumerable<T> Entities) where T : class
		{
			var set = Context.Set<T>();
			foreach (var e in Entities)
				set.Add(e);
		}

		public static void Remove<T>(this IDataContext Context, T Entity) where T:class
			=> Context.Set<T>().Remove(Entity);
		public static void RemoveRange<T>(this IDataContext Context, IEnumerable<T> Entity) where T : class
			=> Context.Set<T>().RemoveRange(Entity);

		public static Task<T> AddOrUpdateAsync<T>(
			this IDataContext Context,
			System.Linq.Expressions.Expression<Func<T, bool>> filter,
			Func<Task<T>> creator,
			Func<T, Task> updater = null,
			bool AutoSave = true
			) where T : class
		{
			var set = Context.Set<T>();
			return set.AddOrUpdateAsync(filter, creator, updater, AutoSave);
		}

		public static async Task<TKey> GetOrCreateAtomEntityId<TKey,TModel>(
			this IDataScope DataScope,
			string AtomType,
			Expression<Func<TModel,bool>> Filter,
			Expression<Func<TModel,TKey>> KeySelector,
			Func<Task<TModel>> Creator
			)
			where TModel:class
		{
			var id = await DataScope.Use(
				$"获取{AtomType}ID",
				ctx => ctx.Queryable<TModel>()
					.Where(Filter)
					.Select(KeySelector)
					.SingleOrDefaultAsync(),
				DataContextFlag.LightMode
				);
			if (!id.IsDefault())
				return id;

			var re = await DataScope.Retry(
				$"创建{AtomType}操作", 
				ctx => ctx.AddOrUpdateAsync(
					Filter,
					Creator
					)
				);

			id = await DataScope.Use(
				$"再次获取{AtomType}ID",
				ctx => ctx.Queryable<TModel>()
					.Where(Filter)
					.Select(KeySelector)
					.SingleOrDefaultAsync(),
				DataContextFlag.LightMode
				);
			if (!id.IsDefault())
				return id;
			throw new InvalidOperationException("成功运行对象生成过程,但任然找不到对象");
		}

		public static IEnumerable<string> GetUnderlingCommandTexts<T>(
			this IQueryable<T> Queryable
			) where T : class
		{
			var ext=((IEntityQueryProvider) Queryable.Provider).DataContext as IDataContextExtension;
			if (ext == null)
				return Enumerable.Empty<string>();
			return ext.GetUnderlingCommandTexts(Queryable);
		}

		//public static Task<T> UseTransaction< T>(
		//	this IDataContext Context,
		//	string TransMessage,
		//	Func<DbTransaction,Task<T>> Action
		//	)
		//{
		//	var tm = Context.TransactionScopeManager;
		//	return tm.UseTransaction(
		//		TransMessage,
		//		async s =>
		//		{
		//			var tran = tm.CurrentDbTransaction;
		//			var provider = Context.Provider;
		//			var orgTran = provider.Transaction;
		//			if (orgTran == tran)
		//				return await Action(tran);

		//			provider.Transaction = tran;
		//			T re;
		//			try
		//			{
		//				re=await Action(tran);
		//			}
		//			catch
		//			{
		//				provider.Transaction = orgTran;
		//				throw;
		//			}

		//			//不能在此时将事务修改回来，否者在CommitTracker中访问DataContext时，会发生Command没有事务的异常
		//			try
		//			{
		//				tm.AddCommitTracker(
		//					TransactionCommitNotifyType.BeforeCommit | TransactionCommitNotifyType.Rollback,
		//					(t, e) =>
		//					{
		//						provider.Transaction = orgTran;
		//					});
		//				return re;
		//			}
		//			catch
		//			{
		//				provider.Transaction = orgTran;
		//				throw;
		//			}
		//		}
		//	);
		//}
	}
}
