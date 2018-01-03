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


using SF.Common.Notifications;
using SF.Common.Notifications.Senders.EMail;
using SF.Common.Notifications.Management;
using SF.Common.Notifications.Senders;
using SF.Common.Notifications.Front;
using SF.Sys.Entities;
using SF.Sys.Events;
using System.Threading.Tasks;

namespace SF.Sys.Services
{
	public static class NotificationServicesDIExtension
	{
		public static IServiceCollection AddNotificationServices(
			this IServiceCollection sc,
			string TablePrefix=null
			)
		{
			sc.AddDataModules<
				SF.Common.Notifications.DataModels.NotificationSendRecord,
				SF.Common.Notifications.DataModels.NotificationSendPolicy,
				SF.Common.Notifications.DataModels.Notification,
				SF.Common.Notifications.DataModels.NotificationTarget,
				SF.Common.Notifications.DataModels.NotificationUserStatus
				>(
				TablePrefix??"Common"
				);
			sc.EntityServices(
				"Notifications",
				"通知",
				d => 
				d.Add<INotificationManager, NotificationManager>("Notification", "通知")
				.Add<INotificationSendRecordManager, NotificationSendRecordManager>("NotificationSendRecord", "通知发送记录")
				.Add<INotificationSendPolicyManager, NotificationSendPolicyManager>("NotificationSendPolicy", "通知发送策略")
				);

			sc.AddManagedTransient<INotificationSendProvider, SystemEMailProvider>();
			sc.AddManagedScoped<INotificationSendProvider, DebugNotificationSendProvider>();

			sc.AddManagedScoped<INotificationService, NotificationService>();
			sc.AddTransient(sp => (IDebugNotificationSendProvider)sp.Resolve<INotificationSendProvider>("debug"));

			sc.AddEntityGlobalCache(
				async (INotificationSendPolicyManager nspm, long Id) =>
				{
					var re = await nspm.GetAsync(ObjectKey.From(Id));
					return re;
				},
				(IEventSubscriber<EntityChanged<SF.Common.Notifications.DataModels.NotificationSendPolicy>> OnPolicyModified, IEntityCacheRemover<long> remover) =>
				{
					OnPolicyModified.Wait(e =>
					{
						remover.Remove(e.Id);
						return Task.CompletedTask;
					});
				}
			);
			sc.InitServices("通知服务",async (sp, sim, scope) =>
			 {
				 await sim.DefaultService<INotificationService, NotificationService>(
					 new NotificationServiceSetting { }
					 ).Ensure(sp, scope);

				 await sim.DefaultService<INotificationSendPolicyManager, NotificationSendPolicyManager>(null)
					.Ensure(sp, scope);

				 await sim.DefaultService<INotificationManager, NotificationManager>(null)
					.Ensure(sp, scope);
				 await sim.DefaultService<INotificationSendRecordManager, NotificationSendRecordManager>(null).Ensure(sp, scope);

				 await sim.Service<INotificationSendProvider, DebugNotificationSendProvider>(null)
					.WithIdent("debug")
					.Ensure(sp, scope);

				 await sim.Service<INotificationSendProvider, SystemEMailProvider>(null)
					.WithIdent("email")
					.Ensure(sp, scope);
			 });
			
			return sc;
		}
		//public static IServiceCollection AddSimPhoneTextMessageService(this IServiceCollection sc)
		//{
		//	sc.AddManagedScoped<IPhoneMessageService, SimPhoneTextMessageService>();
		//	return sc;
		//}
		
	}
}