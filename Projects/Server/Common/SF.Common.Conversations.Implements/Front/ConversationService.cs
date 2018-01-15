
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
		Lazy<IEnumerable<Providers.ISessionProvider>> SessionProviders { get; }

		public long EnsureUserIdent() =>
			AccessToken.User.EnsureUserIdent();


		public ConversationService(
			IDataContext DataContext, 
			IAccessToken AccessToken,
			Lazy<Managers.ISessionManager> SessionManager,
			Lazy<Managers.ISessionMessageManager> SessionMessageManager,
			Lazy<Managers.ISessionMemberManager> SessionMemberManager,
			Lazy<ITimeService> TimeService,
			Lazy<IUserProfileService> UserProfileService, 
			Lazy<IEnumerable<Providers.ISessionProvider>> SessionProviders
			)
		{
			this.AccessToken = AccessToken;
			this.DataContext = DataContext;
			this.SessionManager= SessionManager;
			this.SessionMemberManager = SessionMemberManager;
			this.SessionMessageManager = SessionMessageManager;
			this.TimeService = TimeService;
			this.UserProfileService = UserProfileService;
			this.SessionProviders = SessionProviders;
		}

		
		async Task<(long BizIdent,Providers.ISessionProvider Provider,long UserId)> EnsureSessionMember(
			long SessionId
			)
		{
			var user = EnsureUserIdent();

			var sess = await DataContext
				.Set<DataModels.DataSession>()
				.AsQueryable()
				.Where(s =>
					s.Id == SessionId &&
					s.LogicState == EntityLogicState.Enabled
					)
				.Select(s=>new { s.BizIdentType,s.BizIdent})
				.SingleOrDefaultAsync();
			if (sess == null)
				throw new PublicArgumentException("找不到会话");

			var provider = SessionProviders.Value.Single(p => p.IdentType == sess.BizIdentType);
			if(provider==null)
				throw new PublicDeniedException("找不到会话类型："+sess.BizIdentType);
			await provider.MemberRelationValidate(sess.BizIdent, user);
			return (sess.BizIdent,provider, user);
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

			var ctx=await EnsureSessionMember(Arg.SessionId);

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
						UserId=m.UserId,
						PosterId = m.PosterId,
					};
			var re = await rq.ToQueryResultAsync(Arg.Paging);
			var uids = re.Items.Select(i => i.UserId).Where(i => i.HasValue).Select(i=>i.Value).ToArray();
			var users = await ctx.Provider.GetMemberDesc(ctx.BizIdent, uids);

			foreach(var i in re.Items)
				if (i.UserId.HasValue && users.TryGetValue(i.UserId.Value, out var u))
				{
					i.PosterName = u.Name;
					i.PosterIcon = u.Icon;
				}
			return re;
		}


		/// <summary>
		/// 发送会话消息
		/// </summary>
		/// <param name="Arg">消息发送参数</param>
		/// <returns>消息ID</returns>
		public async Task<ObjectKey<long>> SendMessage(MessageSendArgument Arg)
		{
			var ctx=await EnsureSessionMember(Arg.SessionId);

			var id =await SessionMessageManager.Value.CreateAsync(
				new Models.SessionMessage
				{
					Argument = Arg.Argument,
					SessionId = Arg.SessionId,
					Text = Arg.Text,
					UserId=ctx.UserId,
					Type = Arg.Type,
				});
			return id;
		}

		public async Task SetReadTime(long SessionId)
		{
			var ctx = await EnsureSessionMember(SessionId);
			await SessionMemberManager.Value.SetReadTime(SessionId, ctx.UserId);
		}

	}
}