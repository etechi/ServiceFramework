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

    public class DataContext1 : IDataContext,IDataContextExtension
    {
        IDataContextProvider _Provider;
		public IDataContextProvider Provider => _Provider;
        public IDataContextProviderFactory ProviderFactory { get; }
		public Lazy<ITransactionScopeManager> LazyTransactionScopeManager { get; }
		IServiceProvider ServiceProvider { get; }
		public DataContext1(
			IDataContextProviderFactory ProviderFactory,
			IServiceProvider ServiceProvider
			//Lazy<ITransactionScopeManager> TransactionScopeManager
			)
        {
			this.ServiceProvider = ServiceProvider;
            this.ProviderFactory = ProviderFactory;
            _Provider = ProviderFactory.Create(this);
			LazyTransactionScopeManager = new Lazy<ITransactionScopeManager>(() => ServiceProvider.Resolve<ITransactionScopeManager>());
			//this.LazyTransactionScopeManager = TransactionScopeManager;
		}
		IEnumerable<IDataSetProviderResetable> ListRestable(IDataSetProviderResetable resetable)
		{
			var item = resetable;
			if (item == null)
				yield break;
			yield return item;
			for (;;)
			{
				item = item.NextResetable;
				if (item == resetable)
					break;
				yield return item;
			}
		}
        public void Reset()
        {
            Dispose();
            _Provider = ProviderFactory.Create(this);
			foreach (var pair in Sets)
				foreach (var r in ListRestable(pair.Value))
					r.SetProvider=null;
        }
		Dictionary<Type, IDataSetProviderResetable> Sets { get; } = new Dictionary<Type, IDataSetProviderResetable>();

		public ITransactionScopeManager TransactionScopeManager => LazyTransactionScopeManager.Value;

		internal IDataSetProvider<T> GetSetProvider<T>() where T : class
		{
			var type = typeof(T);
			IDataSetProviderResetable r;
			if (!Sets.TryGetValue(type, out r))
				throw new InvalidOperationException();
			var p = r.SetProvider;
			if (p == null)
				r.SetProvider = p = Provider.SetProvider<T>();
			return (IDataSetProvider<T>)p;
		}
		internal void RegisterDataSet<T>(DataSet<T> Set) where T:class
		{
			var type = typeof(T);
			var r = (IDataSetProviderResetable)Set;
			if (r.NextResetable != null)
				throw new InvalidOperationException();
			IDataSetProviderResetable head;
			if (Sets.TryGetValue(type, out head))
			{
				r.NextResetable = head.NextResetable;
				head.NextResetable = r;
			}
			else {
				r.NextResetable = r;
				Sets.Add(type, r);
			}
		}


        public void Dispose()
        {
			if (_Provider != null)
			{
				_Provider.Dispose();
				_Provider = null;
			}
        }

        public IDataSet<T> Set<T>() where T : class
        {
			var type = typeof(T);
			IDataSetProviderResetable r;
			if (!Sets.TryGetValue(type, out r))
				r = new DataSet<T>(this);
			return (IDataSet<T>)r;
		}

        public int SaveChanges()
        {
			return _Provider.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
			return await _Provider.SaveChangesAsync();
        }

       

        public void UpdateFields<T>(T item, Func<IFieldUpdater<T>, IFieldUpdater<T>> updater) where T : class
        {
            //_internalDataContext.UpdateFields(item, updater);
        }

        public object GetEntityOriginalValue(object Entity, string Field)
        {
            return ((IDataContextExtension)_Provider).GetEntityOriginalValue(Entity, Field);
        }

        public string GetEntitySetName<T>() where T : class
        {
            return ((IDataContextExtension)_Provider).GetEntitySetName<T>();
        }

		public void ClearTrackingEntities()
		{
			_Provider.ClearTrackingEntities();
		}

		public IEnumerable<string> GetUnderlingCommandTexts<T>(IContextQueryable<T> Queryable) where T : class
		{
			return ((IDataContextExtension)_Provider).GetUnderlingCommandTexts(Queryable);
		}

		public DbConnection GetDbConnection()
		{
			return ((IDataContextExtension)_Provider).GetDbConnection();
		}
	}
}
