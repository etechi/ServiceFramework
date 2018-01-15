
using System.Threading.Tasks;
using SF.Sys.Entities;
using SF.Sys.Auth;
using System;
using SF.Sys.Data;
using System.Linq;
using SF.Sys.Clients;
using SF.Sys;
using SF.Sys.TimeServices;
using System.Collections.Generic;

namespace SF.Common.Conversations.Front
{


	/// <summary>
	/// 会话服务
	/// </summary>
	public class ConversationService : IConversationService
	{
		IDataContext DataContext { get; }
		IAccessToken AccessToken { get; }
		Lazy<Managers.ISessionManager> SessionManager { get; }
		Lazy<Managers.ISessionMemberManager> SessionMemberManager { get; }
		Lazy<Managers.ISessionMessageManager> SessionMessageManager { get; }
		Lazy<ITimeService> TimeService { get; }
		Lazy<IUserProfileService> UserProfileService { get; }
		public long EnsureUserIdent() =>
			AccessToken.User.EnsureUserIdent();


		public ConversationService(
			IDataContext DataContext, 
			IAccessToken AccessToken,
			Lazy<Managers.ISessionManager> SessionManager,
			Lazy<Managers.ISessionMessageManager> SessionMessageManager,
			Lazy<Managers.ISessionMemberManager> SessionMemberManager,
			Lazy<ITimeService> TimeService,
			Lazy<IUserProfileService> UserProfileService
			)
		{
			this.AccessToken = AccessToken;
			this.DataContext = DataContext;
			this.SessionManager= SessionManager;
			this.SessionMemberManager = SessionMemberManager;
			this.SessionMessageManager = SessionMessageManager;
			this.TimeService = TimeService;
			this.UserProfileService = UserProfileService;
		}
		
		async Task<DataModels.DataSessionMember> EnsureSessionMember(long SessionId,long UserId)
		{
			var member = await DataContext
				.Set<DataModels.DataSessionMember>()
				.AsQueryable()
				.Where(m => 
					m.SessionId == SessionId && 
					m.OwnerId == UserId &&
					m.JoinState==SessionJoinState.Joined
					)
				.SingleOrDefaultAsync();
			if (member==null)
				throw new PublicDeniedException("您已经离开会话");
			return member;
		}

		/// <summary>
		/// 查询会话消息
		/// </summary>
		/// <param name="Arg">查询参数</param>
		/// <returns></returns>
		public async Task<QueryResult<SessionMessage>> QueryMessage(MessageQueryArgument Arg)
		{
			if (Arg.SessionId == 0)
				throw new PublicArgumentException("必须指定会话");

			var user = EnsureUserIdent();
			await EnsureSessionMember(Arg.SessionId, user);

			var q = from m in DataContext.Set<DataModels.DataSessionMessage>().AsQueryable()
					where m.SessionId == Arg.SessionId 
					select m;
			if (Arg.StartId.HasValue)
				q = q.Where(m => m.Id > Arg.StartId.Value);
			var rq = from m in q
					 orderby m.Id ascending
					select new SessionMessage
					{
						Id = m.Id,
						Argument = m.Argument,
						SessionId = m.SessionId,
						Text = m.Text,
						Time = m.Time,
						Type = m.Type,
						PosterId = m.PosterId,
						PosterIcon = m.PosterId.HasValue ? m.Poster.Icon : null,
						PosterName = m.PosterId.HasValue ? m.Poster.Name : null
					};
			var re = await rq.ToQueryResultAsync(Arg.Paging);
			return re;
		}

