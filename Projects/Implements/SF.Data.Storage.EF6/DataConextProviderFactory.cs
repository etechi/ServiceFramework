using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data.Entity;
using System.Data.Common;

namespace SF.Data.EF6
{
	public class DataContextProviderFactory<T> : 
		IDataContextProviderFactory
		where T: System.Data.Entity.DbContext
	{
		Func<T> DbContextCreator { get; }
		public DataContextProviderFactory(Func<T> DbContextCreator)
		{
			this.DbContextCreator = DbContextCreator;
		}
		public IDataContextProvider Create(IDataContext DataContext)
		{
			return new EntityDbContextProvider<T>(DbContextCreator(), DataContext);
		}
	}
	public class DataContextProviderFactory :
		IDataContextProviderFactory
	{
		IServiceProvider ServiceProvider { get; }
		DbConnection Connection { get; }
		public DataContextProviderFactory(IServiceProvider ServiceProvider, DbConnection Connection)
		{
			this.ServiceProvider = ServiceProvider;
			this.Connection = Connection;
		}
		public IDataContextProvider Create(IDataContext DataContext)
		{
			return new EntityDbContextProvider(
				new DbContext(
					ServiceProvider,
					Connection
				), 
				DataContext);
		}
	}
}
