
using SF.Sys.Entities;
using SF.Sys.Reminders.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Sys.Data;
namespace SF.Sys.Reminders
{
	public class ReminderManager :
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
		public ReminderManager(
			IEntityServiceContext ServiceContext,
			IEnumerable<IReminderActionSource> ActionSources
			) : base(ServiceContext)
		{
			this.ActionSources = ActionSources;
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

		public async Task<bool> Refresh(long Id)
		{
			var reminder = await DataSet.FindAsync(Id);
			var time = reminder?.PlanTime ?? Now;
			var actions = await GetNextActions(Id, time);
			if (actions.Length == 0)
			{
				if (reminder == null)
					return false;
				else
				{
					reminder.TaskState = AtLeastOnceTasks.Models.AtLeastOnceTaskState.Completed;
					reminder.PlanTime = time;
					reminder.Name = "没有动作";
					DataSet.Update(reminder);
				}
			}
			else
			{
				if (reminder == null)
					reminder = DataSet.Add(new DataModels.Reminder
					{
						CreatedTime = Now,
						LogicState = EntityLogicState.Enabled,
					});
				else
					DataSet.Update(reminder);
				var firstAction = actions[0];

				var info = await firstAction.GetInfo();
				reminder.Name = (info.Name + (actions.Length > 1 ? "等" + actions.Length + "项" : "")).Limit(100);
				reminder.TaskState = SF.Sys.AtLeastOnceTasks.Models.AtLeastOnceTaskState.Waiting;
				reminder.PlanTime = time;
				reminder.UpdatedTime = time;
				reminder.TaskNextRunTime = actions[0].Time;
			}
			await DataSet.Context.SaveChangesAsync();
			return true;
		}

		public async Task<DateTime?> RunTask(DataModels.Reminder Model)
		{
			var actions = await GetNextActions(Model.Id, Model.PlanTime);
			if (actions.Length > 0)
			{
				var firstAction = actions[0];
				var time = firstAction.Time;
				var recSet = DataContext.Set<DataModels.RemindRecord>();
				var recs = await recSet
					.AsQueryable()
					.Where(r => r.ReminderId == Model.Id && r.Time == time)
					.ToDictionaryAsync(r=>r.BizIdent);

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
						rec = recSet.Add(new DataModels.RemindRecord
						{
							Id = await IdentGenerator.GenerateAsync<DataModels.RemindRecord>(),

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
						await action.Execute(rec.Id,retry);
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

			Model.PlanTime = Now;
			actions = await GetNextActions(Model.Id, Model.PlanTime);

			if (actions.Length == 0)
			{
				Model.Name = "没有动作";
				return null;
			}
			else
			{
				var firstAction = actions[0];
				var info = await firstAction.GetInfo();
				Model.Name = (info.Name + (actions.Length > 1 ? "等" + actions.Length + "项" : "")).Limit(100);
				return firstAction.Time;
			}
		}
		
	}

}
