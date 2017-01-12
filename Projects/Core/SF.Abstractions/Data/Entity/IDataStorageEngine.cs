using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data.Entity
{
	public enum IsolationLevel
	{

	}
	public interface IDataTransaction:IDisposable
	{
		object UnderlyingTransaction { get; }
		void Commit();
		void Rollback();
	}
	public interface IDataStorageEngine
	{
        Task<object> ExecuteCommandAsync(string Sql, params object[] Args);
		IDataTransaction BeginTransaction();
		IDataTransaction BeginTransaction(IsolationLevel isolationLevel);
	}

}
