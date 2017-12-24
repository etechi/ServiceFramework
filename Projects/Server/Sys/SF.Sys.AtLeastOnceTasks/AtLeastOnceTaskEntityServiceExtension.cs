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

using SF.Sys;
using SF.Sys.AtLeastOnceTasks.Models;
using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys.Entities.DataModels;
using SF.Sys.Services;
using SF.Sys.Services.Management;
using SF.Sys.Services.Management.Models;
using SF.Sys.TimeServices;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Sys.Services
{
	public class AtLeastOnceActionEntityServiceSetting<TKey, TEntity> {
		public string Name { get; set; }
		public int BatchCount { get; set; } = 100;
		public int ThreadCount { get; set; } = 100;
		public int Interval { get; set; } = 5000;
		public int ErrorDelayUnit { get; set; } = 10;
		public int ExecTimeoutSeconds { get; set; } = 0;
		public Func<IServiceProvider,TEntity,Task<DateTime?> > RunTask { get; set; }
	}

	public static class AtLeastOnceTaskEntityService
	{
		public static IServiceCollection AddAtLeastOnceEntityTaskService<TKey, TEntity>(
			this IServiceCollection sc,
			AtLeastOnceActionEntityServiceSetting<TKey, TEntity> Setting
			)
			where TKey : IEquatable<TKey>
			where TEntity : class, IAtLeastOnceTask<TKey>
		{
			sc.AddAtLeastOnceTaskService(
				new AtLeastOnceActionServiceSetting<TKey, TEntity>
				{
					Name = Setting.Name,
					BatchCount = Setting.BatchCount,
					ThreadCount = Setting.ThreadCount,
					Interval = Setting.Interval,
					ErrorDelayUnit = Setting.ErrorDelayUnit,
					ExecTimeoutSeconds = Setting.ExecTimeoutSeconds,
					GetIdentsToRunning = async (isp, count, time) =>
					  {
						  var set = isp.Resolve<IDataSet<TEntity>>();
						  var tasks = await set.AsQueryable()
							  .Where(e => e.TaskState == AtLeastOnceTaskState.Waiting && e.TaskNextRunTime <= time)
							  .OrderBy(e=>e.TaskNextRunTime)
							  .Take(count)
							  .ToArrayAsync();
						  foreach (var t in tasks)
						  {
							  t.TaskState = AtLeastOnceTaskState.Running;
							  set.Update(t);
						  }
						  await set.Context.SaveChangesAsync();
						  var ids = tasks.Select(t => t.Id).ToArray();
						  return ids;
					  },
					LoadRunningTasks = async (isp) =>
					  {
						  var set = isp.Resolve<IDataSet<TEntity>>();
						  var re = await set.AsQueryable()
							  .Where(e => e.TaskState == AtLeastOnceTaskState.Running)
							  .ToArrayAsync();
						  return re;
					  },
					LoadTask = async (isp, id) =>
					{
						var set = isp.Resolve<IDataSet<TEntity>>();
						var re = await set.AsQueryable()
							.Where(e => e.TaskState == AtLeastOnceTaskState.Running && e.Id.Equals(id))
							.SingleOrDefaultAsync();
						return re;
					},
					RunTask = Setting.RunTask,
					SaveTask = async (isp, task) =>
					{
						var set = isp.Resolve<IDataSet<TEntity>>();
						set.Update(task);
						await set.Context.SaveChangesAsync();
					},
					UseDataScope = async (isp, action) =>
					 {
						 var tsm = isp.Resolve<ITransactionScopeManager>();
						 await tsm.UseTransaction(
							 Setting.Name,
							 ts => action(),
							 TransactionScopeMode.RequireTransaction
							 );
					 }
				}
				);
			return sc;
		}
	}
}
