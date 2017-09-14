using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Data
{
	public static class DataContextExtension
	{
		
		public static Task<T> Retry<T>(this IDataContext Context,Func<System.Threading.CancellationToken, Task<T>> Action,int Timeout=6000000,int Retry=5)
		{
			return TaskUtils.Retry(async ct =>
			{
				try
				{
					return await Action(ct);
				}
				catch
				{
					Context.Reset();
					throw;
				}
			},Timeout,Retry);
		}
		public static void Remove<T>(this IDataContext Context, T Entity) where T:class
			=> Context.Set<T>().Remove(Entity);
		public static void RemoveRange<T>(this IDataContext Context, IEnumerable<T> Entity) where T : class
			=> Context.Set<T>().RemoveRange(Entity);

		public static async Task<T> UseTransaction< T>(
			this IDataContext Context,
			string TransMessage,
			Func<DbTransaction,Task<T>> Action
			)
		{
			var tm = Context.TransactionScopeManager;
			using (var ts = await tm.CreateScope(TransMessage, TransactionScopeMode.RequireTransaction))
			{
				var tran = tm.CurrentDbTransaction;
				var provider = Context.Provider;
				var orgTran = provider.Transaction;
				if (orgTran == tran)
					return await Action(tran);
				provider.Transaction = tran;
				try
				{
					var re= await Action(tran);
					await ts.Commit();
					return re;
				}
				finally
				{
					provider.Transaction = orgTran;
				}
				
			}
		}
	}
}
