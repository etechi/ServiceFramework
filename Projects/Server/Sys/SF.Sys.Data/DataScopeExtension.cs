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
using System.Threading.Tasks;
using SF.Sys.Threading;

namespace SF.Sys.Data
{

	public static class DataScopeExtension
	{
		static Task<bool> TaskCompletedResult { get; } = Task.FromResult(true);
		static Func<Task, Task<bool>> FuncTaskCompleted { get; } = task => TaskCompletedResult;

	
		public static Task Use(
			this IDataScope scope,
			string Action,
			Func<IDataContext, Task> Callback,
			DataContextFlag Flags = DataContextFlag.None,
			System.Data.IsolationLevel TransactionLevel = System.Data.IsolationLevel.Unspecified
			)
			=> scope.Use(
				Action,
				async ctx =>
				{
					await Callback(ctx);
					return 0;
				},
				Flags,
				TransactionLevel
				);
		public static Task<T> Retry<T>(
			this IDataScope scope,
			string Action,
			Func<IDataContext,Task<T>> Callback,
			DataContextFlag Flags = DataContextFlag.None,
			System.Data.IsolationLevel IsolationLevel=System.Data.IsolationLevel.Unspecified,
			int RetryInterval = 4000, 
			int RetryRepeat = 5
			)
		{
			return TaskUtils.Retry(ct =>
				scope.Use(Action, Callback, Flags, IsolationLevel)
				, RetryInterval, 
				RetryRepeat
				);
		}

		//public static Task<T> UseSet<TSet, T>(this IDataScope scope, string Action,Func<IDataSet<TSet>, Task<T>> Callback, DataContextFlag Flags=DataContextFlag.None) where TSet:class
		//	=> scope.Use(Action, ctx =>Callback(ctx.Set<TSet>()), Flags);

		//public static Task UseSet<TSet>(this IDataScope scope, string Action, Func<IDataSet<TSet>, Task> Callback,DataContextFlag Flags=DataContextFlag.None) where TSet : class
		//	=> scope.Use(Action, ctx => Callback(ctx.Set<TSet>()),Flags);

		//public class QueryContext
		//{

		//}
		//public static Task<T> Query<TSet, T>(this IDataScope scope, Func<IQueryable<TSet>, Task<T>> Callback, string Action, DataContextFlag Flags=DataContextFlag.None) where TSet : class
		//	=> scope.Use(Action??$"查询{typeof(TSet).FullName}", ctx =>
		//		  Callback(ctx.Set<TSet>().AsQueryable()),Flags);
	}
}
