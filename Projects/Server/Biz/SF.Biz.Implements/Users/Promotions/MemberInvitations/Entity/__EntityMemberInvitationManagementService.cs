using SF.Auth.Identities;
using SF.Core;
using SF.Core.CallPlans;
using SF.Core.Times;
using SF.Data;
using SF.Entities;
using SF.Users.Members.Models;
using SF.Users.Promotions.MemberInvitations.Entity.DataModels;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SF.Users.Promotions.MemberInvitations.Entity
{
	public class __EntityMemberInvitationManagementService<TMemberInvitation> :
		ModidifiableEntityManager<long, Models.MemberInvitationInternal, MemberInvitationQueryArgument, Models.MemberInvitationInternal, TMemberInvitation>,
		IMemberInvitationManagementService
		where TMemberInvitation : MemberInvatation<TMemberInvitation>, new()
	{
		public __EntityMemberInvitationManagementService(
			IDataSetEntityManager<TMemberInvitation> Manager
			) : base(Manager)
		{
			//CallPlanProvider.DelayCall(
			//	typeof(IMemberManagementService).FullName + "-" + Manager.ServiceInstanceDescroptor.InstanceId,
			//	"0",
			//	null,
			//	null,
			//	""
			//	);
		}

		protected override PagingQueryBuilder<TMemberInvitation> PagingQueryBuilder =>
			PagingQueryBuilder<TMemberInvitation>.Simple("time", b => b.CreatedTime, true);

		protected override IContextQueryable<TMemberInvitation> OnBuildQuery(IContextQueryable<TMemberInvitation> Query, MemberInvitationQueryArgument Arg, Paging paging)
		{
			var q = Query.Filter(Arg.Id, r => r.Id)
				.FilterContains(Arg.Name, r => r.Name)
				;
			return q;
		}
		
		protected override async Task OnNewModel(IModifyContext ctx)
		{
			var m = ctx.Model;
			m.Id = await IdentGenerator.GenerateAsync("会员邀请",0);
			m.CreatedTime = Now;
			await base.OnNewModel(ctx);
		}
		protected override Task OnUpdateModel(IModifyContext ctx)
		{
			var e = ctx.Editable;
			var m = ctx.Model;

			//UIEnsure.HasContent(e.Name, "请输入会员邀请");
			//m.Update(e, Now);

			return Task.CompletedTask;
		}
	}

}
