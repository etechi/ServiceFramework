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

using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using SF.Sys.TimeServices;
using System.Linq;
using SF.Sys.CallPlans.Runtime;
using SF.Sys.CallPlans;
using SF.Sys.Reminders.Models;
using SF.Sys.Reminders;
using SF.Sys.Threading;

namespace SF.Sys.Services
{
	class RemindSyncQueue
	{
		public ISyncQueue<long> Queue { get; set; }
	}
	public static class ReminderDIServiceCollectionExtension
	{
		
		public static IServiceCollection AddReminderServices(this IServiceCollection sc,string TablePrefix=null)
		{
			sc.AddDataModules<
				   SF.Sys.Reminders.DataModels.Reminder,
				   SF.Sys.Reminders.DataModels.DataRemindRecord
				   >(
				   TablePrefix ?? "Sys"
				   );

			sc.EntityServices(
				"Reminders",
				"提醒管理",
				d => d
					.Add<IReminderManager, ReminderManager>("Reminder", "提醒计划", typeof(Reminder))
					.Add<IRemindRecordManager, RemindRecordManager>("RemindRecord", "提醒记录", typeof(RemindRecord))
				);


			sc.InitServices("提醒管理", async (sp, sim, scope) =>
			{
				var MenuPath = "系统管理/提醒管理";
				await sim.Service<IReminderManager, ReminderManager>(null)
					.WithMenuItems(MenuPath)
					.Ensure(sp, scope);

				await sim.Service<IRemindRecordManager, RemindRecordManager>(null)
					.WithMenuItems(MenuPath)
					.Ensure(sp, scope);
			});


			var syncQueue = new RemindSyncQueue();
			sc.AddSingleton(sp => syncQueue);
			sc.AddAtLeastOnceEntityTaskService(
				new AtLeastOnceActionEntityServiceSetting<long, long, SF.Sys.Reminders.DataModels.Reminder>
				{
					Name="用户提醒服务",
					GetKey=e=>e.Id,
					KeyEqual=id=>e=>e.Id==id,
					GetSyncKey=id=>id,
					Init = (sp, sq) =>
					{
						syncQueue.Queue = sq;
						return Task.CompletedTask;
					},
					RunTask = async (sp, entity) =>
					{
						var re=	await ((ReminderManager)sp.Resolve<IReminderManager>()).RunTasks(entity);
						return re;
					}
				});

			return sc;
		}

	}
}
