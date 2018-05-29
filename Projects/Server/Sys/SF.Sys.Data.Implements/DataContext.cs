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

using SF.Sys.Services;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.Data
{
    public class DataContext : IDataContext,IDataContextExtension
	{
		public int ScopeId { get; }
		public int ContextId { get; }

		public string Name { get; }
        public IDataContextProvider Provider { get; }
		bool _Disposed;

		public RootDataContext RootContext { get; }
		public DataContext PrevContext { get; }
		protected void CheckDispose()
		{
			if (_Disposed)
				throw new ObjectDisposedException(GetType().FullName);
		}

		public DataContext(
			int ScopeId,
			int ContextId,
			string Name,
			IDataContextProvider Provider,
			RootDataContext RootContext,
			DataContext PrevContext
			)
		{
			this.ScopeId = ScopeId;
			this.ContextId = ContextId;

			this.Name = Name;
			this.Provider = Provider;
			this.RootContext = RootContext;
			this.PrevContext = PrevContext;
		}
        protected virtual void OnDisposed()
		{

		}
        public void Dispose()
        {
			if (_Disposed) return;
			_Disposed = true;
			OnDisposed();
        }
		Dictionary<Type, IDataSet> Sets { get; } = new Dictionary<Type, IDataSet>();

		public virtual IDataContextTransaction Transaction => RootContext.Transaction;

		public IDataSet<T> Set<T>() where T : class
		{
			CheckDispose();
			var type = typeof(T);
			if (Sets.TryGetValue(type, out var re))
				return (IDataSet<T>)re;
			re = Provider.CreateDataSet<T>(this);
			Sets.Add(type, re);
			return (IDataSet<T>)re;
		}

		
		public virtual Task SaveChangesAsync()
        {
			CheckDispose();
			return Task.CompletedTask;
			//return await Provider.SaveChangesAsync();
        }

		public virtual void AddCommitTracker(
			ITransactionCommitTracker Tracker
			)
		{
			CheckDispose();
			RootContext.AddCommitTracker(Tracker);
		}


		public void UpdateFields<T>(T item, Func<IFieldUpdater<T>, IFieldUpdater<T>> updater) where T : class
        {
            //_internalDataContext.UpdateFields(item, updater);
        }

        public object GetEntityOriginalValue(object Entity, string Field)
        {
			CheckDispose();
			return ((IDataContextExtension)Provider).GetEntityOriginalValue(Entity, Field);
        }

        public string GetEntitySetName<T>() where T : class
        {
			CheckDispose();
			return ((IDataContextExtension)Provider).GetEntitySetName<T>();
        }

		public void ClearTrackingEntities()
		{
			CheckDispose();
			Provider.ClearTrackingEntities();
		}

		public IEnumerable<string> GetUnderlingCommandTexts<T>(IContextQueryable<T> Queryable) where T : class
		{
			CheckDispose();
			return ((IDataContextExtension)Provider).GetUnderlingCommandTexts(Queryable);
		}

		public DbConnection GetDbConnection()
		{
			CheckDispose();
			return ((IDataContextExtension)Provider).GetDbConnection();
		}
	}
}
