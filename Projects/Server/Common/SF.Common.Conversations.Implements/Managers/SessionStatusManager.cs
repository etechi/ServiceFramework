
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

		public SessionStatusManager(
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
			var sess = await DataContext.Set<DataModels.DataSessionStatus>()
				.AsQueryable()
				.Where(s =>
					s.BizIdentType == BizIdentType &&
					s.BizIdent == BizIdent
					)
				.Select(s => new { s.LogicState,s.Id })
				.SingleOrDefaultAsync();

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
	}

}
