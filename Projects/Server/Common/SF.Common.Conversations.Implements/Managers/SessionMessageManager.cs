
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Common.Conversations.DataModels;
using SF.Common.Conversations.Models;
using SF.Sys;
using SF.Sys.Data;
using SF.Sys.Entities;

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
		private async Task<int> UpdateSessionStatus(SessionMessage editable, DataModels.DataSessionMessage model)
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
			return Session.MessageCount;
		}
		protected override async Task OnUpdateModel(IModifyContext ctx)
		{
			//需要在此设置发信成员，以及设置发信成员的最后消息
			if(ctx.Action==ModifyAction.Create && ctx.Editable.UserId.HasValue)
			{
				var editable = ctx.Editable;
				var model = ctx.Model;

				var msgCount=await UpdateSessionStatus(editable, model);
				var mid = await ((SessionMemberStatusManager)SessionMemberStatusManager.Value).InternalSetLastMessage(
					editable.SessionId,
					editable.UserId.Value,
					ctx.Model.Id,
					editable.Time,
					msgCount
					);
				ctx.Model.PosterId = mid;
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
				default:
					throw new ArgumentException("不支持指定的消息类型:" + editable.Type);
			}
		}
	}

}
