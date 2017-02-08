using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data.Entity;

namespace SF.Data.Storage.EF6
{
	public class DataContextProviderFactory<T> : 
		IDataContextProviderFactory
		where T:DbContext
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

}
