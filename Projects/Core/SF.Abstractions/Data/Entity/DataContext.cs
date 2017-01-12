using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data.Entity
{

    public class DataContext : IDataContext,IDataContextExtension
    {
        IDataContextProvider _Provider;
		public IDataContextProvider Provider => _Provider;
        public Func<IDataContextProvider> ProviderFactory { get; }

		public DataContext(Func<IDataContextProvider> ProviderFactory)
        {
            this.ProviderFactory = ProviderFactory;
            _Provider = ProviderFactory();
        }
        public void Reset()
        {
            Dispose();
            _Provider = ProviderFactory();
        }


        public IDataStorageEngine Engine
        {
            get
            {
                return _Provider.Engine;
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

        public IDataSetEditable<T> Editable<T>() where T : class
        {
            return _Provider.Editable<T>();
        }

        public IDataSetReadonly<T> ReadOnly<T>() where T : class
        {
            return _Provider.ReadOnly<T>();
        }

        public int SaveChanges()
        {
            return _Provider.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return _Provider.SaveChangesAsync();
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
    }
}
