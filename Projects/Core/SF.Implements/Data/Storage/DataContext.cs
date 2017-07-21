using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data.Storage
{

    public class DataContext : IDataContext,IDataContextExtension
    {
        IDataContextProvider _Provider;
		public IDataContextProvider Provider => _Provider;
        public IDataContextProviderFactory ProviderFactory { get; }

		public DataContext(IDataContextProviderFactory ProviderFactory)
        {
            this.ProviderFactory = ProviderFactory;
            _Provider = ProviderFactory.Create(this);
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
