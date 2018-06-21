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
using SF.Common.Notifications.Models;
using System;
using SF.Sys.BackEndConsole;

namespace SF.Sys.Services
{
	public static class NotificationServicesDIExtension
	{
		public static IServiceCollection AddNotificationServices(
			this IServiceCollection sc,
			string TablePrefix=null
			)
		{
			sc.AddRemindable<SF.Common.Notifications.Senders.Remindable>();

			sc.AddDataModules<
				SF.Common.Notifications.DataModels.DataNotificationSendRecord,
				SF.Common.Notifications.DataModels.DataNotificationSendPolicy,
				SF.Common.Notifications.DataModels.DataNotification,
				SF.Common.Notifications.DataModels.DataNotificationTarget,
				SF.Common.Notifications.DataModels.DataNotificationUserStatus
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
			sc.AddManagedTransient<INotificationSendProvider, DebugNotificationSendProvider>();

			sc.AddManagedScoped<INotificationService, NotificationService>();
			sc.AddTransient(sp => (IDebugNotificationSendProvider)sp.Resolve<INotificationSendProvider>("debug"));

			sc.AddEntityGlobalCache(
				async (INotificationSendPolicyManager nspm, long Id) =>
				{
					var re = await nspm.GetAsync(ObjectKey.From(Id));
					return re;
				},
				(
					IServiceProvider isp,
					IEventSubscriber<EntityChanged<ObjectKey<long>, NotificationSendPolicy>> OnPolicyModified, 
					IEntityCacheRemover<long> remover
					) =>
				{
					OnPolicyModified.Wait(e =>
					{
						remover.Remove(e.Payload.EntityId.Id);
						return Task.CompletedTask;
					});
				}
			);
			sc.InitServices("通知服务",async (sp, sim, scope) =>
			{
				var MenuPath = "用户内容/通知服务";

				await sim.DefaultService<INotificationService, NotificationService>(
					 new NotificationServiceSetting { }
					 ).Ensure(sp, scope);

				 await sim.DefaultService<INotificationManager, NotificationManager>(null)
					.WithConsolePages(MenuPath)
					.WithEntityQuery("Notification", "全体未开始通知", new SF.Common.Notifications.Management.NotificationQueryArgument { State=NotificationState.NotStart,Mode=NotificationMode.Boardcast})
					.WithEntityQuery("Notification", "全体发布中通知", new SF.Common.Notifications.Management.NotificationQueryArgument { State = NotificationState.Available, Mode = NotificationMode.Boardcast })
					.WithEntityQuery("Notification", "个人发布中通知", new SF.Common.Notifications.Management.NotificationQueryArgument { State = NotificationState.Available, Mode = NotificationMode.Normal})
					.WithEntityQuery("Notification", "个人已过期通知", new SF.Common.Notifications.Management.NotificationQueryArgument { State = NotificationState.Expired, Mode = NotificationMode.Normal })
					.Ensure(sp, scope);

				 await sim.DefaultService<INotificationSendRecordManager, NotificationSendRecordManager>(null)
					.WithConsolePages(MenuPath)
					.Ensure(sp, scope);

				await sim.DefaultService<INotificationSendPolicyManager, NotificationSendPolicyManager>(null)
				   .WithConsolePages(MenuPath)
				   .Ensure(sp, scope);


				var debugProviderId = await sim.Service<INotificationSendProvider, DebugNotificationSendProvider>(null)
					.WithIdent("debug")
					.Ensure(sp, scope);

				 await sim.Service<INotificationSendProvider, SystemEMailProvider>(null)
					.WithIdent("email")
					.Ensure(sp, scope);



				 var pm = sp.Resolve<INotificationSendPolicyManager>();
				 await pm.EnsurePolicy(
					 "测试",
					 "测试",
					 "Name:{Name}",
					 "Content:{Content}",
					 new MessageSendAction
					 {
						 ProviderId = debugProviderId,
						 ContentTemplate = "ContentTemplate:{Content}",
						 ExtTemplateArguments = new[]
						 {
							new ExtTemplateArgument
							{
								Name="Arg1",
								Value="Arg1:{Arg1}"
							},
							new ExtTemplateArgument
							{
								Name="Arg2",
								Value="Arg2:{Arg2}"
							},
						 },
						 Name = "测试动作",
						 TargetTemplate = null,//"{Target}",
						 TitleTemplate = "TitleTemplate:{Title}",
					 }
					 );
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