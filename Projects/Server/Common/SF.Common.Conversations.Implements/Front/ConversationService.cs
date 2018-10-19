
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
using SF.Sys.Linq;

namespace SF.Common.Conversations.Front
{

	/// <summary>
	/// 会话服务
	/// </summary>
	public class ConversationService : IConversationService
	{
		IDataScope DataScope { get; }
		IAccessToken AccessToken { get; }
		Lazy<Managers.ISessionStatusManager> SessionStatusManager { get; }
		Lazy<Managers.ISessionMemberStatusManager> SessionMemberStatusManager { get; }
		Lazy<Managers.ISessionMessageManager> SessionMessageManager { get; }
		Lazy<ITimeService> TimeService { get; }
		Lazy<IUserProfileService> UserProfileService { get; }
		IEnumerable<Providers.ISessionProvider> SessionProviders { get; }
		SF.Sys.Services.NamedServiceResolver<Providers.ISessionProvider> SessionProviderResolver { get; }

		public long EnsureUserIdent() =>
			AccessToken.User.EnsureUserIdent();


		public ConversationService(
			IDataScope DataScope, 
			IAccessToken AccessToken,
			
			Lazy<Managers.ISessionStatusManager> SessionStatusManager,
			Lazy<Managers.ISessionMessageManager> SessionMessageManager,
			Lazy<Managers.ISessionMemberStatusManager> SessionMemberStatusManager,

			Lazy<ITimeService> TimeService,
			Lazy<IUserProfileService> UserProfileService, 
			IEnumerable<Providers.ISessionProvider> SessionProviders,
			SF.Sys.Services.NamedServiceResolver<Providers.ISessionProvider> SessionProviderResolver
			)
		{
			this.AccessToken = AccessToken;
			this.DataScope = DataScope;
			this.SessionStatusManager= SessionStatusManager;
			this.SessionMemberStatusManager = SessionMemberStatusManager;
			this.SessionMessageManager = SessionMessageManager;
			this.TimeService = TimeService;
			this.UserProfileService = UserProfileService;
			this.SessionProviders = SessionProviders;
			this.SessionProviderResolver = SessionProviderResolver;
		}

		
		async Task<(Providers.ISessionProvider Provider,long UserId,DateTime JoinTime)> EnsureSessionMember(
			string BizIdentType,
			long BizIdent
			)
		{
			if (BizIdentType.IsNullOrEmpty())
				throw new PublicArgumentException("必须指定业务标识类型");
			if (BizIdent == 0)
				throw new PublicArgumentException("必须指定业务标识");


			var user = EnsureUserIdent();
			var provider = SessionProviderResolver(BizIdentType);
			if(provider==null)
				throw new PublicDeniedException("找不到会话类型："+BizIdentType);
			var joinTime=await provider.MemberRelationValidate(BizIdent, user);
			return (provider, user, joinTime);
		}

		/// <summary>
		/// 查询会话消息
		/// </summary>
		/// <param name="Arg">查询参数</param>
		/// <returns></returns>
		public async Task<QueryResult<SessionMessage>> QueryMessages(MessageQueryArgument Arg)
		{
			var ctx =await EnsureSessionMember(Arg.BizIdentType,Arg.BizIdent);
			return await DataScope.Use("查询消息", async DataContext =>
			 {
				 var user = ctx.UserId;
				 var rq = from s in DataContext.Set<DataModels.DataSessionStatus>().AsQueryable()
						 where s.BizIdentType == Arg.BizIdentType &&
								 s.BizIdent == Arg.BizIdent &&
								 s.LogicState == EntityLogicState.Enabled
                          from m in s.Messages
                          where m.Time >= ctx.JoinTime

                         select new SessionMessage
						 {
							 Id = m.Id,
							 Argument = m.Argument,
							 SessionId = m.SessionId,
							 Text = m.Text,
							 Time = m.Time,
							 Type = m.Type,
							 UserId = m.UserId,
							 Self = m.UserId.HasValue && m.UserId.Value == user
						 };

				 if (Arg.StartId.HasValue)
					 rq = rq.Where(m => m.Id > Arg.StartId.Value).OrderBy(m => m.Id);
				 else if (Arg.EndId.HasValue)
					 rq = rq.Where(m => m.Id < Arg.EndId.Value).OrderByDescending(m => m.Id);
				 else
					 rq = rq.OrderByDescending(m => m.Id);

				 var re = await rq.ToQueryResultAsync(Arg.Paging);

				 var uids = re.Items.Select(i => i.UserId).Where(i => i.HasValue).Select(i => i.Value).ToArray();
				 var users = await ctx.Provider.GetMemberDesc(Arg.BizIdent, uids);

				 foreach (var i in re.Items)
					 if (i.UserId.HasValue && users.TryGetValue(i.UserId.Value, out var u))
					 {
						 i.PosterName = u.Name;
						 i.PosterIcon = u.Icon;
						 i.MemberBizIdent = u.BizIdent;
						 i.MemberBizIdentType = u.BizIdentType;
					 }

				 if (!Arg.StartId.HasValue)
					 re.Items = re.Items.OrderBy(i => i.Id).ToArray();

				 return re;
			 });
		}


