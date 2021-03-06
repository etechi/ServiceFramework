﻿#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.Data
{
	[Flags]
	public enum TransactionCommitNotifyType
	{
		None = 0,
		BeforeCommit = 1,
		AfterCommit = 2,
		Rollback = 4
	}
	public interface ITransactionCommitTracker
	{
		TransactionCommitNotifyType TrackNotifyTypes { get; }
		Task Notify(TransactionCommitNotifyType Type, Exception Exception);
	}
	public interface IFieldUpdater<T>
    {
        IFieldUpdater<T> Update<P>(Expression<Func<T, P>> field);
    }

	[Flags]
	public enum DataContextFlag
	{
		None=0,
        //需要新事务
        RequireNewTransaction=2
	}
	public interface IDataScope
	{
		Task<T> Use<T>(
			string Actin,
			Func<IDataContext, Task<T>> Callback, 
			DataContextFlag Flags=DataContextFlag.None,
			System.Data.IsolationLevel TransactionLevel=System.Data.IsolationLevel.Unspecified
			);
	}
	

	public interface IDataContext :IDisposable
	{
		IDataSet<T> Set<T>() where T : class;
		void AddCommitTracker(ITransactionCommitTracker Tracker);
		
		//int SaveChanges();
		void ClearTrackingEntities();
		Task SaveChangesAsync();
	}
	public interface IDataContextExtension
    {
		void UpdateFields<T>(T item, Func<IFieldUpdater<T>, IFieldUpdater<T>> updater) where T : class;
		object GetEntityOriginalValue(object Entity, string Field);
        string GetEntitySetName<T>() where T : class;
		IEnumerable<string> GetUnderlingCommandTexts<T>(IQueryable<T> Queryable) where T: class;
		IDataContextTransaction Transaction { get; }
		DbConnection GetDbConnection();
    }
	public interface IDbDataContextState
	{
		string State { get; set; }
	}
    
}
