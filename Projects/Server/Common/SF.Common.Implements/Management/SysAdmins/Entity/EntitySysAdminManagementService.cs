using SF.Auth.Identities;
using SF.Core.Times;
using SF.Data;
using SF.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Management.SysAdmins.Entity
{
	public class EntitySysAdminManagementService<TSysAdmin> :
		EntityManager<long, Models.SysAdminInternal,  SysAdminQueryArgument, Models.SysAdminEditable, TSysAdmin>,
		ISysAdminManagementService
		where TSysAdmin: DataModels.SysAdmin<TSysAdmin>,new()
	{
		public Lazy<IIdentityService> IdentityService { get; }
		public Lazy<IIdentityCredentialProvider> SignupCredentialProvider { get; }

		public EntitySysAdminManagementService(
			IDataSetEntityManager<TSysAdmin> Manager,
			Lazy<IIdentityService> IdentityService,
			Lazy<IIdentityCredentialProvider> SignupCredentialProvider
			) : base(Manager)
		{
			this.IdentityService = IdentityService;
			this.SignupCredentialProvider = SignupCredentialProvider;
		}

		protected override PagingQueryBuilder<TSysAdmin> PagingQueryBuilder =>
			PagingQueryBuilder<TSysAdmin>.Simple("time", b => b.CreatedTime, true);

		protected override IContextQueryable<TSysAdmin> OnBuildQuery(IContextQueryable<TSysAdmin> Query, SysAdminQueryArgument Arg, Paging paging)
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
			m.CreatedTime =Now;
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
							Entity="系统管理员",
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
