﻿using SF.Core.ServiceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data.Storage
{

	public interface IFieldUpdater<T>
    {
        IFieldUpdater<T> Update<P>(Expression<Func<T, P>> field);
    }

	public interface IDataContext :IDisposable
	{
		IDataSet<T> Set<T>() where T : class;

		void Reset();
        int SaveChanges();

		Task<int> SaveChangesAsync();

		IDataContextProvider Provider { get; }
		ITransactionScopeManager TransactionScopeManager { get; }
	}
	public interface IDataContextExtension
    {
		void UpdateFields<T>(T item, Func<IFieldUpdater<T>, IFieldUpdater<T>> updater) where T : class;
		object GetEntityOriginalValue(object Entity, string Field);
        string GetEntitySetName<T>() where T : class;
    }
}