		/// <summary>
		/// 查询会话成员
		/// </summary>
		/// <param name="Arg">查询参数</param>
		/// <returns></returns>
		public async Task<QueryResult<SessionMember>> QueryMember(MemberQueryArgument Arg)
		{
			var user = EnsureUserIdent();

			var q = DataContext.Set<DataModels.DataSessionMember>().AsQueryable();
			if (Arg.Id != null)
			{
				var id = Arg.Id.Id;
				q = q.Where(m => m.Id == id && m.OwnerId == user && m.LogicState == EntityLogicState.Enabled);
			}
			else
			{
				if (!Arg.SessionId.HasValue && !Arg.OwnerId.HasValue)
					return QueryResult<SessionMember>.Empty;

				q = q.Where(m => m.LogicState == EntityLogicState.Enabled);
				if (Arg.SessionId.HasValue)
				{
					var cid = Arg.SessionId.Value;
					await EnsureSessionMember(cid, user);
					q = q.Where(m => m.SessionId == cid);
				}
				if (Arg.OwnerId.HasValue)
					q = q.Where(m => 
						m.OwnerId == Arg.OwnerId
						);
				q = q.OrderByDescending(m => m.LastActiveTime);
			}

			var rq =from m in q
				let lm=m.LastMessage
				
				select new SessionMember
				{
					Id = m.Id,
					SessionId=m.SessionId,
					Name=m.Name,
					Icon = m.Icon,
					LastActiveTime = m.LastActiveTime,
					MessageCount=m.MessageCount,
					BizIdentType=m.BizIdentType,
					BizType=m.BizType,
					BizIdent=m.BizIdent,
					JoinState=m.JoinState,
					LastMessage=lm==null && m.JoinState == SessionJoinState.Joined?null:new SessionMessage
					{
						Argument=lm.Argument,
						SessionId=lm.SessionId,
						Text=lm.Text,
						Id=lm.Id,
						Time=lm.Time,
						Type=lm.Type
					}
				};
			var re = await rq.ToQueryResultAsync(Arg.Paging);
			return re;
		}

		/// <summary>
		/// 查询会话
		/// </summary>
		/// <param name="Arg">查询参数</param>
		/// <returns></returns>
		public async Task<QueryResult<Session>> QuerySession(SessionQueryArgument Arg)
		{
			var user = EnsureUserIdent();
			var q = Arg.Id != null ?
				//查询指定会话，必须是会话成员
				from s in DataContext.Set<DataModels.DataSession>().AsQueryable()
				where s.Id == Arg.Id.Id &&
						s.LogicState == EntityLogicState.Enabled
				let sm = s.Members.FirstOrDefault(m => m.OwnerId.Value == user)
				where sm != null
				select new { s, sm}
			:
				//查询所有自己参加的会话
				from sm in DataContext.Set<DataModels.DataSessionMember>().AsQueryable()
					where sm.OwnerId == user && 
							sm.LogicState == EntityLogicState.Enabled
				let s=sm.Session
				where s.LogicState==EntityLogicState.Enabled
				orderby s.LastActiveTime descending
				select new { s, sm}
				;
			var rq = from i in q
					 let s = i.s
					 let sm = i.sm
					 let lm = s.LastMessage
					 let lmm = lm.Poster
					 select new Session
					 {
						 Id = s.Id,
						 Name = s.Name,
						 Icon = s.Icon,
						 Flags=s.Flags,
						 SessionBizIdent=s.BizIdent,
						 SessionBizIdentType=s.BizIdentType,
						 MemberBizIdent=sm.BizIdent,
						 MemberBizIdentType=sm.BizIdentType,
						 MemberBizType=sm.BizType,
						 MemberCount = s.MemberCount,
						 MessageCount = s.MessageCount,
						 LastActiveTime = s.LastActiveTime,
						 LastReadTime = sm.LastReadTime,
						 MessageUnreaded=s.MessageCount-sm.MessageReaded,
						 OwnerMemberId = s.OwnerMemberId,
						 OwnerMemberIcon = s.OwnerMember.Icon,
						 OwnerMemberName = s.OwnerMember.Name,
						 JoinState=sm.JoinState,
						 LastMessage = lm == null|| sm.JoinState!=SessionJoinState.Joined ? null : new SessionMessage
						 {
							 Argument = lm.Argument,
							 SessionId = lm.SessionId,
							 Text = lm.Text,
							 Id = lm.Id,
							 Time = lm.Time,
							 Type = lm.Type,
							 PosterId = lm.PosterId,
							 PosterIcon = lmm == null ? null : lmm.Icon,
							 PosterName = lmm == null ? null : lmm.Name
						 }
					 };
			var re = await rq.ToQueryResultAsync(Arg.Paging);
			return re;
		}


		

		async Task RemoveMember(DataModels.DataSessionMember member)
		{
			if (member.MessageCount > 0)
				await SessionMemberManager.Value.UpdateEntity(
					ObjectKey.From(member.Id),
					m => m.LogicState = EntityLogicState.Deleted
					);
			else
				await SessionMemberManager.Value.RemoveAsync(ObjectKey.From(member.Id));
		}
		/// <summary>
		/// 将某人从自己的会话中移除
		/// </summary>
		/// <param name="MemberId">成员ID</param>
		/// <returns></returns>
		public async Task RemoveMember(long MemberId)
		{
			var user = EnsureUserIdent();
			var member = await (
				from m in DataContext.Set<DataModels.DataSessionMember>().AsQueryable()
				where m.Id == MemberId && m.Session.OwnerId == user && m.LogicState==EntityLogicState.Enabled
				select m
				).SingleOrDefaultAsync();
			if (member == null)
				throw new PublicArgumentException("找不到指定的成员");

			await RemoveMember(member);
		}

