
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Common.Conversations.DataModels;
using SF.Common.Conversations.Models;
using SF.Sys;
using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys.Services;
using SF.Sys.Settings;
using SF.Sys.Threading;
using SF.Sys.Logging;

namespace SF.Common.Conversations.Managers
{
    
	public class SessionMessageManager :
		AutoModifiableEntityManager<
			ObjectKey<long>,
			SessionMessage,
			SessionMessage,
			SessionMessageQueryArgument,
			SessionMessage,
			DataModels.DataSessionMessage
			>,
		ISessionMessageManager
	{
		Lazy<ISessionMemberStatusManager> SessionMemberStatusManager { get; }
		Lazy<ISessionStatusManager> SessionStatusManager { get; }
        public SessionMessageManager(
			IEntityServiceContext ServiceContext, 
			SessionSyncScope SessionSyncScope,
			Lazy<ISessionMemberStatusManager> SessionMemberStatusManager,
			Lazy<ISessionStatusManager> SessionStatusManager
            ) : base(ServiceContext)
		{
            SetSyncQueue(SessionSyncScope, e => e.SessionId);
			this.SessionMemberStatusManager = SessionMemberStatusManager;
			this.SessionStatusManager = SessionStatusManager;

        }

		public async Task<long> SendSystemMessage(
			string BizIdentType, 
			long BizIdent, 
			string Message,
			string Argument
			)
		{
			var sid = await SessionStatusManager.Value.GetOrCreateSession(
				BizIdentType, 
				BizIdent
				);

			var id = await CreateAsync(
				new Models.SessionMessage
				{
					SessionId = sid,
					Text = Message,
					Argument=Argument,
					Type = MessageType.System,
				});

			return id.Id;
		}

		public Task<SessionMessageDetail> GetMessageDetail(long Id)
		{
			return DataScope.Use("查找消息详情", ctx =>
				 (from m in ctx.Queryable<DataModels.DataSessionMessage>()
				  where m.Id == Id
				  select new SessionMessageDetail
				  {
					  Id = m.Id,
					  Argument=m.Argument,
					  MemberBizIdent=null,
					  MemberBizType=null,
					  PosterId=m.PosterId,
					  SessionBizIdent=m.Session.BizIdent,
					  SessionBizType=m.Session.BizIdentType,
					  SessionId=m.SessionId,
					  SessionName=m.Session.Name,
					  SessionOwnerId=m.Session.OwnerId,
					  PosterName=m.Poster.Name,
					  Text=m.Text,
					  Time=m.Time,
					  Type=m.Type,
					  UserId=m.UserId
				  }).SingleOrDefaultAsync()
				);
		}
		private async Task<DataModels.DataSessionStatus> UpdateSessionStatus(IDataContext DataContext, SessionMessage editable, DataModels.DataSessionMessage model)
		{
			var Session = await DataContext.Set<DataModels.DataSessionStatus>().FindAsync(editable.SessionId);
			if (Session == null)
				throw new ArgumentException("找不到会话:" + editable.SessionId);

			Session.LastMessageId = model.Id;
			Session.UpdatedTime = Now;
			Session.LastMessageText = editable.Text;
			Session.LastMessageType = editable.Type;
			Session.MessageCount++;

			
			DataContext.Update(Session);

            DataContext.AddCommitTracker(
                TransactionCommitNotifyType.AfterCommit, 
                (type, e) =>
                {
                    var setting = ServiceContext.ServiceProvider.Resolve<ISettingService<MessageNotifySetting>>();
                    var scoped_manager = ServiceContext.ServiceProvider.Resolve<IScoped<ISessionStatusManager>>();
                    Debouncer.Start(Session.Id, (cancelled) =>
                    {
                        if (cancelled) return;
                        Task.Run(async () =>
                        {
                            try
                            {
                                await scoped_manager.Use(async smm =>
                                {
                                    await smm.UserMessageNotify(Session.Id);
                                    return 0;
                                }
                            );
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex, "发送消息提醒时发生异常");
                            }
                        });
                    }, setting.Value.MinMessageNotifyDelaySeconds*1000, setting.Value.MaxMessageNotifyDelaySeconds * 1000);
                });
			return Session;
		}
		async Task UpdateSessionLastMember(IDataContext DataContext, DataModels.DataSessionStatus Session, long PosterId, DateTime Time)
		{
			if (Session.LastMemberId != PosterId)
			{
				if (Session.LastMemberId.HasValue)
				{
					var LastMember = await DataContext.Set<DataModels.DataSessionMemberStatus>().FindAsync(Session.LastMemberId.Value);
					if (LastMember == null)
						throw new ArgumentException("找不到成员:" + Session.LastMemberId.Value);

					LastMember.MessageSectionCount++;
					LastMember.MessageSectionTotalTime = (int)Time.Subtract(Session.LastMemberStartTime.Value).TotalSeconds;
					DataContext.Update(LastMember);
				}

				Session.LastMemberId = PosterId;
				Session.LastMemberStartTime = Time;
				Session.LastMemberMessageCount = 1;
			}
			else
				Session.LastMemberMessageCount++;

		}
		protected override async Task OnUpdateModel(IModifyContext ctx)
		{
			//需要在此设置发信成员，以及设置发信成员的最后消息
			if(ctx.Action==ModifyAction.Create && ctx.Editable.UserId.HasValue)
			{
				var editable = ctx.Editable;
				var model = ctx.Model;

				var session=await UpdateSessionStatus(ctx.DataContext,editable, model);
				var mid = await ((SessionMemberStatusManager)SessionMemberStatusManager.Value).InternalSetLastMessage(
					editable.SessionId,
					editable.UserId.Value,
					ctx.Model.Id,
					editable.Time,
					session.MessageCount
					);
				ctx.Model.PosterId = mid;
				await UpdateSessionLastMember(ctx.DataContext, session, mid, editable.Time);
			}
			await base.OnUpdateModel(ctx);
		}
		protected override async Task OnNewModel(IModifyContext ctx)
		{
			var editable = ctx.Editable;
			ValidateArguments(editable);

			editable.Text = editable.Text?.Trim();
			editable.Argument = editable.Argument?.Trim();

			if (editable.Time == DateTime.MinValue)
				editable.Time = Now;

			var model = ctx.Model;
			await base.OnNewModel(ctx);

		}


		private static void ValidateArguments(SessionMessage editable)
		{
			if (editable.SessionId == 0)
				throw new ArgumentException("为指定会话ID");

			switch (editable.Type)
			{
				case MessageType.Text:
					if (!editable.Text.HasContent())
						throw new PublicArgumentException("请输入消息内容");
					break;
				case MessageType.Voice:
					if (!editable.Argument.HasContent())
						throw new PublicArgumentException("请提供语音资源");
					if (editable.Text.IsNullOrEmpty())
						editable.Text = "【语音】";


				break;
				case MessageType.Image:
					if (!editable.Argument.HasContent())
						throw new PublicArgumentException("请提供图片资源");
					if (editable.Text.IsNullOrEmpty())
						editable.Text = "【图片】";

					break;
				case MessageType.System:
					if (!editable.Text.HasContent())
						throw new PublicArgumentException("未指定消息内容");
					break;
				default:
					throw new ArgumentException("不支持指定的消息类型:" + editable.Type);
			}
		}
	}

}
