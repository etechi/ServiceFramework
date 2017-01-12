using System.Linq;

namespace SF.Data.Entity.EntityFrameworkCore
{
	class DataSetReadonly<T> : DataSet<T>, IDataSetReadonly<T>
		where T:class
	{
		public DataSetReadonly(EntityDbContextProvider Context, IQueryable<T> query) : base(Context, query)
		{
		}
	}
	
}
