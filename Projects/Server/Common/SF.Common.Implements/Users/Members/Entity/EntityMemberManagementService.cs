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
	public class EntityMemberManagementService<TMember> :
		ModidifiableEntityManager<Models.MemberInternal,  MemberQueryArgument, Models.MemberEditable, TMember>,
		IMemberManagementService,
		ICallable
		where TMember: DataModels.Member<TMember>,new()
	{
		public Lazy<IIdentityService> IdentityService { get; }
		public EntityMemberManagementService(
			IDataSetEntityManager<Models.MemberEditable,TMember> Manager,
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

		protected override PagingQueryBuilder<TMember> PagingQueryBuilder =>
			PagingQueryBuilder<TMember>.Simple("time", b => b.CreatedTime, true);

		protected override IContextQueryable<TMember> OnBuildQuery(IContextQueryable<TMember> Query, MemberQueryArgument Arg, Paging paging)
		{
			var q = Query.Filter(Arg.Id, r => r.Id)
				.FilterContains(Arg.Name, r => r.Name)
				.FilterContains(Arg.PhoneNumber, r => r.PhoneNumber)
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
		public async Task<string> CreateMemberAsync(
			CreateMemberArgument Arg,
			IIdentityCredentialProvider CredentialProvider
			)
		{
			var ctx = NewModifyContext();
			await InternalCreateAsync(
				ctx,
				new MemberEditable
				{
					Name=Arg.Identity?.Name,
					Icon=Arg.Identity?.Icon,
					Password=Arg.Password,
					PhoneNumber=Arg.Credential
				},
				(Arg, CredentialProvider)
				);
			return (string)ctx.UserData;
		}
		protected override async Task OnNewModel(IModifyContext ctx)
		{
			var m = ctx.Model;
			m.Id = await IdentGenerator.GenerateAsync("会员",0);
			m.CreatedTime = Now;
			m.OwnerId = m.Id;
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
			m.UpdatorId = await IdentityService.Value.EnsureCurIdentityId();

			if (ctx.Action == ModifyAction.Create)
			{
				var (CreateArgument,IdentityProvider)= ((CreateMemberArgument, IIdentityCredentialProvider))ctx.ExtraArgument;


				UIEnsure.HasContent(e.Password, "需要提供密码");
				var sess=await IdentityService.Value.CreateIdentity(
					new CreateIdentityArgument
					{
						CredentialProviderId= IdentityProvider?.Id,
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
					EntityManager.EventEmitter.Emit(new MemberRegisted
					{
						MemberId = m.Id,
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
