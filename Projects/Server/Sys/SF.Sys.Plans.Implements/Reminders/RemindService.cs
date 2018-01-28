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

using SF.Sys.Annotations;
using SF.Sys.AtLeastOnceTasks.DataModels;
using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys.Entities.DataModels;
using SF.Sys.Services;
using SF.Sys.TimeServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Sys.Reminders
{
	public class RemindService : IRemindService
	{
		IDataScope DataScope { get; }
		Lazy<RemindableManager> RemindableManager { get; }
		Lazy<IIdentGenerator> IdentGenerator { get; }
		Lazy<ITimeService> TimeService { get; }
		Lazy<IAtLeastOnceTaskExecutor<long,DataModels.DataReminder, long>> AtLeastOnceTaskExecutor { get; }
		Lazy<RemindSyncQueue> RemindSyncQueue { get; }
		public RemindService(
			IDataScope DataScope,
			Lazy<RemindableManager> RemindableManager,
			Lazy<IIdentGenerator> IdentGenerator,
			Lazy<ITimeService> TimeService,
			Lazy<IAtLeastOnceTaskExecutor<long, DataModels.DataReminder, long>> AtLeastOnceTaskExecutor,
			Lazy<RemindSyncQueue> RemindSyncQueue
			)
		{
			this.AtLeastOnceTaskExecutor = AtLeastOnceTaskExecutor;
			this.DataScope = DataScope;
			this.RemindableManager = RemindableManager;
			this.IdentGenerator = IdentGenerator;
			this.TimeService = TimeService;
			this.RemindSyncQueue = RemindSyncQueue;
		}

		public Task<bool> Remove(long Id)
		{
			return DataScope.Use("删除提醒", async ctx =>
			{
				var r = await ctx.Set<DataModels.DataReminder>().FindAsync(Id);
				if (r == null)
					return false;
				ctx.Remove(r);
				ctx.AddCommitTracker(
					TransactionCommitNotifyType.AfterCommit,
					(t, e) =>
					{
						if (e != null) return;
						AtLeastOnceTaskExecutor.Value.RemoveTimedTaskExecutor(Id);
					});
				await ctx.SaveChangesAsync();
				return true;
			});
		}

		public Task<bool> Remove(string BizType, string BizIdentType, long BizIdent)
		{
			return DataScope.Use("删除提醒", async ctx =>
			{
				var remind = await ctx
					.Queryable<DataModels.DataReminder>(false)
					.Where(r =>r.BizType==BizType && r.BizIdentType == BizIdentType && r.BizIdent == BizIdent)
					.SingleOrDefaultAsync();
				if (remind == null)
					return false;
				ctx.Remove(remind);
				ctx.AddCommitTracker(
					TransactionCommitNotifyType.AfterCommit,
					(t, e) =>
					{
						if (e != null) return;
						AtLeastOnceTaskExecutor.Value.RemoveTimedTaskExecutor(remind.Id);
					});
				await ctx.SaveChangesAsync();
				return true;
			});

		}
		public Task<long> Setup(RemindSetupArgument Argument)
		{
			//Ensure.Validate(Argument, "提醒参数");
			//if(Argument==null)
			RemindableManager.Value.GetDefination(Argument.RemindableName);
			return DataScope.Use("设置提醒", async ctx =>
			{
				var remind = await ctx
					.Queryable<DataModels.DataReminder>(false)
					.Where(r => r.BizIdentType == Argument.BizType && r.BizIdent == Argument.BizIdent)
					.SingleOrDefaultAsync();

				var now = TimeService.Value.Now;
				if (remind == null)
				{
					remind = new DataModels.DataReminder
					{
						Id = await IdentGenerator.Value.GenerateAsync<DataModels.DataReminder>(),
						BizIdent = Argument.BizIdent,
						BizIdentType = Argument.BizType,
						CreatedTime = now,
						OwnerId = Argument.UserId,
					};
					ctx.Add(remind);
				}
				else
					ctx.Update(remind);

				remind.Data = Argument.RemindData;
				remind.TaskNextExecTime = Argument.RemindTime;
				remind.RemindableName = Argument.RemindableName;
				remind.UpdatorId = Argument.UserId;
				remind.UpdatedTime = now;
				remind.TaskStartTime = now;
				remind.TaskMessage = null;
				remind.TaskLastExecTime = null;
				remind.Name = Argument.Name;
				remind.TaskState = AtLeastOnceTasks.AtLeastOnceTaskState.Waiting;
				remind.TaskExecCount = 0;

				if (Argument.RemindTime.Subtract(now).TotalMinutes < 30)
					ctx.AddCommitTracker(
						TransactionCommitNotifyType.AfterCommit,
						(t, e) =>
						{
							if (e != null) return;
							AtLeastOnceTaskExecutor.Value.UpdateTimedTaskExecutor(
								remind.Id, 
								remind.Id, 
								remind.TaskLastExecTime.Value
								);
						});
				await ctx.SaveChangesAsync();
				return remind.Id;
			},
			DataContextFlag.LightMode
			);
		}
		class RemindContext: IRemindContext
		{
			public long? ServiceScopeId { get; set; }
			public string BizType { get; set; }
			public string BizIdentType { get; set; }
			public long BizIdent { get; set; }
			public string Data { get; set; }
			public object Argument { get; set; }
			public string RemindableName { get; set; }
			public DateTime? NextRemindTime { get; set; }
			public DateTime Time { get; set; }
		}

		public async Task Remind(IServiceProvider sp,DataModels.DataReminder entity,object ActiveArgument)
		{
			var def = RemindableManager.Value.GetDefination(entity.RemindableName);
			try
			{
				var ctx = new RemindContext
				{
					Data = entity.Data,
					Argument = ActiveArgument,
					BizIdent = entity.BizIdent,
					BizIdentType = entity.BizIdentType,
					RemindableName = entity.RemindableName,
					Time=TimeService.Value.Now,
					BizType=entity.BizType
				};
				var remindable = def.CreateRemindable(sp,null);
				await remindable.Remind(ctx);
				entity.Data = ctx.Data;
				if (ctx.NextRemindTime.HasValue)
				{
					entity.TaskNextExecTime = ctx.NextRemindTime.Value;
					entity.TaskState = AtLeastOnceTasks.AtLeastOnceTaskState.Waiting;
				}
				else
				{
					entity.TaskState = AtLeastOnceTasks.AtLeastOnceTaskState.Completed;
					try
					{
						await DataScope.Use("提醒任务完成",async dbctx =>
						{
							var rec = new DataModels.DataRemindRecord();
							Poco.Update<DataModels.DataRemindRecord>(rec, entity);
							rec.Id = await IdentGenerator.Value.GenerateAsync<DataModels.DataRemindRecord>();
							dbctx.Add(rec);
							await dbctx.SaveChangesAsync();
						}
						);
					}catch
					{

					}
				}
			}
			catch(Exception ex)
			{
				entity.TaskMessage = ex.ToString();
				if (entity.TaskExecCount >= def.RetryMaxCount)
				{
					entity.TaskState = AtLeastOnceTasks.AtLeastOnceTaskState.Failed;
				}
				else
				{
					entity.TaskState = AtLeastOnceTasks.AtLeastOnceTaskState.Waiting;
					entity.TaskNextExecTime = entity.TaskLastExecTime.Value.AddSeconds(
						Math.Min(
							def.RetryDelayMax,
							def.RetryDelayStart + def.RetryDelayStep * (1 << entity.TaskExecCount)
							)
						);
				}
			}
		}

		public async Task Remind(string BizType, string BizIdentType, long BizIdent, object Argument)
		{
			var rid = await DataScope.Use(
				"查找提醒",ctx =>ctx
				.Queryable<DataModels.DataReminder>(false)
				.Where(r => r.BizType == BizType && r.BizIdentType== BizIdentType && r.BizIdent == BizIdent)
				.Select(r => r.Id)
				.SingleOrDefaultAsync()
				);
			if (rid == 0)
				throw new ArgumentException($"找不到提醒:{BizType} {BizIdentType} {BizIdent}");
			await AtLeastOnceTaskExecutor.Value.Execute(
				rid, 
				rid,//SF.Sys.Services.RemindSyncQueue.BuildKey(BizType,BizIdentType,BizIdent), 
				Argument
				);
		}
		public Task Remind(long Id, object Argument)
		{
			return AtLeastOnceTaskExecutor.Value.Execute(
				Id, 
				Id,//SF.Sys.Services.RemindSyncQueue.BuildKey(sk.BizType, sk.BizIdentType, sk.BizIdent),  
				Argument
				);
		}
		public async Task<T> Sync<T>(string BizType, string BizIdentType, long BizIdent, Func<Task<T>> Callback)
		{
			var rid = await DataScope.Use(
				"查找提醒", ctx => ctx
				 .Queryable<DataModels.DataReminder>(false)
				 .Where(r => r.BizType == BizType && r.BizIdentType == BizIdentType && r.BizIdent == BizIdent)
				 .Select(r => r.Id)
				 .SingleOrDefaultAsync()
				);
			if (rid == 0)
				throw new ArgumentException($"找不到提醒:{BizType} {BizIdentType} {BizIdent}");

			return await RemindSyncQueue.Value.Queue(
				rid,//SF.Sys.Services.RemindSyncQueue.BuildKey(BizType, BizIdentType, BizIdent),
				Callback
				);
		}

		
	}
}
