
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
		

	}

}
