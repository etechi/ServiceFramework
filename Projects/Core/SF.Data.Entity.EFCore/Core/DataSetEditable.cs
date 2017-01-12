using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;

namespace SF.Data.Entity.EntityFrameworkCore
{
	class DataSetEditable<T> : DataSet<T>,IDataSetEditable<T>
		where T : class
	{
		public DataSetEditable(EntityDbContextProvider Context, DbSet<T> set):base(Context,set)
		{
			
		}
		public T Add(T Model)
			=> Set.Add(Model).Entity;

		public void AddRange(IEnumerable<T> Items)
			=> Set.AddRange(Items);


		public T Remove(T Model)
			=> Set.Remove(Model).Entity;

		public void RemoveRange(IEnumerable<T> Items)
			=> Set.RemoveRange(Items);

		public T Update(T item)
			=>Set.Update(item).Entity;
	}
	
}
