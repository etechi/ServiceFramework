using SF.Core.ServiceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Data.Storage
{
	[UnmanagedService]
	public interface IDataSetProvider<T>
		where T : class
	{
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
