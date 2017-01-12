using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Data.Entity.EntityFrameworkCore
{
	class DataSet<T> : DbQueryable<T>, IDataSet<T>
		where T:class
	{
		public DataSet(EntityDbContextProvider Context, IQueryable<T> query) : base(Context, query)
		{
		}

		public DbSet<T> Set { get { return (DbSet<T>)Queryable; } }

		public Task<T> FindAsync(params object[] Idents)
		{
			return Set.FindAsync(Idents);
		}

		public Task<T> FindAsync(object Ident)
		{
			return Set.FindAsync(Ident);
		}
	}
	
}
