using SF.Sys.Services;
using SF.Sys.Entities;
using SF.Sys.Services.Management;
using SF.Common.Conversations.Managers;
using SF.Common.Conversations.Models;
using SF.Sys.Settings;

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
					.Add<ISessionManager, SessionManager>("Session", "会话", typeof(Session))
					.Add<ISessionMemberManager, SessionMemberManager>("SessionMember", "会话成员", typeof(SessionMember))
					.Add<ISessionMessageManager, SessionMessageManager>("SessionMessage", "会话消息", typeof(SessionMessage), typeof(SF.Common.Conversations.Front.SessionMessage))
				);

			sc.AddManagedScoped<SF.Common.Conversations.Front.IConversationService, SF.Common.Conversations.Front.ConversationService>();
			
			sc.AddDataModules<
				SF.Common.Conversations.DataModels.DataSession,
				SF.Common.Conversations.DataModels.DataSessionMember,
				SF.Common.Conversations.DataModels.DataSessionMessage
				>(TablePrefix ?? "Conversation");


			sc.InitServices("交谈", async (sp, sim, scope) =>
			{
				var MenuPath = "系统/交谈服务";
				await sim.Service<ISessionManager, SessionManager>(null)
					.WithMenuItems(MenuPath)
					.Ensure(sp, scope);

				await sim.Service<ISessionMemberManager, SessionMemberManager>(null)
					.WithMenuItems(MenuPath)
					.Ensure(sp, scope);

				await sim.Service<ISessionMessageManager, SessionMessageManager>(null)
					.WithMenuItems(MenuPath)
					.Ensure(sp, scope);

				await sim.Service<SF.Common.Conversations.Front.IConversationService, SF.Common.Conversations.Front.ConversationService>(null)
					.Ensure(sp, scope);

			});
			
			
			return sc;
		}

		
	}
}
