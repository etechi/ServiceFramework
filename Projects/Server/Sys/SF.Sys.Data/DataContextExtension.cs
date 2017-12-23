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
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Sys.Data
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

		public static Task<T> UseTransaction< T>(
			this IDataContext Context,
			string TransMessage,
			Func<DbTransaction,Task<T>> Action
			)
		{
			var tm = Context.TransactionScopeManager;
			return tm.UseTransaction(
				TransMessage,
				async s =>
				{
					var tran = tm.CurrentDbTransaction;
					var provider = Context.Provider;
					var orgTran = provider.Transaction;
					if (orgTran == tran)
						return await Action(tran);

					provider.Transaction = tran;
					try
					{
						return await Action(tran);
					}
					finally
					{
						provider.Transaction = orgTran;
					}

				}
			);
		}
	}
}
