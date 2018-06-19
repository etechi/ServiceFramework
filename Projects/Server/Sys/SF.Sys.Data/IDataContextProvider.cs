#region Apache License Version 2.0
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Sys.Data
{

	public interface IDataContextTransaction : IDisposable
	{
		void Commit();
		void Rollback();
		object RawTransaction { get; }
	}
	public interface IDataContextProvider :IDisposable, IQueryableContext,IAsyncQueryableContext
	{
		IEntityQueryableProvider EntityQueryableProvider { get; }
		IDataSet<T> CreateDataSet<T>(IDataContext DataContext) where T:class;
		void ClearTrackingEntities();
		Task<int> SaveChangesAsync();

	}
	public interface IDataContextProviderExtension
	{
		void UpdateFields<T>(T item, Func<IFieldUpdater<T>, IFieldUpdater<T>> updater) where T : class;
		object GetEntityOriginalValue(object Entity, string Field);
        string GetEntitySetName<T>() where T : class;
		DbConnection GetDbConnection();
		IEnumerable<string> GetUnderlingCommandTexts<T>(IContextQueryable<T> Queryable) where T : class;
		Task<IDataContextTransaction> BeginTransaction(
			System.Data.IsolationLevel IsolationLevel,
			CancellationToken cancellationToken
			);
	}
}
