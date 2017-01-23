using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data.Entity
{

    public interface IFieldUpdater<T>
    {
        IFieldUpdater<T> Update<P>(System.Linq.Expressions.Expression<Func<T, P>> field);
    }

    public interface IDataContext :IDisposable
	{
		IDataSet<T> Set<T>() where T : class;

		void Reset();
        int SaveChanges();

		Task<int> SaveChangesAsync();

		IDataStorageEngine Engine { get; }
		
    }
    public interface IDataContextExtension
    {
		void UpdateFields<T>(T item, Func<IFieldUpdater<T>, IFieldUpdater<T>> updater) where T : class;
		object GetEntityOriginalValue(object Entity, string Field);
        string GetEntitySetName<T>() where T : class;
    }
}
