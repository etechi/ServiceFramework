using SF.Sys.Services;
using SF.Sys.Entities;
using SF.Sys.Services.Management;
using SF.Common.Conversations.Managers;
using SF.Common.Conversations.Models;
using SF.Sys.Settings;
using SF.Sys.BackEndConsole;

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
			
			
			return sc;
		}

		
	}
}
