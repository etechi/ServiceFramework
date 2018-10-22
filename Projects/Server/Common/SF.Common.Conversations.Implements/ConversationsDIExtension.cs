using SF.Sys.Services;
using SF.Sys.Entities;
using SF.Sys.Services.Management;
using SF.Common.Conversations.Managers;
using SF.Common.Conversations.Models;
using SF.Sys.Settings;
using SF.Sys.BackEndConsole;
using SF.Common.Notifications.Management;
using SF.Common.Notifications.Senders;

namespace SF.Sys.Services
{
	public static class ConversationDIExtension

	{
		public static IServiceCollection AddConversationServices(this IServiceCollection sc,string TablePrefix=null)
		{
			
			sc.AddSingleton<SessionSyncScope>();
						sc.EntityServices(
				"Conversation",
				"交谈",
				d => d
					.Add<ISessionStatusManager, SessionStatusManager>("Session", "会话", typeof(SessionStatus))
					.Add<ISessionMemberStatusManager, SessionMemberStatusManager>("SessionMember", "会话成员", typeof(SessionMemberStatus))
					.Add<ISessionMessageManager, SessionMessageManager>("SessionMessage", "会话消息", typeof(SessionMessage), typeof(SF.Common.Conversations.Front.SessionMessage))
				);

			sc.AddManagedScoped<SF.Common.Conversations.Front.IConversationService, SF.Common.Conversations.Front.ConversationService>();
			
			sc.AddDataModules<
				SF.Common.Conversations.DataModels.DataSessionStatus,
				SF.Common.Conversations.DataModels.DataSessionMemberStatus,
				SF.Common.Conversations.DataModels.DataSessionMessage
				>(TablePrefix ?? "Conversation");

            sc.AddSetting(new Common.Conversations.MessageNotifySetting
            {
                MaxMessageExpireHours=24,
                MaxMessageNotifyDelaySeconds=5*60,
                MinMessageNotifyDelaySeconds=3*60,
                MinNotifyIntervalMinutes=60                
            }
            );

            sc.InitServices("交谈", async (sp, sim, scope) =>
			{
				var MenuPath = "用户内容/会话服务";
				await sim.Service<ISessionStatusManager, SessionStatusManager>(null)
					.WithConsolePages(MenuPath)
					.Ensure(sp, scope);

				await sim.Service<ISessionMemberStatusManager, SessionMemberStatusManager>(null)
					.WithConsolePages(MenuPath)
					.Ensure(sp, scope);

				await sim.Service<ISessionMessageManager, SessionMessageManager>(null)
					.WithConsolePages(MenuPath)
					.Ensure(sp, scope);

				await sim.Service<SF.Common.Conversations.Front.IConversationService, SF.Common.Conversations.Front.ConversationService>(null)
					.Ensure(sp, scope);

			});

            sc.AddInitializer("data", "初始化消息提醒设置", async sp =>
            {

                var mpm = sp.Resolve<INotificationSendPolicyManager>();
                var sim = sp.Resolve<IServiceInstanceManager>();

                var jpushProvider = await sim.GetService<INotificationSendProvider>("jpush");
                var debugProvider = await sim.GetService<INotificationSendProvider>("debug");
                await mpm.EnsurePolicy(
                        "会话消息提醒",
                        "会话消息提醒",
                        "来自{用户}的新消息: {内容}",
                        null,
                        new Common.Notifications.Models.MessageSendAction
                        {
                            ProviderId = jpushProvider.Id,
                            Name = "会话提醒",
                            TitleTemplate = "{用户}: {内容}"
                        },
                        new Common.Notifications.Models.MessageSendAction
                        {
                            ProviderId = debugProvider.Id,
                            Name = "会话提醒",
                            TitleTemplate = "{用户}: {内容}"
                        }
                    );
            });
            return sc;
		}

		
	}
}
