using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace SF.Data.Entity.EntityFrameworkCore
{
	class DataSet<T> :  IDataSet<T>
		where T:class
	{
		public DataSet(EntityDbContextProvider Provider, DbSet<T> Set)
		{
			this.Set = Set;
			this.Provider = Provider;
		}

		DbSet<T> Set { get; }

		EntityDbContextProvider Provider { get; }

		public IDataContext Context => Provider.DataContext;

		public T Add(T Model)
			=> Set.Add(Model).Entity;

		public void AddRange(IEnumerable<T> Items)
			=> Set.AddRange(Items);


		public T Remove(T Model)
			=> Set.Remove(Model).Entity;

		public void RemoveRange(IEnumerable<T> Items)
			=> Set.RemoveRange(Items);

		public T Update(T item)
			=> Set.Update(item).Entity;

		public IContextQueryable<T> AsQueryable(bool ReadOnly)
		{

			IQueryable<T> query = Set;
			if (ReadOnly)
				query = query.AsNoTracking();
			return new DbQueryable<T>(Provider, query);
		}

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
