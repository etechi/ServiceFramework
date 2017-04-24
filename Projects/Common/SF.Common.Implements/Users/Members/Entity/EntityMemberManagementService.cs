using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Auth.Identities.Models;
using SF.Auth.Identities.Internals;
using SF.Core.Caching;
using SF.Data.Entity;
using SF.Auth.Identities.DataModels;
using SF.Core.Times;
using SF.Auth.Identities;
using SF.Users.Members.Models;
using SF.Clients;

namespace SF.Users.Members.Entity
{
	public class EntityMemberManagementService<TMember,TMemberSource> :
		EntityManager<long, Models.MemberInternal,  MemberQueryArgument, Models.MemberEditable, TMember>,
		IMemberManagementService
		where TMember: DataModels.Member<TMember,TMemberSource>,new()
		where TMemberSource: DataModels.MemberSource<TMember, TMemberSource>
	{
		public Lazy<ITimeService> TimeService { get; }
		public Lazy<IIdentityService> IdentityService { get; }
		public Lazy<IIdentGenerator> IdentGenerator { get; }
		public EntityMemberManagementService(
			IDataSet<TMember> DataSet,
			Lazy<ITimeService> TimeService,
			Lazy<IIdentityService> IdentityService,
			Lazy<IIdentGenerator> IdentGenerator
			) : base(DataSet)
		{
			this.TimeService = TimeService;
			this.IdentityService = IdentityService;
			this.IdentGenerator = IdentGenerator;
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
			var ctx = await InternalCreateAsync(
				new MemberEditable
				{
					Name=Arg.Identity?.Name,
					Icon=Arg.Identity?.Icon,
					Password=Arg.Password,
					PhoneNumber=Arg.Credential,
				},
				Tuple.Create(Arg, CredentialProvider)
				);
			return (string)ctx.UserData;
		}
		protected override async Task OnNewModel(ModifyContext ctx)
		{
			var m = ctx.Model;
			m.Id = await IdentGenerator.Value.GenerateAsync("会员");
			m.CreatedTime = TimeService.Value.Now;
			m.OwnerId = m.Id;
			await base.OnNewModel(ctx);
		}
		protected override async Task OnUpdateModel(ModifyContext ctx)
		{
			var e = ctx.Editable;
			var m = ctx.Model;

			UIEnsure.HasContent(e.Name,"请输入姓名");
			UIEnsure.HasContent(e.PhoneNumber, "请输入电话");


			m.Icon = e.Icon;
			m.Name = e.Name.Trim();
			m.PhoneNumber = e.PhoneNumber.Trim();
			m.ObjectState = e.ObjectState;
			m.UpdatedTime = e.UpdatedTime;
			m.UpdatorId = await IdentityService.Value.EnsureCurIdentityId();

			if (ctx.Action == ModifyAction.Create)
			{
				var ExtraArg = (Tuple<CreateIdentityArgument,IIdentityCredentialProvider>)ctx.ExtraArgument;
				UIEnsure.HasContent(e.Password, "需要提供密码");
				ctx.UserData=await IdentityService.Value.CreateIdentity(
					new CreateIdentityArgument
					{
						
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
					false,
					ExtraArg.Item2
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
