#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0


using SF.Biz.MemberSources.Models;
using System.Linq;
using System.Threading.Tasks;
using SF.Sys.Entities;
using SF.Sys.Data;
namespace SF.Biz.MemberSources
{
	public class EntityMemberSourceManagementService<TMemberSource, TSourceMember> :
		ModidifiableEntityManager<ObjectKey<long>, MemberSourceInternal,  MemberSourceQueryArgument, Models.MemberSourceInternal, TMemberSource>,
		IMemberSourceManagementService
		where TMemberSource: DataModels.MemberSource<TMemberSource, TSourceMember>, new()
		where TSourceMember : DataModels.SourceMember<TMemberSource,TSourceMember>,new()
	{
		public EntityMemberSourceManagementService(
			IEntityServiceContext ServiceContext

			) : base(ServiceContext)
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
			m.Id = await IdentGenerator.GenerateAsync< TMemberSource>();
			m.CreatedTime = Now;
			await base.OnNewModel(ctx);
		}
		protected override Task OnUpdateModel(IModifyContext ctx)
		{
			var e = ctx.Editable;
			var m = ctx.Model;

			UIEnsure.HasContent(e.Name,"请输入渠道名称");
			m.Update(e, Now);

			return Task.CompletedTask;
		}
	}

}
