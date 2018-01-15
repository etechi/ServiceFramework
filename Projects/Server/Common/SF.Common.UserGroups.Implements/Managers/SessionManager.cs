
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
			else
				editable.Name = string.Format(editable.Name, user.Name);

			if (editable.Icon == null)
				editable.Icon = user.Icon;
			
			model.LastActiveTime = Now;
			await base.OnNewModel(ctx);

			if (editable.Members == null)
				editable.Members = new[]
				{
					new SessionMember
					{
						OwnerId=user.Id
					}
				};

			foreach (var m in editable.Members)
				m.SessionId = model.Id;
			var om = editable
				.Members
				.Where(mi=>mi.OwnerId==editable.OwnerId)
				.SingleOrDefault();
			if (om == null)
				throw new PublicArgumentException("缺少会话所有人");

			if (om.Name.IsNullOrEmpty())
				om.Name = user.Name;
			if (om.Icon.IsNullOrEmpty())
				om.Icon = user.Icon;
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
		

	}

}
