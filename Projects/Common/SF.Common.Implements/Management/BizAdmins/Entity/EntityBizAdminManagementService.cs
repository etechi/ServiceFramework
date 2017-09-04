﻿using SF.Auth.Identities;
using SF.Core.Times;
using SF.Data;
using SF.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Management.BizAdmins.Entity
{
	public class EntityBizAdminManagementService<TBizAdmin> :
		EntityManager<long, Models.BizAdminInternal,  BizAdminQueryArgument, Models.BizAdminEditable, TBizAdmin>,
		IBizAdminManagementService
		where TBizAdmin: DataModels.BizAdmin<TBizAdmin>,new()
	{
		public Lazy<ITimeService> TimeService { get; }
		public Lazy<IIdentityService> IdentityService { get; }
		public Lazy<IIdentGenerator> IdentGenerator { get; }
		public Lazy<IIdentityCredentialProvider> SignupCredentialProvider { get; }

		public EntityBizAdminManagementService(
			IDataSet<TBizAdmin> DataSet,
			Lazy<ITimeService> TimeService,
			Lazy<IIdentityService> IdentityService,
			Lazy<IIdentGenerator> IdentGenerator,
			Lazy<IIdentityCredentialProvider> SignupCredentialProvider
			) : base(DataSet)
		{
			this.TimeService = TimeService;
			this.IdentityService = IdentityService;
			this.IdentGenerator = IdentGenerator;
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

		protected override async Task OnNewModel(ModifyContext ctx)
		{
			var m = ctx.Model;
			m.Id = await IdentGenerator.Value.GenerateAsync("系统管理员",0);
			m.CreatedTime = TimeService.Value.Now;
			m.OwnerId = m.Id;
			await base.OnNewModel(ctx);
		}
		protected override async Task OnUpdateModel(ModifyContext ctx)
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
							Entity="业务管理员",
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