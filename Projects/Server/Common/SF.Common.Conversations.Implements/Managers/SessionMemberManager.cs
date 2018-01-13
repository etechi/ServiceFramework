
using System.Threading.Tasks;
using SF.Common.Conversations.Models;
using SF.Sys.Data;
using SF.Sys.Entities;

namespace SF.Common.Conversations.Managers
{
	public class SessionMemberManager :
		AutoModifiableEntityManager<
			ObjectKey<long>,
			SessionMember,
			SessionMember,
			SessionMemberQueryArgument,
			SessionMember,
			DataModels.DataSessionMember
			>,
		ISessionMemberManager
	{
		public SessionMemberManager(
			IEntityServiceContext ServiceContext,
			SessionSyncScope SessionSyncScope
			) : base(ServiceContext)
		{
			SetSyncQueue(SessionSyncScope, e => e.SessionId);
		}
		async Task UpdateMemberCount(long SessionId,int Diff)
		{
			var Session = await DataContext.Set<DataModels.DataSession>().FindAsync(SessionId);
			Session.MemberCount += Diff;
			DataContext.Update(Session);
		}
		protected override async Task OnUpdateModel(IModifyContext ctx)
		{
			var editable = ctx.Editable;
			var model = ctx.Model;

			model.JoinState = 
				editable.MemberAccepted.HasValue ? (
					editable.SessionAccepted.HasValue ?
					(
						editable.MemberAccepted.Value ?
							(editable.SessionAccepted.Value ?
								SessionJoinState.Joined :
								SessionJoinState.ApplyRejected
								) :
							(editable.SessionAccepted.Value ?
								SessionJoinState.InviteRejected :
								SessionJoinState.ApplyRejected
							)
					) : (
						editable.MemberAccepted.Value ?
							SessionJoinState.Applying :
							SessionJoinState.None
					)
				) : editable.SessionAccepted.HasValue ?
					SessionJoinState.Inviting :
					SessionJoinState.None;



			var memberCountDiff = 0;
			if(ctx.Action==ModifyAction.Create && editable.LogicState==EntityLogicState.Enabled ||
				ctx.Action==ModifyAction.Update && model.LogicState!=EntityLogicState.Enabled && editable.LogicState==EntityLogicState.Enabled)
			{
				memberCountDiff = 1;
			}
			else if(ctx.Action==ModifyAction.Update && 
				model.LogicState==EntityLogicState.Enabled && 
				editable.LogicState!=EntityLogicState.Enabled
				)
			{
				memberCountDiff = -1;
			}
			if (memberCountDiff != 0)
				await UpdateMemberCount(editable.SessionId, memberCountDiff);
			await base.OnUpdateModel(ctx);
		}
		protected override async Task OnRemoveModel(IModifyContext ctx)
		{
			var model = ctx.Model;
			if (model.LogicState == EntityLogicState.Enabled)
				await UpdateMemberCount(model.SessionId, -1);
			
			await base.OnRemoveModel(ctx);
		}
		protected override Task OnNewModel(IModifyContext ctx)
		{
			var model = ctx.Model;
			model.LastActiveTime = Now;
			return base.OnNewModel(ctx);
		}
	}

}
