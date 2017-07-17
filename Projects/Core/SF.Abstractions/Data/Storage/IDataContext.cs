using SF.Core.ServiceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data.Storage
{

	[UnmanagedService]
	public interface IFieldUpdater<T>
    {
        IFieldUpdater<T> Update<P>(Expression<Func<T, P>> field);
    }

	[UnmanagedService]
	public interface IDataContext :IDisposable
	{
		IDataSet<T> Set<T>() where T : class;

		void Reset();
        int SaveChanges();

		Task<int> SaveChangesAsync();

		IDataStorageEngine Engine { get; }
		
		IDataContextProvider Provider { get; }

	}
	[UnmanagedService]
	public interface IDataContextExtension
    {
		void UpdateFields<T>(T item, Func<IFieldUpdater<T>, IFieldUpdater<T>> updater) where T : class;
		object GetEntityOriginalValue(object Entity, string Field);
        string GetEntitySetName<T>() where T : class;
    }
}
