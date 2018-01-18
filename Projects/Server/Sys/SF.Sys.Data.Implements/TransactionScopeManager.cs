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
using System.Threading.Tasks;

namespace SF.Sys.Data
{
	
    public class TransactionScopeManager : ITransactionScopeManager
    {
		DbConnection Connection { get; }

		public DbTransaction CurrentDbTransaction => _DBTransaction;

		public TransactionScopeManager(DbConnection Connection)
        {
            this.Connection = Connection;
        }

		class Scope : ITransactionScope
        {
            public TransactionScopeManager Manager { get; }
            public Scope PrevScope { get; }
            public string Message { get; }
			public int Level { get; }
            public bool IsRollbacking { get { return Manager._IsRollbacking; } }
			public Scope(TransactionScopeManager Manager, Scope PrevScope,string Message,int Level)
            {
				this.Level = Level;
                this.Manager = Manager;
                this.PrevScope = PrevScope;
                this.Message = Message;
            }
		}
		
		DbTransaction _DBTransaction;
        Scope _TopScope;
        bool _IsRollbacking;
		Exception _Exception;
		public ITransactionScope CurrentScope => _TopScope;
		List<ITransactionCommitTracker> _CommitTrackers;

		public void AddCommitTracker(
			ITransactionCommitTracker Tracker
			)
		{
			if (_TopScope == null)
				throw new InvalidOperationException("当前没有事务在执行");

			var pas = _CommitTrackers;
			if (pas == null)
				_CommitTrackers = pas = new List<ITransactionCommitTracker>();
			pas.Add(Tracker);
		}


		async Task TraceCommitAsync(List<ITransactionCommitTracker> trackers, TransactionCommitNotifyType Type, Exception exception)
		{
			if (trackers == null)
				return;
			foreach (var tracker in trackers)
			{
				if ((tracker.TrackNotifyTypes & Type) == Type)
					await tracker.Notify(Type, exception);
			}
		}
		async Task EndScope()
		{
			var scope = _TopScope;
			_TopScope = scope.PrevScope;
			//顶层事务范围
			if (_TopScope == null)
			{
				var expr = _Exception;
				var isRollbacking = _IsRollbacking;
				var trans = _DBTransaction;
				var trackers = _CommitTrackers;
				_CommitTrackers = null;
				_Exception = null;
				_IsRollbacking = false;
				_DBTransaction = null;

				using (trans)
				{
					//内部已经rollback
					if (isRollbacking)
						await TraceCommitAsync(trackers,TransactionCommitNotifyType.Rollback, expr);
					//没有错误
					else if (expr == null)
					{
						await TraceCommitAsync(trackers, TransactionCommitNotifyType.BeforeCommit, null);
						try
						{
							trans.Commit();
							await TraceCommitAsync(trackers, TransactionCommitNotifyType.AfterCommit, null);
						}
						catch (Exception e)
						{
							await TraceCommitAsync(trackers, TransactionCommitNotifyType.AfterCommit, e);
							throw;
						}
					}
					//内部没有rollback，但顶级有异常
					else
					{
						try
						{
							trans.Rollback();
						}
						finally
						{
							await TraceCommitAsync(trackers, TransactionCommitNotifyType.Rollback, expr);
						}
					}
				}
			}
			//内部事务范围
			else if (_Exception==null)
				return;
			else if (!_IsRollbacking)
			{
				_IsRollbacking = true;
				_DBTransaction.Rollback();
			}
		}
		public async Task UseTransaction(
			string Message,
			Func<ITransactionScope,Task> Action, 
			TransactionScopeMode Mode,
			System.Data.IsolationLevel IsolationLevel
			)
        {
            if (Mode == TransactionScopeMode.RequireNewTransaction && _DBTransaction != null)
                throw new InvalidOperationException("事务已存在");
			if (_DBTransaction == null)
			{
				if (Connection.State == System.Data.ConnectionState.Closed)
					await Connection.OpenAsync();
				_DBTransaction = Connection.BeginTransaction(IsolationLevel);
			}
			_TopScope = new Scope(this, _TopScope, Message, (_TopScope?.Level ?? -1) + 1);
			try
			{
				await Action(_TopScope);
			}
			catch(Exception e)
			{
				_Exception = e;
				throw;
			}
			finally
			{
				await EndScope();
			}
        }

        public void Dispose()
        {
            if (_DBTransaction != null)
            {
                var trans = _DBTransaction;
                _DBTransaction = null;
                using (trans)
                {
                    trans.Rollback();
                    throw new TransactioinRollbackException(_TopScope?.Message ?? "事务未正常结束");
                }
            }
        }
    }
}
