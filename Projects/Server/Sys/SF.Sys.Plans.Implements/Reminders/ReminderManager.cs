
using SF.Sys.Entities;
using SF.Sys.Reminders.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Sys.Data;
using SF.Sys.Services;
namespace SF.Sys.Reminders
{
	class ReminderManager :
		AutoModifiableEntityManager<
			ObjectKey<long>,
			Reminder,
			Reminder,
			ReminderQueryArgument,
			Reminder,
			DataModels.Reminder
			>,
		IReminderManager
	{
		IEnumerable<IReminderActionSource> ActionSources { get; }
		Lazy<RemindSyncQueue> RemindSyncQueue { get; }
		public ReminderManager(
			IEntityServiceContext ServiceContext,
			IEnumerable<IReminderActionSource> ActionSources,
			Lazy<RemindSyncQueue> RemindSyncQueue
			) : base(ServiceContext)
		{
			this.ActionSources = ActionSources;
			this.RemindSyncQueue = RemindSyncQueue;
		}

		async Task<IRemindAction[]> GetNextActions(long Id,DateTime Time)
		{
			var list = new List<IRemindAction>();
			foreach(var src in ActionSources)
			{
				list.AddRange(await src.GetActions(Id, Time));
			}

			var q=from act in list
			group act by act.Time into g
			orderby g.Key
			select g.ToArray();
			var actions = q.FirstOrDefault();
			return actions;
		}

		public Task RefreshAt(long Id,DateTime Time,string DefaultName)
		{
			return RemindSyncQueue.Value.Queue(Id, () =>
			   DataScope.Use("刷新提醒", async ctx =>
			   {
				   var DataSet = ctx.Set<DataModels.Reminder>();
				   var reminder = await DataSet.FindAsync(Id);
				   if (reminder == null)
					   reminder = DataSet.Add(new DataModels.Reminder
					   {
						   Id = Id,
						   CreatedTime = Now,
						   LogicState = EntityLogicState.Enabled,
						   Name = DefaultName
					   });
				   else
					   DataSet.Update(reminder);

				   reminder.PlanTime = Now;
				   reminder.TaskNextTryTime = Time;
				   reminder.TaskState = SF.Sys.AtLeastOnceTasks.AtLeastOnceTaskState.Waiting;
				   await DataSet.Context.SaveChangesAsync();
				   return 0;
			   }
		   ));
		}

		async Task ExecTasks(IDataContext ctx,DataModels.Reminder Model,IRemindAction[] actions)
		{
			var recSet = ctx.Set<DataModels.DataRemindRecord>();
			var recs = await recSet
				.AsQueryable()
				.Where(r => r.ReminderId == Model.Id && r.Time == Now)
				.ToDictionaryAsync(r => r.BizIdent);

			Exception error = null;
			foreach (var action in actions)
			{
				var retry = false;
				if (recs.TryGetValue(action.BizIdent, out var rec))
				{
					recSet.Update(rec);
					retry = true;
				}
				else
					rec = recSet.Add(new DataModels.DataRemindRecord
					{
						Id = await IdentGenerator.GenerateAsync<DataModels.DataRemindRecord>(),

						ReminderId = Model.Id,

					});

				var info = await action.GetInfo();

				rec.Action = info.Action.Limit(100);
				rec.BizIdent = action.BizIdent;
				rec.Description = info.Description.Limit(200);
				rec.Name = info.Name.Limit(100);
				rec.ActionTime = action.Time;
				rec.Time = Now;

				try
				{
					await action.Execute(rec.Id, retry);
					rec.Error = null;
				}
				catch (Exception e)
				{
					rec.Error = e.Message.Limit(200);
					if (error == null)
						error = e;
				}
			}
			if (error != null)
				throw error;
		}

		public async Task<DateTime?> RunTasks(DataModels.Reminder Model)
		{
			//使用计划时间，载入动作
			var actions = await GetNextActions(Model.Id, Model.PlanTime);
			//找不到动作
			if ((actions?.Length ??0) == 0)
			{
				Model.Name = "没有动作";
				return null;
			}

			//假如动作时间已经到达
			if (actions[0].Time < Now)
			{
				//如果有动作存在，则执行动作
				if (actions.Length > 0)
					await DataScope.Use("执行提醒", ctx =>
						 ExecTasks(ctx, Model, actions)
						 );
			
				//正常执行完成，尝试查找下一批动作
				Model.PlanTime = Now;
				actions = await GetNextActions(Model.Id, Model.PlanTime);
				//找不到动作
				if (actions.Length == 0)
				{
					Model.Name = "没有动作";
					return null;
				}
			}
			

			var firstAction = actions[0];
			var info2= await firstAction.GetInfo();
			Model.Name = (info2.Name + (actions.Length > 1 ? "等" + actions.Length + "项" : "")).Limit(100);
			return firstAction.Time;
		}
	}
}
