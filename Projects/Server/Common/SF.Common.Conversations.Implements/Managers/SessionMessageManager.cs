﻿
using System;
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
		Lazy<ISessionMemberManager> SessionMemberManager { get; }
		public SessionMessageManager(
			IEntityServiceContext ServiceContext, 
			SessionSyncScope SessionSyncScope,
			Lazy<ISessionMemberManager> SessionMemberManager
			) : base(ServiceContext)
		{
			SetSyncQueue(SessionSyncScope, e => e.SessionId);
			this.SessionMemberManager = SessionMemberManager;

		}
		protected override async Task OnUpdateModel(IModifyContext ctx)
		{
			//需要在此设置发信成员，以及设置发信成员的最后消息
			if(ctx.Action==ModifyAction.Create && ctx.Editable.UserId.HasValue)
			{
				var mid = await ((SessionMemberManager)SessionMemberManager.Value).InternalSetLastMessage(
					ctx.Editable.SessionId,
					ctx.Editable.UserId.Value,
					ctx.Model.Id
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

			await UpdateSessionStatus(editable, model);

		}

		private async Task UpdateSessionStatus(SessionMessage editable, DataModels.DataSessionMessage model)
		{
			var Session = await DataContext.Set<DataModels.DataSession>().FindAsync(editable.SessionId);
			if (Session == null)
				throw new ArgumentException("找不到会话:" + editable.SessionId);

			Session.LastMessageId = model.Id;
			Session.LastMessageTime = Now;
			Session.MessageCount++;
			DataContext.Update(Session);
		}

		private static void ValidateArguments(SessionMessage editable)
		{
			if (editable.SessionId == 0)
				throw new ArgumentException("需要指定会话");

			switch (editable.Type)
			{
				case MessageType.Text:
					if (!editable.Text.HasContent())
						throw new PublicArgumentException("请输入消息内容");
					break;
				case MessageType.Voice:
					if (!editable.Argument.HasContent())
						throw new PublicArgumentException("请提供语音资源");
					break;
				case MessageType.Image:
					if (!editable.Argument.HasContent())
						throw new PublicArgumentException("请提供图片资源");
					break;
				default:
					throw new ArgumentException("不支持指定的消息类型:" + editable.Type);
			}
		}
	}

}
