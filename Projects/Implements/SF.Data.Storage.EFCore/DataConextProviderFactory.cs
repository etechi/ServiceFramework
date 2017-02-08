using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace SF.Data.Storage.EntityFrameworkCore
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