		/// <summary>
		/// 离开某个已加入的会话
		/// </summary>
		/// <param name="SessionId">会话ID</param>
		/// <returns></returns>
		public async Task LeaveSession(long SessionId)
		{
			var user = EnsureUserIdent();
			var member = await EnsureSessionMember(SessionId, user);
			await RemoveMember(member);
		}

		/// <summary>
		/// 发送会话消息
		/// </summary>
		/// <param name="Arg">消息发送参数</param>
		/// <returns>消息ID</returns>
		public async Task<ObjectKey<long>> SendMessage(MessageSendArgument Arg)
		{
			var user = EnsureUserIdent();

			var memberId = await DataContext.Set<DataModels.DataSessionMember>()
				.AsQueryable()
				.Where(m =>
					m.OwnerId == user &&
					m.SessionId == Arg.SessionId &&
					m.LogicState == EntityLogicState.Enabled
					)
				.Select(m => m.Id)
				.SingleOrDefaultAsync();
			if(memberId==0)
				throw new PublicDeniedException($"您已离开此会话");

			var id=await SessionMessageManager.Value.CreateAsync(
				new Models.SessionMessage
				{
					Argument = Arg.Argument,
					SessionId = Arg.SessionId,
					Text = Arg.Text,
					UserId=user,
					PosterId = memberId,
					Type = Arg.Type,
				});
			return id;
		}

		public async Task UpdateMember(SessionMember Member)
		{
			var user = EnsureUserIdent();
			var editable = await SessionMemberManager.Value.LoadForEdit(ObjectKey.From(Member.Id));
			if (editable == null)
				throw new PublicArgumentException("找不到指定的成员");

			//只有会话成员用户或会话所有人能修改成员信息
			if (editable.OwnerId != user)
			{
				if (!await DataContext
					.Set<DataModels.DataSession>()
					.AsQueryable()
					.AnyAsync(c => c.Id == editable.SessionId && c.OwnerId.Value == user))
					throw new PublicDeniedException("您不能修改此成员信息");
			}

			editable.Name = Member.Name;
			editable.Icon = Member.Icon;
			await SessionMemberManager.Value.UpdateAsync(editable);
		}


		public async Task UpdateSession(Session Session)
		{
			var user = EnsureUserIdent();
			var c = await SessionManager.Value.LoadForEdit(ObjectKey.From(Session.Id));
			if (c == null)
				throw new PublicArgumentException("找不到会话:"+Session.Id);
			if (c.OwnerId.Value != user)
				throw new PublicDeniedException("只能修改自己的会话信息");
			c.Name = Session.Name;
			c.Icon = Session.Icon;
			await SessionManager.Value.UpdateAsync(c);
		}

		public async Task SetReadTime(long SessionId)
		{
			var user = EnsureUserIdent();
			var mid = await SessionMemberManager.Value.QuerySingleEntityIdent(new Managers.SessionMemberQueryArgument
			{
				SessionId = SessionId,
				OwnerId = user
			});
			if (mid == null)
				throw new PublicDeniedException("你已不在指定的会话中");
			var member=await SessionMemberManager.Value.LoadForEdit(mid);
			member.LastReadTime = TimeService.Value.Now;
			await SessionMemberManager.Value.UpdateAsync(member);
		}

		public async Task SetAcceptType(long MemberId, bool AcceptType)
		{
			var uid = EnsureUserIdent();
			var m = await SessionMemberManager.Value.LoadForEdit(ObjectKey.From(MemberId));
			if (uid == m.OwnerId)
			{
				m.MemberAccepted = AcceptType;
				await SessionMemberManager.Value.UpdateAsync(m);
			}
			else
			{
				var soid = await DataContext.Set<DataModels.DataSession>().AsQueryable().Where(s => s.Id == m.SessionId).Select(s => s.OwnerId).SingleOrDefaultAsync();
				if (uid != soid)
					throw new PublicDeniedException("当前用户必须是会话所有者或成员所有者");
				m.SessionAccepted = AcceptType;
				await SessionMemberManager.Value.UpdateAsync(m);

			}
		}
	}
}