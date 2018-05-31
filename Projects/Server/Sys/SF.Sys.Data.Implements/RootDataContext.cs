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

using SF.Sys.Logging;
using SF.Sys.Services;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.Data
{
    public class RootDataContext : DataContext
	{
		public RootDataContext PrevRootContext { get; }
		public DataContextFlag Flags { get; }
		DataContext _TopLightContext;
		public override IDataContextTransaction Transaction { get; }
		public RootDataContext(
			int ScopeId,
			int ContextId,
			string Name,
			DataContextFlag Flags,
			IDataContextTransaction Transaction,
			IDataContextProvider Provider,
			RootDataContext PrevRootContext
			):base(ScopeId,ContextId,Name,Provider,null,null)
        {
			this.PrevRootContext = PrevRootContext;
			this.Flags = Flags;
			this.Transaction = Transaction;
		}
		
		public DataContext PushLightContext(
			int ScopeId,
			int ContextId,
			string Action
			)
		{
			return _TopLightContext = new DataContext(ScopeId,ContextId,Action, Provider, this, _TopLightContext);
		}
		public void PopLightContext(DataContext ctx)
		{
			if (ctx != _TopLightContext)
				throw new ArgumentException("指定的数据上下文不是顶层数据上下文");
			_TopLightContext = ctx.PrevContext;
		}

		protected override void OnDisposed()
		{
		}
		List<ITransactionCommitTracker> _CommitTrackers;
		
		public override void AddCommitTracker(ITransactionCommitTracker Tracker)
		{
			CheckDispose();
			if (_CommitTrackers == null)
				_CommitTrackers = new List<ITransactionCommitTracker>();
			_CommitTrackers.Add(Tracker);
		}
		
		async Task TraceCommitAsync(
			List<ITransactionCommitTracker> trackers,
			TransactionCommitNotifyType Type, 
			Exception exception
			)
		{
			if (trackers == null)
				return;
			foreach (var tracker in trackers)
			{
				if ((tracker.TrackNotifyTypes & Type) == Type)
					await tracker.Notify(Type, exception);
			}
		}
		public override async Task SaveChangesAsync()
		{
			CheckDispose();
			if (Transaction == null)
			{
				var trackers = _CommitTrackers;
				_CommitTrackers = null;
				try
				{
					await TraceCommitAsync(trackers,TransactionCommitNotifyType.BeforeCommit, null);
					await Provider.SaveChangesAsync();
					Provider.ClearTrackingEntities();
					await TraceCommitAsync(trackers, TransactionCommitNotifyType.AfterCommit, null);
				}
				catch (Exception ex)
				{
					await TraceCommitAsync(trackers, TransactionCommitNotifyType.AfterCommit, ex);
					throw;
				}
				
			}
			else
			{
				await Provider.SaveChangesAsync();
			}
		}
		public async Task EndUse(Exception error,ILogger Logger)
		{
			if (_TopLightContext != null)
				throw new InvalidOperationException("仍有内部数据上下文未释放");

			if (Transaction == null)
				return;
			var trackers = _CommitTrackers;
			_CommitTrackers = null;
			if (error == null)
			{
				try
				{
					await TraceCommitAsync(trackers,TransactionCommitNotifyType.BeforeCommit, null);
					Transaction.Commit();
					await TraceCommitAsync(trackers, TransactionCommitNotifyType.AfterCommit, null);
				}
				catch (Exception ex)
				{
					await TraceCommitAsync(trackers, TransactionCommitNotifyType.AfterCommit, ex);
					throw;
				}
			}
			//内部没有rollback，但顶级有异常
			else
			{
				try
				{
					Transaction.Rollback();
				}
				catch(Exception ex)
				{
					Logger.Warn(ex, () => $"事务回滚时发生异常:{ex.Message}");
				}
				finally
				{
					await TraceCommitAsync(trackers, TransactionCommitNotifyType.Rollback, error);
				}
			}
		}
	}
	
}
