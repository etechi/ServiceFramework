using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Data.Entity
{
	public interface IDataSet<T> :
		IContextQueryable<T>
		where T : class
	{
		Task<T> FindAsync(object Ident);
		Task<T> FindAsync(params object[] Idents);
	}
	public interface IDataSetReadonly<T> : IDataSet<T>
		where T : class
	{

	}
	public interface IDataSetEditable<T> : IDataSet<T>
		where T : class
	{
		T Add(T Model);
		void AddRange(IEnumerable<T> Items);

		T Remove(T Model);
		void RemoveRange(IEnumerable<T> Items);

		T Update(T item);
	}

}
