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
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace SF.Data
{
	
	public static class TransactionScopeManagerExtensions
	{
		public static async Task<T> UseTransaction<T>(
			this ITransactionScopeManager ScopeManager, 
			string Message,
			Func<ITransactionScope,Task<T>> Callback,
			TransactionScopeMode Mode=TransactionScopeMode.RequireTransaction, 
			IsolationLevel IsolationLevel = IsolationLevel.ReadCommitted
			)
		{
			var re = default(T);
			await ScopeManager.UseTransaction(
				Message,
				async s =>
				{
					re = await Callback(s);
				},
				Mode,
				IsolationLevel
				);
			return re;
		}
		class CommitTracker : ITransactionCommitTracker
		{
			public TransactionCommitNotifyType TrackNotifyTypes { get; }
			public Func<TransactionCommitNotifyType,Exception,Task> Callback { get; }
			public CommitTracker(TransactionCommitNotifyType TrackNotifyTypes, Func<TransactionCommitNotifyType, Exception,Task> Callback)
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
			this ITransactionScopeManager ScopeManager, 
			TransactionCommitNotifyType TrackNotifyTypes, 
			Func<TransactionCommitNotifyType, Exception, Task> Callback
			)
		{
			ScopeManager.AddCommitTracker(new CommitTracker(TrackNotifyTypes, Callback));
		}
		public static void AddCommitTracker(
			this ITransactionScopeManager ScopeManager,
			TransactionCommitNotifyType TrackNotifyTypes,
			Action<TransactionCommitNotifyType, Exception> Callback
			)
		{
			ScopeManager.AddCommitTracker(
				new CommitTracker(
					TrackNotifyTypes, 
					(t,e)=> {
						Callback(t, e);
						return Task.CompletedTask;
					}
				)
			);
		}
	}
  
}
