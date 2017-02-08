using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Data.Storage
{
	public interface IDataSet<T>
		where T : class
	{
		IDataContext Context { get; }
		Task<T> FindAsync(object Ident);
		Task<T> FindAsync(params object[] Idents);

		T Add(T Model);
		void AddRange(IEnumerable<T> Items);

		T Remove(T Model);
		void RemoveRange(IEnumerable<T> Items);

		T Update(T Model);

		IContextQueryable<T> AsQueryable(bool ReadOnly=true);

		IDataSetMetadata Metadata { get; }
	}
}
