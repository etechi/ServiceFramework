using SF.Auth.Identities;
using SF.Core;
using SF.Core.CallPlans;
using SF.Core.Times;
using SF.Data;
using SF.Entities;
using SF.Users.Members.Models;
using SF.Users.Promotions.MemberSources.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SF.Users.Promotions.MemberSources.Entity
{
	public class EntityMemberSourceManagementService<TMemberSource, TSourceMember> :
		EntityManager<long, MemberSourceInternal,  MemberSourceQueryArgument, Models.MemberSourceInternal, TMemberSource>,
		IMemberSourceManagementService
		where TMemberSource: DataModels.MemberSource<TMemberSource, TSourceMember>, new()
		where TSourceMember : DataModels.SourceMember<TMemberSource,TSourceMember>,new()
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

		public async Task AddSourceMember(long SourceId, long MemberId)
		{
			//m.InvitorId = CreateArgument.InvitorId;
			//var PntSourceId = await DataContext.Set<TMemberSource>().SingleOrDefaultAsync(
			//	mi => mi.Id == SourceId,
			//	mi => mi.ContainerId.HasValue ? mi.ContainerId : 0
			//	);
			//if (PntSourceId.HasValue)
			//{
			//	if (PntSourceId.Value == 0)
			//		m.MemberSourceId = CreateArgument.MemberSourceId;
			//	else
			//	{
			//		m.ChildMemberSourceId = CreateArgument.MemberSourceId;
			//		m.MemberSourceId = PntSourceId;
			//	}
			//}

			DataContext.Set<TSourceMember>().Add(new TSourceMember
			{
				Id = MemberId,
				ContainerId = SourceId
			});
			await DataContext.SaveChangesAsync();
		}

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
