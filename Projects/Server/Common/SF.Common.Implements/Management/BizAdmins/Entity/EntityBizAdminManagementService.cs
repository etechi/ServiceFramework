using SF.Auth.Identities;
using SF.Core.Times;
using SF.Data;
using SF.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Management.BizAdmins.Entity
{
	public class EntityBizAdminManagementService<TBizAdmin> :
		ModidifiableEntityManager<Models.BizAdminInternal,  BizAdminQueryArgument, Models.BizAdminEditable, TBizAdmin>,
		IBizAdminManagementService
		where TBizAdmin: DataModels.BizAdmin<TBizAdmin>,new()
	{
		public Lazy<IIdentityService> IdentityService { get; }
		public Lazy<IIdentityCredentialProvider> SignupCredentialProvider { get; }

		public EntityBizAdminManagementService(
			IDataSetEntityManager<Models.BizAdminEditable,TBizAdmin> EntityManager,
			Lazy<IIdentityService> IdentityService,
			Lazy<IIdentityCredentialProvider> SignupCredentialProvider
			) : base(EntityManager)
		{
			this.IdentityService = IdentityService;
			this.SignupCredentialProvider = SignupCredentialProvider;
		}

		protected override PagingQueryBuilder<TBizAdmin> PagingQueryBuilder =>
			PagingQueryBuilder<TBizAdmin>.Simple("time", b => b.CreatedTime, true);

		protected override IContextQueryable<TBizAdmin> OnBuildQuery(IContextQueryable<TBizAdmin> Query, BizAdminQueryArgument Arg, Paging paging)
		{
			var q = Query.Filter(Arg.Id, r => r.Id)
				.FilterContains(Arg.Name, r => r.Name)
				.FilterContains(Arg.Account, r => r.Account)
				;
			
			return q;
		}

		protected override async Task OnNewModel(IModifyContext ctx)
		{
			var m = ctx.Model;
			m.Id = await IdentGenerator.GenerateAsync("系统管理员",0);
			m.CreatedTime = Now;
			m.OwnerId = m.Id;
			await base.OnNewModel(ctx);
		}
		protected override async Task OnUpdateModel(IModifyContext ctx)
		{
			var e = ctx.Editable;
			var m = ctx.Model;

			UIEnsure.HasContent(e.Name,"请输入姓名");
			UIEnsure.HasContent(e.Account, "请输入账号");

			m.Icon = e.Icon;
			m.Name = e.Name.Trim();
			m.Account = e.Account.Trim();
			m.LogicState = e.LogicState;
			m.UpdatedTime = e.UpdatedTime;
			m.UpdatorId = await IdentityService.Value.EnsureCurIdentityId();

			if (ctx.Action == ModifyAction.Create)
			{
				UIEnsure.HasContent(e.Password, "需要提供密码");
				ctx.UserData=await IdentityService.Value.CreateIdentity(
					new CreateIdentityArgument
					{
						Credential = m.Account,
						//CredentialProviderId=
						Password = e.Password.Trim(),
						Identity = new Auth.Identities.Models.Identity
						{
							OwnerId="svc-"+ServiceInstanceDescriptor.InstanceId+"-"+m.Id,
							Icon = e.Icon,
							Id = m.Id,
							Name=m.Name
						}
					}, 
					false
					);
			}
			else
			{
				await IdentityService.Value.UpdateIdentity(
					new Auth.Identities.Models.Identity
					{
						Id = e.Id,
						Icon = e.Icon,
						Name = e.Name
					});
			}
		}
	}

}
