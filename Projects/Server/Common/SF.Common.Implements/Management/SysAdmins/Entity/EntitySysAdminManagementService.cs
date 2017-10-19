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
		ModidifiableEntityManager<ObjectKey<long>, Models.SysAdminInternal,  SysAdminQueryArgument, Models.SysAdminEditable, TSysAdmin>,
		ISysAdminManagementService
		where TSysAdmin: DataModels.SysAdmin<TSysAdmin>,new()
	{
		public Lazy<IIdentityService> IdentityService { get; }
		public Lazy<IIdentityCredentialProvider> SignupCredentialProvider { get; }

		public EntitySysAdminManagementService(
			IDataSetEntityManager<Models.SysAdminEditable,TSysAdmin> Manager,
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
				.WithScope(ServiceInstanceDescriptor)
				;
			
			return q;
		}

		protected override async Task OnNewModel(IModifyContext ctx)
		{
			var m = ctx.Model;
			m.Id = await IdentGenerator.GenerateAsync();
			m.CreatedTime =Now;
			m.OwnerId = m.Id;
			m.ScopeId = DataScopeId;
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
							OwnerId="系统管理员",
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