		/// <summary>
		/// 发送会话消息
		/// </summary>
		/// <param name="Arg">消息发送参数</param>
		/// <returns>消息ID</returns>
		public async Task<ObjectKey<long>> SendMessage(MessageSendArgument Arg)
		{
			var ctx = await EnsureSessionMember(Arg.BizIdentType, Arg.BizIdent);
			var sid = await SessionStatusManager.Value.GetOrCreateSession(Arg.BizIdentType, Arg.BizIdent);

			var id =await SessionMessageManager.Value.CreateAsync(
				new Models.SessionMessage
				{
					SessionId= sid,
					Argument = Arg.Argument,
					Text = Arg.Text,
					UserId=ctx.UserId,
					Type = Arg.Type,
				});
			return id;
		}

		public async Task SetReadTime(SetReadTimeArgument Arg)
		{
			var ctx=await EnsureSessionMember(Arg.BizIdentType, Arg.BizIdent);
			var sid = await SessionStatusManager.Value.GetOrCreateSession(Arg.BizIdentType, Arg.BizIdent);

			await SessionMemberStatusManager.Value.SetReadTime(
				sid,
				ctx.UserId
				);
		}

		public async Task<QueryResult<SessionGroup>> QuerySessions(SessionQueryArgument Arg)
		{
			var user = EnsureUserIdent();
			var grps = new List<SessionGroup>();
			foreach (var sp in SessionProviders)
			{
				grps.AddRange(await sp.QuerySessions(user));
			}
			return await DataScope.Use("查询消息", async DataContext =>
			{

				var ssQuery = DataContext.Set<DataModels.DataSessionStatus>().AsQueryable();
				var Members = DataContext.Set<DataModels.DataSessionMemberStatus>().AsQueryable();

				//填充会话状态
				foreach (var g in from grp in grps
								  from s in grp.Sessions
								  group s by s.BizIdentType into g
								  select (
									  BizIdentType: g.Key,
									  BizIdents: g.Select(gi => gi.BizIdent).Distinct().ToArray(),
									  Sessions: g.ToArray()
									  )
								)
				{
					var dics = await (
						from s in ssQuery
						where s.BizIdentType == g.BizIdentType && g.BizIdents.Contains(s.BizIdent)
						join tm in Members on s.Id equals tm.SessionId into tms
						from m in tms.Where(tm => tm.OwnerId.Value == user).DefaultIfEmpty()
							//let m= s.Members.Where(m => m.OwnerId.Value == user).SingleOrDefault()
						let readed = m == null ? 0 : m.MessageReaded
						select new
						{
							s.BizIdent,
							s.BizIdentType,
							s.LogicState,
							Unread = s.MessageCount - readed,
							s.UpdatedTime,
							s.LastMessageText
						}).ToDictionaryAsync(s => (s.BizIdentType, s.BizIdent));

					foreach (var s in g.Sessions)
						if (dics.TryGetValue((s.BizIdentType, s.BizIdent), out var ss))
						{
							s.Unread = ss.Unread;

                            //只有加入时间比消息时间早，才显示消息
                            if (s.JoinTime <= ss.UpdatedTime)
                            {
                                s.Text = s.Text ?? ss.LastMessageText;
                                s.Time = s.Time ?? ss.UpdatedTime;
                            }
						}
				}


				var q = from g in grps
						from s in g.Sessions
						group (g, s) by g.Name into gi
						let fg = gi.First().g
						orderby fg.Order
						select new SessionGroup
						{
							Name = fg.Name,
							Text = fg.Text,
							Sessions = gi.Select(p => p.s)
									  .OrderByDescending(s => s.Time)
									  .ToArray()
						};
				var items = q.ToArray();
				return new QueryResult<SessionGroup>
				{
					Items = items
				};
			});
		}
		public async Task<QueryResult<SessionMember>> QueryMembers(MemberQueryArgument Arg)
		{
			var ctx = await EnsureSessionMember(Arg.BizIdentType, Arg.BizIdent);
			var items = await ctx.Provider.QueryMembers(Arg.BizIdent);

			return new QueryResult<SessionMember>
			{
				Items = items
			};

		}
	}
}