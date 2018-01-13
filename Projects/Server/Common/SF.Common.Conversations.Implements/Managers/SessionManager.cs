
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Common.Conversations.Models;
using SF.Sys.Auth;
using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys.Settings;

namespace SF.Common.Conversations.Managers
{
	public class SessionManager :
		AutoModifiableEntityManager<
			ObjectKey<long>,
			Session,
			Session,
			SessionQueryArgument,
			SessionEditable,
			DataModels.DataSession
			>,
		ISessionManager
	{
		Lazy<IUserProfileService > UserProfileService { get; }
		public SessionManager(
			IEntityServiceContext ServiceContext,
			Lazy<IUserProfileService> UserProfileService,
			SessionSyncScope SessionSyncScope
			) : base(ServiceContext)
		{
			this.UserProfileService = UserProfileService;
			this.SetSyncQueue(SessionSyncScope, e => e.Id);
		}
		protected override async Task OnNewModel(IModifyContext ctx)
		{
			var editable = ctx.Editable;
			var model = ctx.Model;
			if (!editable.OwnerId.HasValue)
				throw new ArgumentException("必须指定会话所有人");

			var user = await UserProfileService.Value.GetUser(editable.OwnerId.Value);
			if (user == null)
				throw new ArgumentException("找不到用户:" + editable.OwnerMemberId);

			if (editable.Name == null)
				editable.Name = user.Name + "的会话";
			if (editable.Icon == null)
				editable.Icon = user.Icon;
			
			model.LastActiveTime = Now;
			await base.OnNewModel(ctx);

			if (editable.Members == null)
				editable.Members = new[]
				{
					new SessionMember
					{
						SessionId=model.Id,
						Name =user.Name,
						Icon=user.Icon,
						OwnerId=user.Id
					}
				};

		}
		protected override async Task OnUpdateModel(IModifyContext ctx)
		{


			await base.OnUpdateModel(ctx);
			


			if(ctx.Editable.Members!=null)
				ctx.Model.MemberCount =
					ctx.Editable.Members
						.Count(m => m.LogicState == EntityLogicState.Enabled);
			if (ctx.Action == ModifyAction.Create)
			{
				foreach (var m in ctx.Model.Members)
					m.LastActiveTime = Now;

				//由于循环引用，不能直接设置OwnerMemberId，需要先保存，然后重新设置
				//由数据库事务确保原子性
				await DataContext.SaveChangesAsync();

				DataContext.Update(ctx.Model);

				ctx.Model.OwnerMemberId = ctx.Model.Members.First().Id;
			}
		}
		public async Task<long> EnsureSession(long UserId)
		{
			var e=await this.EnsureEntity (
				await this.QuerySingleEntityIdent(new SessionQueryArgument { OwnerId=UserId }),
				() =>
				{
					var re=new Models.SessionEditable
					{
						LogicState = EntityLogicState.Enabled,
						OwnerId = UserId,
					};
					return re;
				});
			return e.Id;
		}
	}

}
