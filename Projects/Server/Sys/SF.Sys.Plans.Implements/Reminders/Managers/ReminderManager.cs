
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
			DataModels.DataReminder
			>,
		IReminderManager
	{
		public ReminderManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
		{
		}

		public Task ClearAllReminders()
		{
			return DataScope.Use("删除所有提醒", async ctx =>
			{
				ctx.RemoveRange(
					await ctx.Set<DataModels.DataReminder>().AsQueryable(false).ToArrayAsync()
					);
				await ctx.SaveChangesAsync();
			});
		}
	}
}
