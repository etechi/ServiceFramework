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
using System.Threading;
using System.Threading.Tasks;

namespace SF.Sys.Data
{
    public class DataContext : IDataContext,IDataContextExtension
	{
		public int ScopeId { get; }
		public int ContextId { get; }

		public string Name { get; }
		bool _Disposed;

        public DataContext TransContext => _TransContext;

		public DataContext PrevContext { get; }

        DataContext _TransContext;
        IDataContextTransaction _Transaction;

        IDataContextProvider _Provider;
        bool _ProviderOwner;
        public bool IsTransContext => _TransContext == this;
        Dictionary<Type, IDataSet> _Sets;
        bool _AutoSaveChangedRequred;

        public IDataContextTransaction Transaction => TransContext._Transaction;
        public IDataContextProvider Provider => _Provider;
        public IDataScope DataScope { get; }
        protected void CheckDispose()
		{
			if (_Disposed)
				throw new ObjectDisposedException(GetType().FullName);
		}
        
        DataContext(
			int ScopeId,
			int ContextId,
			string Name,
			DataContext PrevContext,
            IDataScope DataScope
            )
        {
			this.ScopeId = ScopeId;
			this.ContextId = ContextId;
			this.Name = Name;
            this.PrevContext = PrevContext;
            this.DataScope = DataScope;


        }
        public static async Task<DataContext> Create(
            int ScopeId,
            int ContextId,
            string Name,
            IDataScope DataScope,
            DataContext PrevContext,
            IDataContextProviderFactory ProviderFactory,
            DataContextFlag Flags,
            System.Data.IsolationLevel TransactionIsolationLevel
            )
        {
            var ctx = new DataContext(ScopeId, ContextId, Name,PrevContext, DataScope);
            System.Data.IsolationLevel prevTranIsolationLevel;
            if (PrevContext == null ||
                DataScope!=PrevContext.DataScope||
                (Flags & DataContextFlag.RequireNewTransaction) == DataContextFlag.RequireNewTransaction && 
                (PrevContext.Transaction != null || TransactionIsolationLevel==System.Data.IsolationLevel.Unspecified)
                )
            {
                ctx._Provider = ProviderFactory.Create();
                ctx._ProviderOwner = true;
                ctx._TransContext = ctx;
                prevTranIsolationLevel = System.Data.IsolationLevel.Unspecified;
            }
            else
            {
                prevTranIsolationLevel = PrevContext.Transaction?.IsolationLevel ?? System.Data.IsolationLevel.Unspecified;
                ctx._TransContext = PrevContext.TransContext;
                ctx._Provider = PrevContext.Provider;
            }
            if (TransactionIsolationLevel > prevTranIsolationLevel)
            {
                if (prevTranIsolationLevel != System.Data.IsolationLevel.Unspecified)
                    throw new NotSupportedException("不支持事务隔离级别提升");
                ctx._Transaction = await ((IDataContextProviderExtension)ctx.Provider).BeginTransaction(
                    TransactionIsolationLevel, 
                    CancellationToken.None
                    );
                ctx._TransContext = ctx;
                
            }
            return ctx;
        }
        protected virtual void OnDisposed()
		{

		}
        public void Dispose()
        {
			if (_Disposed) return;
			_Disposed = true;
			OnDisposed();
            if (_Transaction != null)
                _Transaction.Dispose();
            if (_Provider != null && _ProviderOwner)
                _Provider.Dispose();
        }
		public IDataSet<T> Set<T>() where T : class
		{
			CheckDispose();
			var type = typeof(T);
            var sets = _Sets;
            if (sets == null)
                _Sets = sets = new Dictionary<Type, IDataSet>();
			if (sets.TryGetValue(type, out var re))
				return (IDataSet<T>)re;
			re = Provider.CreateDataSet<T>(this);
			sets.Add(type, re);
			return (IDataSet<T>)re;
		}


		public void UpdateFields<T>(T item, Func<IFieldUpdater<T>, IFieldUpdater<T>> updater) where T : class
        {
            //_internalDataContext.UpdateFields(item, updater);
        }

        public object GetEntityOriginalValue(object Entity, string Field)
        {
			CheckDispose();
			return ((IDataContextProviderExtension)Provider).GetEntityOriginalValue(Entity, Field);
        }

        public string GetEntitySetName<T>() where T : class
        {
			CheckDispose();
			return ((IDataContextProviderExtension)Provider).GetEntitySetName<T>();
        }

		public void ClearTrackingEntities()
		{
			CheckDispose();
			Provider.ClearTrackingEntities();
		}

		public IEnumerable<string> GetUnderlingCommandTexts<T>(IQueryable<T> Queryable) where T : class
		{
			CheckDispose();
			return ((IDataContextProviderExtension)Provider).GetUnderlingCommandTexts(Queryable);
		}

		public DbConnection GetDbConnection()
		{
			CheckDispose();
			return ((IDataContextProviderExtension)Provider).GetDbConnection();
		}


        List<ITransactionCommitTracker> _CommitTrackers;

        public virtual void AddCommitTracker(ITransactionCommitTracker Tracker)
        {
            CheckDispose();
            var trackers = TransContext._CommitTrackers;
            if (trackers == null)
                TransContext._CommitTrackers = trackers=new List<ITransactionCommitTracker>();
            trackers.Add(Tracker);
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
        public virtual async Task SaveChangesAsync()
        {
            CheckDispose();
            if (!IsTransContext)
            {
                if (Transaction == null)
                    TransContext._AutoSaveChangedRequred = true;
                else
                {
                    await Provider.SaveChangesAsync();
                    Provider.ClearTrackingEntities();
                }
                return;
            }

            _AutoSaveChangedRequred = false;
            if (Transaction == null)
            {
                var trackers = _CommitTrackers;
                _CommitTrackers = null;
                try
                {
                    await TraceCommitAsync(trackers, TransactionCommitNotifyType.BeforeCommit, null);
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
        public async Task EndUsing(Exception error, ILogger Logger)
        {
            if (!IsTransContext)
                return;
            if (_AutoSaveChangedRequred)
                await SaveChangesAsync();
            if (Transaction == null)
                return;
            var trackers = _CommitTrackers;
            _CommitTrackers = null;
            if (error == null)
            {
                try
                {
                    await TraceCommitAsync(trackers, TransactionCommitNotifyType.BeforeCommit, null);
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
                catch (Exception ex)
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
