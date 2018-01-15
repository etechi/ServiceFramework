
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Common.Conversations.Models;
using SF.Services.Security;
using SF.Sys;
using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys.TimeServices;

namespace SF.Common.Conversations.Managers
{
	public class SessionMemberManager :
		AutoModifiableEntityManager<
			ObjectKey<long>,
			SessionMember,
			SessionMember,
			SessionMemberQueryArgument,
			SessionMember,
			DataModels.DataSessionMember
			>,
		ISessionMemberManager
	{
		Lazy<IDataProtector> DataProtector { get; }
		Lazy<ITimeService> TimeService { get; }

		public SessionMemberManager(
			IEntityServiceContext ServiceContext,
			SessionSyncScope SessionSyncScope,
			Lazy<IDataProtector> DataProtector,
			Lazy<ITimeService> TimeService
			) : base(ServiceContext)
		{
			this.DataProtector = DataProtector;
			this.TimeService = TimeService;
			SetSyncQueue(SessionSyncScope, e => e.SessionId);
		}
		async Task UpdateMemberCount(long SessionId,int Diff)
		{
			var Session = await DataContext.Set<DataModels.DataSession>().FindAsync(SessionId);
			Session.MemberCount += Diff;
			DataContext.Update(Session);
		}
		protected override async Task OnUpdateModel(IModifyContext ctx)
		{
			var editable = ctx.Editable;
			var model = ctx.Model;

			model.JoinState = 
				editable.MemberAccepted.HasValue ? (
					editable.SessionAccepted.HasValue ?
					(
						editable.MemberAccepted.Value ?
							(editable.SessionAccepted.Value ?
								SessionJoinState.Joined :
								SessionJoinState.ApplyRejected
								) :
							(editable.SessionAccepted.Value ?
								SessionJoinState.InviteRejected :
								SessionJoinState.ApplyRejected
							)
					) : (
						editable.MemberAccepted.Value ?
							SessionJoinState.Applying :
							SessionJoinState.None
					)
				) : editable.SessionAccepted.HasValue ?
					SessionJoinState.Inviting :
					SessionJoinState.None;



			var memberCountDiff = 0;
			if(ctx.Action==ModifyAction.Create && editable.LogicState==EntityLogicState.Enabled ||
				ctx.Action==ModifyAction.Update && model.LogicState!=EntityLogicState.Enabled && editable.LogicState==EntityLogicState.Enabled)
			{
				memberCountDiff = 1;
			}
			else if(ctx.Action==ModifyAction.Update && 
				model.LogicState==EntityLogicState.Enabled && 
				editable.LogicState!=EntityLogicState.Enabled
				)
			{
				memberCountDiff = -1;
			}
			if (memberCountDiff != 0)
				await UpdateMemberCount(editable.SessionId, memberCountDiff);
			await base.OnUpdateModel(ctx);
		}
		protected override async Task OnRemoveModel(IModifyContext ctx)
		{
			var model = ctx.Model;
			if (model.LogicState == EntityLogicState.Enabled)
				await UpdateMemberCount(model.SessionId, -1);
			
			await base.OnRemoveModel(ctx);
		}
		protected override Task OnNewModel(IModifyContext ctx)
		{
			var model = ctx.Model;
			model.LastActiveTime = Now;
			return base.OnNewModel(ctx);
		}

		public async Task<long> MemberEnsure(
			long SessionId,
			long TargetUserId,
			bool SessionAccepted,
			bool MemberAccepted,
			int BizType,
			string BizIdentType,
			long? BizIdent			
			)
		{
			var sq = from s in DataContext.Set<DataModels.DataSession>().AsQueryable()
					 where s.Id == SessionId && s.LogicState == EntityLogicState.Enabled
					 let m = (from m in s.Members
							 where m.OwnerId == TargetUserId
							 select new
							 {
								 m.Id,
								 m.LogicState,
								 m.MemberAccepted,
								 m.SessionAccepted
							 }).SingleOrDefault()
					 select new
					 {
						 s.Flags,
						 Member = m
					 };
			var data = await sq.SingleOrDefaultAsync();
			if (data == null)
				throw new PublicArgumentException("指定的会话已被删除");
			if (data.Flags.HasFlag(SessionFlag.Public))
				SessionAccepted = true;
			if (data.Member == null)
			{
				var m = await CreateAsync(new SessionMember
				{
					SessionId = SessionId,
					OwnerId = TargetUserId,
					BizIdent = BizIdent,
					BizIdentType = BizIdentType,
					BizType = BizType,
					MemberAccepted = MemberAccepted ? (bool?)true : null,
					SessionAccepted = SessionAccepted ? (bool?)true : null
				});
				return m.Id;
			}
			else if (data.Member.LogicState != EntityLogicState.Enabled)
				throw new PublicDeniedException("成员已被禁止加入");
			else if (
				(data.Member.SessionAccepted ?? false) != SessionAccepted ||
				(data.Member.MemberAccepted ?? false) != MemberAccepted)
			{
				var m = await LoadForEdit(ObjectKey.From(data.Member.Id));
				if (SessionAccepted)
					m.SessionAccepted = SessionAccepted;
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
				"交谈会话成员动作:" + ActionName,
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
				"交谈会话成员动作:" + ActionName,
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
