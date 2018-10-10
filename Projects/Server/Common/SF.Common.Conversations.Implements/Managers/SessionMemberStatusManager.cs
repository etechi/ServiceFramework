
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Common.Conversations.Models;
using SF.Services.Security;
using SF.Sys;
using SF.Sys.Auth;
using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys.TimeServices;

namespace SF.Common.Conversations.Managers
{
	public class SessionMemberStatusManager :
		AutoModifiableEntityManager<
			ObjectKey<long>,
			SessionMemberStatus,
			SessionMemberStatus,
			SessionMemberQueryArgument,
			SessionMemberStatus,
			DataModels.DataSessionMemberStatus
			>,
		ISessionMemberStatusManager
	{
		Lazy<IUserProfileService> UserProfileService { get; }
		public SessionMemberStatusManager(
			IEntityServiceContext ServiceContext,
			SessionSyncScope SessionSyncScope,
			Lazy<IDataProtector> DataProtector,
			Lazy<ITimeService> TimeService,
			Lazy<IUserProfileService> UserProfileService
			) : base(ServiceContext)
		{
			this.UserProfileService = UserProfileService;
			SetSyncQueue(SessionSyncScope, e => e.SessionId);
		}
		
		
		protected override async Task OnUpdateModel(IModifyContext ctx)
		{
			var editable = ctx.Editable;
			var model = ctx.Model;
			//如果修改了读取时间，但未修改读取数，重新获得读取数
			if (editable.LastReadTime != model.LastReadTime && 
				editable.MessageReaded==model.MessageReaded
				)
			{
				var messageCount = await ctx.DataContext
					.Set<DataModels.DataSessionStatus>()
					.AsQueryable()
					.Where(s => s.Id == editable.SessionId)
					.Select(s => s.MessageCount)
					.SingleOrDefaultAsync();
				model.MessageReaded = messageCount;
			}

			await base.OnUpdateModel(ctx);
		}
		protected override async Task OnRemoveModel(IModifyContext ctx)
		{
			var model = ctx.Model;
			await base.OnRemoveModel(ctx);
		}
		protected override async Task OnNewModel(IModifyContext ctx)
		{
			var model = ctx.Model;
			model.LastMessageTime = Now;

			var editable = ctx.Editable;
			if (!editable.OwnerId.HasValue)
				throw new ArgumentNullException("未指定成员用户");

			var user= await UserProfileService.Value.GetUser(editable.OwnerId.Value);
			if(user==null)
				throw new ArgumentException("找不到指定的用户");

			if (editable.Name.IsNullOrEmpty())
				editable.Name = user.Name;

			await base.OnNewModel(ctx);
		}
		internal async Task<long> InternalSetLastMessage(long SessionId,long UserId,long MessageId,DateTime Now,int MessageCount)
		{
			AutoSaveChanges = false;
			var re=await base.InternalCreateOrUpdateAsync(
				NewModifyContext(),
				new SessionMemberStatus
				{
					SessionId = SessionId,
					OwnerId = UserId
				},
				m => m.SessionId == SessionId && m.OwnerId.HasValue && m.OwnerId.Value == UserId,
				ctx =>
				{
					var m = ctx.Model;
					m.LastMessageId = MessageId;
					m.LastMessageTime = Now;
					m.MessageCount++;
					m.LastReadTime = Now;
					m.MessageReaded = MessageCount;
					return Task.FromResult(m.Id);
				}
				);
			return re.Id.Id;
		}
		public Task SetReadTime(long SessionId, long UserId)
		{
			return base.CreateOrUpdateAsync(
				new SessionMemberStatus
				{
					SessionId = SessionId,
					OwnerId = UserId
				},
				m => m.SessionId == SessionId && m.OwnerId.HasValue && m.OwnerId.Value == UserId,
				ctx =>
				{
					ctx.Editable.LastReadTime = Now;
				}
				);
		}

	}

}
