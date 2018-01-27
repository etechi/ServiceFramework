
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Common.UserGroups.Models;
using SF.Services.Security;
using SF.Sys;
using SF.Sys.Auth;
using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys.TimeServices;

namespace SF.Common.UserGroups.Managers
{
	static class JoinStateDetector
	{
		public static GroupJoinState Detect(bool? SessionAccepted, bool? MemberAccepted)
		{
			return MemberAccepted.HasValue ? (
				   SessionAccepted.HasValue ?
					(
						MemberAccepted.Value ?
							(SessionAccepted.Value ?
								GroupJoinState.Joined :
								GroupJoinState.ApplyRejected
								) :
							(SessionAccepted.Value ?
								GroupJoinState.InviteRejected :
								GroupJoinState.ApplyRejected
							)
					) : (
						MemberAccepted.Value ?
							GroupJoinState.Applying :
							GroupJoinState.None
					)
				) : SessionAccepted.HasValue ?
					GroupJoinState.Inviting :
					GroupJoinState.None;
		}
		
	}
	public class GroupMemberManager<TGroup,TMember, TMemberEditable,TQueryArument, TDataGroup,TDataMember> :
		AutoModifiableEntityManager<
			ObjectKey<long>,
			TMember,
			TMember,
			TQueryArument,
			TMemberEditable,
			TDataMember
			>,
		IGroupMemberManager<TGroup,TMember,TMemberEditable,TQueryArument>
		where TGroup:Group<TGroup,TMember>
		where TMember:GroupMember<TGroup,TMember>,new()
		where TMemberEditable:GroupMember<TGroup,TMember>,new()
		where TQueryArument:GroupMemberQueryArgument,new()
		where TDataGroup:DataModels.DataGroup<TDataGroup, TDataMember>
		where TDataMember : DataModels.DataGroupMember<TDataGroup,TDataMember>,new()
	{
		Lazy<IDataProtector> DataProtector { get; }
		Lazy<ITimeService> TimeService { get; }
		Lazy<IUserProfileService> UserProfileService { get; }

		public GroupMemberManager(
			IEntityServiceContext ServiceContext,
			GroupSyncScope SessionSyncScope,
			Lazy<IDataProtector> DataProtector,
			Lazy<ITimeService> TimeService,
			Lazy<IUserProfileService> UserProfileService
			) : base(ServiceContext)
		{
			this.DataProtector = DataProtector;
			this.TimeService = TimeService;
			this.UserProfileService = UserProfileService;
			SetSyncQueue(SessionSyncScope, e => e.GroupId);
		}
		async Task UpdateMemberCount(IDataContext ctx,long SessionId,int Diff)
		{
			var Session = await ctx.Set<TDataGroup>().FindAsync(SessionId);
			Session.MemberCount += Diff;
			ctx.Update(Session);
		}
		internal static GroupJoinState DetectJoinState(bool? SessionAccepted,bool? MemberAccepted)
		{
			return JoinStateDetector.Detect(SessionAccepted, MemberAccepted);

		}
		protected override async Task OnUpdateModel(IModifyContext ctx)
		{
			var editable = ctx.Editable;
			var model = ctx.Model;
			var orgExists = model.LogicState == EntityLogicState.Enabled && model.JoinState == GroupJoinState.Joined ? 1 : 0;
			var newJoinState = DetectJoinState(editable.GroupAccepted, editable.MemberAccepted);
			var newExists = editable.LogicState == EntityLogicState.Enabled && newJoinState == GroupJoinState.Joined ? 1 : 0;
			var memberCountDiff = newExists - orgExists;
			model.JoinState = newJoinState;

			if (memberCountDiff != 0)
				await UpdateMemberCount(ctx.DataContext,editable.GroupId, memberCountDiff);
			await base.OnUpdateModel(ctx);
		}
		protected override async Task OnRemoveModel(IModifyContext ctx)
		{
			var model = ctx.Model;
			if (model.LogicState == EntityLogicState.Enabled && model.JoinState==GroupJoinState.Joined)
				await UpdateMemberCount(ctx.DataContext,model.GroupId, -1);
			
			await base.OnRemoveModel(ctx);
		}
		protected override async Task OnNewModel(IModifyContext ctx)
		{
			var model = ctx.Model;
			model.LastActiveTime = Now;

			var editable = ctx.Editable;
			if (!editable.OwnerId.HasValue)
				throw new ArgumentNullException("未指定成员用户");

			var user= await UserProfileService.Value.GetUser(editable.OwnerId.Value);
			if(user==null)
				throw new ArgumentException("找不到指定的用户");

			if (editable.Name.IsNullOrEmpty())
				editable.Name = user.Name;
			if (editable.Icon.IsNullOrEmpty())
				editable.Icon = user.Icon;

			await base.OnNewModel(ctx);
		}

		public async Task<long> MemberEnsure(
			long GroupId,
			long TargetUserId,
			bool GroupAccepted,
			bool MemberAccepted,
			int BizType,
			string BizIdentType,
			long? BizIdent			
			)
		{
			var data = await DataScope.Use(
				"查找已有成员",
				ctx =>
				{
					var Members = ctx.Set<TMember>().AsQueryable();
					var sq = from s in ctx.Set<TDataGroup>().AsQueryable()
							 where s.Id == GroupId && s.LogicState == EntityLogicState.Enabled
							 join tm in Members on s.Id equals tm.GroupId into tms
							 from m in tms.Where(tm => tm.OwnerId == TargetUserId).DefaultIfEmpty()
							 select new
							 {
								 s.Flags,
								 Member = m == null ? null : new
								 {
									 m.Id,
									 m.LogicState,
									 m.MemberAccepted,
									 m.GroupAccepted
								 }
							 };
					return sq.SingleOrDefaultAsync();
				});
			if (data == null)
				throw new PublicArgumentException("指定的用户组已被删除");
			if (data.Flags.HasFlag(SessionFlag.Public))
				GroupAccepted = true;
			if (data.Member == null)
			{
				var m = await CreateAsync(new TMemberEditable
				{
					GroupId = GroupId,
					OwnerId = TargetUserId,
					MemberAccepted = MemberAccepted ? (bool?)true : null,
					GroupAccepted = GroupAccepted ? (bool?)true : null
				});
				return m.Id;
			}
			else if (data.Member.LogicState != EntityLogicState.Enabled)
				throw new PublicDeniedException("成员已被禁止加入");
			else if (
				(data.Member.GroupAccepted ?? false) != GroupAccepted ||
				(data.Member.MemberAccepted ?? false) != MemberAccepted)
			{
				var m = await LoadForEdit(ObjectKey.From(data.Member.Id));
				if (GroupAccepted)
					m.GroupAccepted = GroupAccepted;
				if (MemberAccepted)
					m.MemberAccepted = MemberAccepted;
				await UpdateAsync(m);
				return m.Id;
			}
			else
				return data.Member.Id;
		}

		class ActionArgument
		{
			public long sid { get; set; }
			public long? uid { get; set; }
			public bool ma { get; set; }
			public bool sa { get; set; }
			public int bt { get; set; }
			public string bit{ get; set; }
			public long? bi { get; set; }
		}
		static byte[] InvitationTokenSecKey { get; } = new byte[] { 123, 21, 49, 51, 201, 14, 78, 45 };

		public async Task<string> CreateMemberEnsureToken(
			string ActionName,
			long SessionId,
			long? TargetUserId,
			bool MemberAccepted,
			bool SessionAccepted,
			int BizType,
			string BizIdentType,
			long? BizIdent
			)
		{
			var token = (await DataProtector.Value.Encrypt(
				"交谈用户组成员动作:" + ActionName,
				Json.Stringify(new ActionArgument
				{
					sid = SessionId,
					uid = TargetUserId,
					ma = MemberAccepted,
					sa=SessionAccepted,
					bi=BizIdent,
					bit=BizIdentType,
					bt=BizType
				}).UTF8Bytes(),
				TimeService.Value.Now.AddDays(7),
				InvitationTokenSecKey
				)).Base64();

			return token;
		}

		public async Task<long> EvalMemberEnsureToken(
			string ActionName,
			string Token,
			long UserId
			)
		{
			var data = await DataProtector.Value.Decrypt(
				"交谈用户组成员动作:" + ActionName,
				Token.Base64(),
				TimeService.Value.Now,
				(d, l) => Task.FromResult(InvitationTokenSecKey)
				);

			var arg = Json.Parse<ActionArgument>(data.UTF8String());
			if (arg.uid.HasValue && arg.uid.Value != UserId)
				throw new PublicDeniedException("用户不一致");
			var bizIdent = arg.bi ?? (arg.bit == "AuthUser" ? (long?)UserId : null);
			return await MemberEnsure(
				arg.sid, 
				UserId, 
				arg.sa, 
				arg.ma, 
				arg.bt, 
				arg.bit,
				bizIdent
				);
		}
	}

}
