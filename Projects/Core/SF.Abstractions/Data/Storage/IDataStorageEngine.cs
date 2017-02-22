﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Data.Storage
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
        Task<object> ExecuteCommandAsync(string Sql, CancellationToken CancellationToken,params object[] Args);
		IDataTransaction BeginTransaction();
		IDataTransaction BeginTransaction(IsolationLevel isolationLevel);
	}

}