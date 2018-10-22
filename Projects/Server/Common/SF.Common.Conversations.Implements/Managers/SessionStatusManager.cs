
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Common.Conversations.Models;
using SF.Services.Security;
using SF.Sys;
using SF.Sys.Auth;
using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys.Settings;
using SF.Sys.TimeServices;
using SF.Sys.Services;
using SF.Common.Notifications.Management;
using SF.Sys.Logging;

namespace SF.Common.Conversations.Managers
{
	public class SessionStatusManager :
		AutoModifiableEntityManager<
			ObjectKey<long>,
			SessionStatus,
			SessionStatus,
			SessionQueryArgument,
			SessionEditable,
			DataModels.DataSessionStatus
			>,
		ISessionStatusManager
	{
		Lazy<IUserProfileService > UserProfileService { get; }
        Lazy<ISettingService<MessageNotifySetting>> MessageNotifySetting { get; }
        SessionSyncScope SessionSyncScope { get; }

        public SessionStatusManager(
			IEntityServiceContext ServiceContext,
			Lazy<IUserProfileService> UserProfileService,
            Lazy<ISettingService<MessageNotifySetting>> MessageNotifySetting,
            SessionSyncScope SessionSyncScope
			) : base(ServiceContext)
		{
            this.MessageNotifySetting = MessageNotifySetting;
            this.UserProfileService = UserProfileService;
            this.SessionSyncScope = SessionSyncScope;
            this.SetSyncQueue(SessionSyncScope, e => e.Id);
		}
		protected override async Task OnNewModel(IModifyContext ctx)
		{
			var editable = ctx.Editable;
			if (editable.BizIdentType.IsNullOrEmpty())
				throw new PublicArgumentException("必须指定业务标识类型");
			if(editable.BizIdent==0)
				throw new PublicArgumentException("必须指定业务标识");

			if (editable.Name.IsNullOrEmpty())
				editable.Name = editable.BizIdentType + "-" + editable.BizIdent;

			var model = ctx.Model;
			if (editable.OwnerId.HasValue)
			{
				var user = await UserProfileService.Value.GetUser(editable.OwnerId.Value);
				if (user == null)
					throw new ArgumentException("找不到会话所有人:" + editable.OwnerId);
			}
			await base.OnNewModel(ctx);
		}
		protected override async Task OnUpdateModel(IModifyContext ctx)
		{
			await base.OnUpdateModel(ctx);
		}

		public async Task<long> GetOrCreateSession(string BizIdentType, long BizIdent)
		{
			var sess = await DataScope.Use("查询已有会话", DataContext =>
				DataContext.Set<DataModels.DataSessionStatus>()
					.AsQueryable()
					.Where(s =>
						s.BizIdentType == BizIdentType &&
						s.BizIdent == BizIdent
						)
					.Select(s => new { s.LogicState, s.Id })
					.SingleOrDefaultAsync()
					);

			if (sess != null)
			{
				if (sess.LogicState != EntityLogicState.Enabled)
					throw new PublicArgumentException("找不到指定会话:" + sess.Id);
				return sess.Id;
			}

			var re= await CreateAsync(new SessionEditable
			{
				BizIdent = BizIdent,
				BizIdentType = BizIdentType
			});
			return re.Id;
		}

        public Task UserMessageNotify(long SessionID)
        {
            Logger.Info($"会话消息通知6 SessionID:{SessionID}");
            return SessionSyncScope.Queue(SessionID, () =>
             DataScope.Use("发送消息通知", async ctx =>
             {
                 Logger.Info($"会话消息通知7 SessionID{SessionID}");
                 var time = Now;
                 var setting = MessageNotifySetting.Value.Value;
                 var message_expire_time = time.AddHours(-setting.MaxMessageExpireHours);
                 var sess = await (
                     from s in ctx.Queryable<DataModels.DataSessionStatus>()
                     where s.Id == SessionID && s.LastMessageId.HasValue
                     select new
                     {
                         LastMessageUserId = s.LastMessage.UserId,
                         s.LastMemberId,
                         s.LastMessageText,
                         s.UpdatedTime
                     }).SingleOrDefaultAsync();

                 Logger.Info($"会话消息通知7 sess: {sess.LastMemberId} {sess.LastMessageText} {sess.LastMessageUserId}");
                 if (sess == null && sess.UpdatedTime < message_expire_time)
                     return;

                 var min_notify_interval = time.AddMinutes(-setting.MinNotifyIntervalMinutes);
                 var members = await (
                     from m in ctx.Queryable<DataModels.DataSessionMemberStatus>()
                     where m.SessionId == SessionID &&
                        m.OwnerId.HasValue &&
                         m.LastNotifyTime < min_notify_interval &&
                         (!m.LastReadTime.HasValue || m.LastReadTime.Value < time) &&
                         m.Id != sess.LastMemberId
                     select m
                     ).ToArrayAsync();

                 Logger.Info($"会话消息通知8 members: {members.Length}");

                 if (members.Length == 0)
                     return;

                 Logger.Info($"会话消息通知9");
                 var nm = ServiceContext.ServiceProvider.Resolve<INotificationManager>();
                 var sender = await UserProfileService.Value.GetUser(sess.LastMessageUserId.Value);
                 var args = new System.Collections.Generic.Dictionary<string, object>
                 {
                     {"用户",sender.Name },
                     {"内容",sess.LastMessageText }
                 };
                 await nm.CreateNotification(
                     Notifications.NotificationMode.Normal,
                     members.Select(m => m.OwnerId.Value).ToArray(),
                     "会话消息提醒",
                     args,
                     BizIdent: $"会话消息提醒-{SessionID}-{sess.UpdatedTime.ToString("yyyyMMddHHmmss")}",
                     Name: $"会话消息提醒:{SessionID}:{sess.LastMessageText.Limit(50)}",
                     Content: sess.LastMessageText
                     ); 

                 foreach (var member in members)
                 {
                     member.LastNotifyTime = time;
                     ctx.Update(member);
                 }
                 await ctx.SaveChangesAsync();
                 Logger.Info($"会话消息通知10");
             })
            );
        }
    }

}
