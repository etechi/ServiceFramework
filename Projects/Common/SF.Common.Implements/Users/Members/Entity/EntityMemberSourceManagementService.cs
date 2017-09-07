using SF.Auth.Identities;
using SF.Core;
using SF.Core.CallPlans;
using SF.Core.Times;
using SF.Data;
using SF.Entities;
using SF.Users.Members.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SF.Users.Members.Entity
{
	public class EntityMemberSourceManagementService<TMember,TMemberSource> :
		EntityManager<long, Models.MemberSourceInternal,  MemberSourceQueryArgument, Models.MemberSourceInternal, TMemberSource>,
		IMemberSourceManagementService
		where TMember: DataModels.Member<TMember,TMemberSource>
		where TMemberSource: DataModels.MemberSource<TMember, TMemberSource>, new()
	{
		public EntityMemberSourceManagementService(
			IDataSetEntityManager<TMemberSource> Manager
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

		protected override PagingQueryBuilder<TMemberSource> PagingQueryBuilder =>
			PagingQueryBuilder<TMemberSource>.Simple("time", b => b.CreatedTime, true);

		protected override IContextQueryable<TMemberSource> OnBuildQuery(IContextQueryable<TMemberSource> Query, MemberSourceQueryArgument Arg, Paging paging)
		{
			var q = Query.Filter(Arg.Id, r => r.Id)
				.FilterContains(Arg.Name, r => r.Name)
				;
			return q;
		}
		
		protected override async Task OnNewModel(IModifyContext ctx)
		{
			var m = ctx.Model;
			m.Id = await IdentGenerator.GenerateAsync("会员渠道",0);
			m.CreatedTime = TimeService.Now;
			await base.OnNewModel(ctx);
		}
		protected override Task OnUpdateModel(IModifyContext ctx)
		{
			var e = ctx.Editable;
			var m = ctx.Model;

			UIEnsure.HasContent(e.Name,"请输入渠道名称");
			m.Update(e, TimeService.Now);

			return Task.CompletedTask;
		}
	}

}
