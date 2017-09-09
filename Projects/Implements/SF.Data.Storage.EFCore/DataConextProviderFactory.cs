using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Data.Common;

namespace SF.Data.EntityFrameworkCore
{
	public class DataContextProviderFactory<T> : 
		IDataContextProviderFactory
		where T: Microsoft.EntityFrameworkCore.DbContext
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
		Func<Microsoft.EntityFrameworkCore.DbContext> DbContextCreator { get; }
		public DataContextProviderFactory(Func<DbContext> DbContextCreator)
		{
			this.DbContextCreator = DbContextCreator;
		}
		public IDataContextProvider Create(IDataContext DataContext)
		{
			return new EntityDbContextProvider(DbContextCreator(), DataContext);
		}
	}
}
