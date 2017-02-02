using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SF.Data.Entity.EF6
{
	class DbQueryable<T> : IContextQueryable<T>
	{
		public IQueryable<T> Queryable { get; }
		public EntityDbContextProvider Context { get; }

		IQueryable IContextQueryable.Queryable
		{
			get
			{
				return Queryable;
			}
		}

		IQueryableContext IContextQueryable.Context
		{
			get
			{
				return Context;
			}
		}

		public DbQueryable(EntityDbContextProvider Context,IQueryable<T> query)
		{
			this.Context = Context;
			this.Queryable = query;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return Queryable.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Queryable.GetEnumerator();
		}
	}
	
}
