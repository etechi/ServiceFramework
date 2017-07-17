using SF.Core.ServiceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data.Storage
{


	[UnmanagedService]
	public interface IDataContextProvider :IDisposable, IQueryableContext,IAsyncQueryableContext
	{
		IDataSetProvider<T> SetProvider<T>() where T : class;

		IEntityQueryableProvider EntityQueryableProvider { get; }
    
        int SaveChanges();

		Task<int> SaveChangesAsync();

		IDataStorageEngine Engine { get; }
    }
	[UnmanagedService]
	public interface IDataContextProviderExtension
	{
		void UpdateFields<T>(T item, Func<IFieldUpdater<T>, IFieldUpdater<T>> updater) where T : class;
		object GetEntityOriginalValue(object Entity, string Field);
        string GetEntitySetName<T>() where T : class;
    }
}
