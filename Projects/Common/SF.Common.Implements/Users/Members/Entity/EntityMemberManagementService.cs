using SF.Auth.Identities;
using SF.Core.Times;
using SF.Data;
using SF.Entities;
using SF.Users.Members.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Users.Members.Entity
{
	public class EntityMemberManagementService<TMember,TMemberSource> :
		EntityManager<long, Models.MemberInternal,  MemberQueryArgument, Models.MemberEditable, TMember>,
		IMemberManagementService
		where TMember: DataModels.Member<TMember,TMemberSource>,new()
		where TMemberSource: DataModels.MemberSource<TMember, TMemberSource>
	{
		public Lazy<IIdentityService> IdentityService { get; }
		public EntityMemberManagementService(
			IDataSetEntityManager<TMember> Manager,
			Lazy<IIdentityService> IdentityService
			) : base(Manager)
		{
			this.IdentityService = IdentityService;
		}

		protected override PagingQueryBuilder<TMember> PagingQueryBuilder =>
			PagingQueryBuilder<TMember>.Simple("time", b => b.CreatedTime, true);

		protected override IContextQueryable<TMember> OnBuildQuery(IContextQueryable<TMember> Query, MemberQueryArgument Arg, Paging paging)
		{
			var q = Query.Filter(Arg.Id, r => r.Id)
				.FilterContains(Arg.Name, r => r.Name)
				.FilterContains(Arg.PhoneNumber, r => r.PhoneNumber)
				.Filter(Arg.InvitorId, r => r.InvitorId)
				;
			if (Arg.MemberSourceId.HasValue)
			{
				var sid = Arg.MemberSourceId.Value;
				q = q.Where(r =>
				  r.MemberSourceId.HasValue && r.MemberSourceId.Value == sid ||
				  r.ChildMemberSourceId.HasValue && r.ChildMemberSourceId.Value == sid
				);
			}
			return q;
		}
		public async Task<string> CreateMemberAsync(
			CreateIdentityArgument Arg,
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
					PhoneNumber=Arg.Credential,
				},
				(Arg, CredentialProvider)
				);
			return (string)ctx.UserData;
		}
		protected override async Task OnNewModel(IModifyContext ctx)
		{
			var m = ctx.Model;
			m.Id = await IdentGenerator.GenerateAsync("会员",0);
			m.CreatedTime = TimeService.Now;
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
			m.UpdatedTime = e.UpdatedTime;
			m.UpdatorId = await IdentityService.Value.EnsureCurIdentityId();

			if (ctx.Action == ModifyAction.Create)
			{
				var ExtraArg = ((CreateIdentityArgument,IIdentityCredentialProvider))ctx.ExtraArgument;
				UIEnsure.HasContent(e.Password, "需要提供密码");
				ctx.UserData=await IdentityService.Value.CreateIdentity(
					new CreateIdentityArgument
					{
						CredentialProviderId=ExtraArg.Item1.CredentialProviderId,
						Credential = m.PhoneNumber,
						Password = e.Password.Trim(),
						Identity = new Auth.Identities.Models.Identity
						{
							Entity="会员",
							Icon = e.Icon,
							Id = m.Id,
							Name=m.Name
						},
						ReturnToken= ExtraArg.Item1.ReturnToken,
						CaptchaCode= ExtraArg.Item1.CaptchaCode,
						VerifyCode= ExtraArg.Item1.VerifyCode,
						Expires= ExtraArg.Item1.Expires
					}, 
					false
					);
				EntityManager.AddPostAction(() =>
				{
					

				}, PostActionType.BeforeCommit);
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
