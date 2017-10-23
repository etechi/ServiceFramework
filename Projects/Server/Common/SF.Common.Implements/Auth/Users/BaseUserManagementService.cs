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

namespace SF.Auth.Users
{
	public abstract class BaseUserManagementService<TUserRegisted,TUserInternal,TUserEditable,TUserQueryArgument,TUser> :
		ModidifiableEntityManager<ObjectKey<long>, TUserInternal,  TUserQueryArgument, TUserEditable, TUser>,
		IUserManagementService<TUserInternal,TUserEditable,TUserQueryArgument>,
		ICallable
		where TUser: DataModels.BaseUserModel<TUser>,new()
		where TUserInternal:Models.UserInternal
		where TUserEditable:Models.UserEditable,new()
		where TUserQueryArgument:UserQueryArgument,new()
		where TUserRegisted:UserRegisted,new()
	{
		public Lazy<IIdentityService> IdentityService { get; }
		public BaseUserManagementService(
			IDataSetEntityManager<TUserEditable,TUser> Manager,
			Lazy<IIdentityService> IdentityService,
			ICallPlanProvider CallPlanProvider
			) : base(Manager)
		{
			this.IdentityService = IdentityService;
			//CallPlanProvider.DelayCall(
			//	typeof(IMemberManagementService).FullName + "-" + Manager.ServiceInstanceDescroptor.InstanceId,
			//	"0",
			//	null,
			//	null,
			//	""
			//	);
		}

		protected override PagingQueryBuilder<TUser> PagingQueryBuilder =>
			PagingQueryBuilder<TUser>.Simple("time", b => b.CreatedTime, true);

		protected override IContextQueryable<TUser> OnBuildQuery(IContextQueryable<TUser> Query, TUserQueryArgument Arg, Paging paging)
		{
			var q = Query.Filter(Arg.Id, r => r.Id)
				.FilterContains(Arg.Name, r => r.Name)
				.FilterContains(Arg.PhoneNumber, r => r.PhoneNumber)
				.WithScope(ServiceInstanceDescriptor)
				;
			//if (Arg.MemberSourceId.HasValue)
			//{
			//	var sid = Arg.MemberSourceId.Value;
			//	q = q.Where(r =>
			//	  r.MemberSourceId.HasValue && r.MemberSourceId.Value == sid ||
			//	  r.ChildMemberSourceId.HasValue && r.ChildMemberSourceId.Value == sid
			//	);
			//}
			return q;
		}
		public async Task<string> CreateUserAsync(
			CreateIdentityArgument Arg
			)
		{
			var ctx = NewModifyContext();
			await InternalCreateAsync(
				ctx,
				new TUserEditable
				{
					Name=Arg.Identity?.Name,
					Icon=Arg.Identity?.Icon,
					Password=Arg.Password,
					PhoneNumber=Arg.Credential
				},
				Arg
				);
			return (string)ctx.UserData;
		}
		protected override async Task OnNewModel(IModifyContext ctx)
		{
			var m = ctx.Model;
			m.Id = await IdentGenerator.GenerateAsync();
			m.CreatedTime = Now;
			m.OwnerId = m.Id;
			m.ScopeId = DataScopeId;
			await base.OnNewModel(ctx);
		}
		protected override async Task OnUpdateModel(IModifyContext ctx)
		{
			var e = ctx.Editable;
			var m = ctx.Model;

			UIEnsure.HasContent(e.Name,"请输入姓名");
			UIEnsure.HasContent(e.PhoneNumber, "请输入电话");


			m.Icon = e.Icon;
			m.Name = e.Name.Trim();
			m.PhoneNumber = e.PhoneNumber.Trim();
			m.LogicState = e.LogicState;
			m.UpdatedTime = Now;
			//m.UpdatorId = await IdentityService.Value.EnsureCurIdentityId();

			if (ctx.Action == ModifyAction.Create)
			{
				var CreateArgument= (TCreateUserArgument)ctx.ExtraArgument;


				UIEnsure.HasContent(e.Password, "需要提供密码");
				var sess=await IdentityService.Value.CreateIdentity(
					new CreateIdentityArgument
					{
						CredentialProviderId= CreateArgument.CredentialProviderId,
						Credential = m.PhoneNumber,
						Password = e.Password.Trim(),
						Identity = new Auth.Identities.Models.Identity
						{
							OwnerId=ServiceEntityIdent.Create(ServiceInstanceDescriptor.InstanceId,m.Id),
							Icon = e.Icon,
							Name=m.Name
						},
						ReturnToken= CreateArgument.ReturnToken,
						CaptchaCode= CreateArgument.CaptchaCode,
						VerifyCode= CreateArgument.VerifyCode,
						Expires= CreateArgument.Expires,
						ExtraArgument=CreateArgument.ExtraArgument
					}, 
					false
					);
				ctx.UserData = sess;
				var iid = await IdentityService.Value.ParseAccessToken(sess);
				m.SignupIdentityId = iid;

				EntityManager.AddPostAction(() =>
					EntityManager.EventEmitter.Emit(new TUserRegisted
					{
						UserId = m.Id,
						ServiceId = ServiceInstanceDescriptor.InstanceId,
						Time= m.CreatedTime
					})
				, PostActionType.BeforeCommit);
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
		Task Execute(Expression<Func<ICallContext,Task>> expr)
		{
			return null;
		}
		class MemberSignup
		{
			public long ServiceScopeId { get; set; }
			public long MemberId { get; set; }
		}
		Task ICallable.Execute(ICallContext CallContext)
		{
			Execute(
				ctx=>
					EntityManager.EventEmitter.Emit(Json.Parse<MemberSignup>(ctx.Argument),true)
				);
			return null;
		}
	}

}
