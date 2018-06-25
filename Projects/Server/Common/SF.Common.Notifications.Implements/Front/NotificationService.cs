using SF.Common.Notifications.Management;
using SF.Sys;
using SF.Sys.Auth;
using SF.Sys.Clients;
using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys.Linq;
using SF.Sys.Services;
using SF.Sys.TimeServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Common.Notifications.Front
{
	//public static class MsgServiceTypes
	//   {
	//       public static readonly string 邮件 = "邮件";
	//       public static readonly string APP推送 = "APP推送";
	//       public static readonly string 验证短信 = "验证短信";
	//       public static readonly string 通知短信 = "通知短信";
	//       public static readonly string 促销短信 = "促销短信";
	//       public static readonly string 微信通知 = "微信通知";
	//   }
	//   [RequireServiceType(typeof(IMsgProvider), "邮件", "APP推送","验证短信","通知短信","促销短信", "微信通知")]
	public class NotificationService : INotificationService
	{
		//public NotificationServiceSetting Setting { get; }
		//public TypedInstanceResolver<IMsgProvider> MsgProviderResolver { get; }
		//public IMsgArgumentFactory ArgumentFactory { get; }
		//public IMsgLogger Logger { get; }
		//public MsgService(
		//	NotificationServiceSetting Setting, 
		//	IMsgArgumentFactory ArgumentFactory,
		//	TypedInstanceResolver<IMsgProvider> MsgProviderResolver, 
		//	IMsgLogger Logger
		//	)
		//{
		//	this.Setting = Setting;
		//	this.MsgProviderResolver = MsgProviderResolver;
		//	this.ArgumentFactory = ArgumentFactory;
		//	this.Logger = Logger;
		//}

		//bool IsDisabled()
		//{
		//          return Setting.Disabled;

		//}
		//public async Task<long> Send(long? targetId, Message message)
		//{
		//	if (IsDisabled())
		//		return 0;
		//	return await Logger.Add(
		//		targetId,
		//		message,
		//		async (al) =>
		//		{
		//			var re = await ArgumentFactory.Create(targetId, message);

		//			List<Exception> Errors = null;
		//			foreach (var a in re.Args)
		//			{
		//				try
		//				{
		//					await al.Add(a, async () =>
		//					{
		//						var p = MsgProviderResolver(a.MsgProviderId);
		//						if (string.IsNullOrEmpty(a.Target))
		//						{
		//							if (!a.TargetId.HasValue || string.IsNullOrEmpty(a.Target = await p.TargetResolve(a.TargetId.Value)))
		//								throw new ArgumentException($"消息未指定发送目标：Provider:{a.MsgProviderId} {message.ToString()}");
		//						}
		//						return await p.Send(a);
		//					});
		//				}
		//				catch (Exception ex)
		//				{
		//					if (Errors == null)
		//						Errors = new List<Exception>();
		//					Errors.Add(ex);
		//				}
		//			}
		//			if (Errors != null)
		//			{
		//				if (Errors.Count == 1)
		//					throw Errors[0];
		//				else
		//					throw new AggregateException(Errors.ToArray());
		//			}
		//			return re.PolicyId;
		//		}
		//	);

		//}

		Lazy<INotificationManager> NotificationManager { get; }
		Lazy<IDataScope> DataScope { get; }
		IAccessToken AccessToken { get; }
		Lazy<ITimeService> TimeService { get; }
		public long EnsureUserIdent() =>
			AccessToken.User.EnsureUserIdent();

		public NotificationService(
			Lazy<INotificationManager> NotificationManager,
			IAccessToken AccessToken,
			Lazy<IDataScope> DataScope,
			Lazy<ITimeService> TimeService
			)
		{
			this.TimeService = TimeService;
			this.NotificationManager = NotificationManager;
			this.AccessToken = AccessToken;
			this.DataScope = DataScope;
		}
		public Task Delete(long NotificationId)
		{
			return DataScope.Value.Use("删除通知", async DataContext =>
			 {
				 var uid = EnsureUserIdent();
				 var now = TimeService.Value.Now;

				 var re = await (
					 from n in DataContext.Set<DataModels.DataNotification>().AsQueryable()
					 where n.Id == NotificationId
					 let t = n.Targets.FirstOrDefault(t => t.UserId == uid)
					 where
						 //全局通知，可以没有通知对象，或通知对象没有删除
						 n.Mode == NotificationMode.Boardcast &&
							 (t == null || t != null && t.LogicState != EntityLogicState.Deleted)

						 //普通通知，必须有通知对象，且通知对象没有删除
						 || n.Mode == NotificationMode.Normal &&
							 t != null && t.LogicState != EntityLogicState.Deleted

					 select new
					 {
						 mode = n.Mode,
						 time = n.Time,
						 target = t
					 }).SingleOrDefaultAsync();
				 if (re == null)
					 return;

				//如果是普通通知，需要调整用户通知计数
				var status = re.mode == NotificationMode.Normal ?
					 await DataContext.Set<DataModels.DataNotificationUserStatus>().FindAsync(uid) :
					 null;

				 if (re.target == null)
				 {
					//如果是全局通知，需要增加删除记录
					if (re.mode == NotificationMode.Boardcast)
						 DataContext.Add(new DataModels.DataNotificationTarget
						 {
							 UserId = uid,
							 NotificationId = NotificationId,
							 LogicState = EntityLogicState.Deleted,
							 Time = re.time,
							 ReadTime = now
						 });
				 }
				//如果没有删除
				else if (re.target.LogicState != EntityLogicState.Deleted)
				 {
					 if (status != null)
					 {
						 status.Received = Math.Max(0, status.Received - 1);
						 if (re.target.ReadTime == null)
							 status.ReceivedUnreaded = Math.Max(0, status.ReceivedUnreaded - 1);
						 DataContext.Update(status);
					 }

					 if (!re.target.ReadTime.HasValue)
						 re.target.ReadTime = now;

					 re.target.LogicState = EntityLogicState.Deleted;
					 DataContext.Update(re.target);
				 }

				 await DataContext.SaveChangesAsync();
			 });
		}

		IQueryable<UserNotification> Select(
			IQueryable<DataModels.DataNotification> q,long user,bool WithContent)
		{
			return q.Select(n => new UserNotification
			{
				BizIdent=n.BizIdent,
				Content= WithContent?n.Content:null,
				HasBody=n.Content!=null && n.Content.Length>0,
				Id=n.Id,
				Image=n.Image,
				Link=n.Link,
				Mode=n.Mode,
				SenderId=n.SenderId,
				Time=n.Time,
				Name=n.Name,
				ReadTime=n.Targets.Where(t=>t.UserId==user).Select(t=>t.ReadTime).FirstOrDefault()
			}).OrderByDescending(n=>n.Time);
		}
		public async Task<UserNotification> Get(long Id)
		{
			var now = TimeService.Value.Now;
			var uid = EnsureUserIdent();

			var re = await DataScope.Value.Use("获取通知",
				DataContext =>
				{
					var q = from n in DataContext.Set<DataModels.DataNotification>().AsQueryable()
							where n.Id == Id &&
								n.Time <= now &&
								n.Expires > now &&
								n.LogicState == EntityLogicState.Enabled
							join ti in DataContext.Queryable<DataModels.DataNotificationTarget>() on n.Id equals ti.NotificationId into tt
							from t in tt.DefaultIfEmpty()
							where n.Mode == NotificationMode.Boardcast && (t==null || t.LogicState==EntityLogicState.Enabled) ||
								n.Mode==NotificationMode.Normal && t!=null && t.LogicState==EntityLogicState.Enabled
							select n;
					return Select(q, uid, true).SingleOrDefaultAsync();
				});
			return re;
		}

		public Task<UserNotificationStatus> GetStatus()
		{
			var uid = EnsureUserIdent();
			return DataScope.Value.Use("获取通知状态", async DataContext =>
			{
				var re = await DataContext.Set<DataModels.DataNotificationUserStatus>().AsQueryable()
				   .Where(s => s.Id == uid)
				   .Select(s => new UserNotificationStatus
				   {
					   Id = s.Id,
					   Received = s.Received,
					   ReceivedUnreaded = s.ReceivedUnreaded
				   }).SingleOrDefaultAsync();
				if (re == null)
					re = new UserNotificationStatus { Id = uid };

				var now = TimeService.Value.Now;

				var readed = await (from nr in DataContext.Set<DataModels.DataNotificationTarget>().AsQueryable()
									where nr.UserId == uid && nr.ReadTime.HasValue && nr.LogicState==EntityLogicState.Enabled
									let n = nr.Notification
									where n.Mode == NotificationMode.Boardcast &&
											n.Time <= now && n.Expires > now &&
											n.LogicState == EntityLogicState.Enabled
									select 1).SumAsync();

				var count= await (from n in DataContext.Set<DataModels.DataNotification>().AsQueryable()
								  where n.Mode == NotificationMode.Boardcast &&
										  n.Time <= now && n.Expires > now &&
										  n.LogicState == EntityLogicState.Enabled
									join ti in DataContext.Queryable<DataModels.DataNotificationTarget>() on n.Id equals ti.NotificationId into tt
									from t in tt.DefaultIfEmpty()
									where t==null || t.LogicState==EntityLogicState.Enabled
								  select 1).SumAsync();
				re.Received += count;
				re.ReceivedUnreaded += count-readed;
				return re;
			});
		}

		public async Task<QueryResult<UserNotification>> Query(NotificationQueryArgument Arg)
		{
			if (!Arg.Mode.HasValue)
			{
				var broadcasts = (await Query(new NotificationQueryArgument {
					Mode = NotificationMode.Boardcast,
					Readed = false
				})).Items.ToArray();
				Arg.Mode = NotificationMode.Normal;
				var normals =await Query(Arg);
				if (broadcasts.Length > 0)
					normals.Items = broadcasts.Concat(normals.Items);
				return normals;
			}

			var uid = EnsureUserIdent();
			var now = TimeService.Value.Now;

			return await DataScope.Value.Use("查询通知", async DataContext =>
			 {
				 IQueryable<DataModels.DataNotification> q;
				 if (Arg.Mode == NotificationMode.Boardcast)
				 {
					 q = from n in DataContext.Set<DataModels.DataNotification>().AsQueryable()
						 where n.Mode == NotificationMode.Boardcast &&
						 n.LogicState == EntityLogicState.Enabled &&
							 n.Time <= now &&
							 n.Expires > now
						 select n;
					 if (Arg.Readed.HasValue)
					 {
						 if (Arg.Readed.Value)
							 q = q.Where(n => n.Targets.Any(t => t.UserId == uid && t.ReadTime.HasValue));
						 else
							 q = q.Where(n => n.Targets.All(t => t.UserId != uid || !t.ReadTime.HasValue));
					 }
				 }
				 else
				 {
					 var readed = Arg.Readed;
					 q = from nt in DataContext.Set<DataModels.DataNotificationTarget>().AsQueryable()
						 where
							 nt.Mode == NotificationMode.Normal &&
							 nt.UserId == uid &&
							 nt.LogicState == EntityLogicState.Enabled &&
							 (!readed.HasValue ||
							 readed.HasValue && nt.ReadTime.HasValue == readed.Value
							 )
						 let n = nt.Notification
						 where n.LogicState == EntityLogicState.Enabled &&
							 n.Time <= now //&& n.Expires > now 个人消息不会超时
						 select n;
				 }
				 var re = await Select(q, uid, false)
					 .ToQueryResultAsync(Arg.Paging);
				 return re;
			 });
		}

		public Task SetReaded(SetReadedArgument Arg)
		{
			var uid = EnsureUserIdent();
			var ids = Arg.NotificationIds;
			if (ids == null || ids.Length == 0)
				return Task.CompletedTask;

			return DataScope.Value.Retry("设置已读状态", async DataContext =>
			 {
				 var now = TimeService.Value.Now;
				 var notifications = await (
					 from n in DataContext.Set<DataModels.DataNotification>().AsQueryable(true)
					 where ids.Contains(n.Id)
					 select new
					 {
						 id = n.Id,
						 mode = n.Mode,
						 time = n.Time,
						 target = n.Targets.FirstOrDefault(nt => nt.UserId == uid && !nt.ReadTime.HasValue)
					 }
					 )
					 .ToArrayAsync();

				//更新找到对象的通知记录统计
				var status = await DataContext.Set<DataModels.DataNotificationUserStatus>().FindAsync(uid);
				 var statusUpdated = false;
				 foreach (var nt in notifications)
				 {
					 if (nt.target != null)
					 {
						 nt.target.ReadTime = now;
						 DataContext.Update(nt.target);

						 if (nt.mode == NotificationMode.Normal &&
							 status != null &&
							 status.ReceivedUnreaded > 0)
						 {
							 status.ReceivedUnreaded--;
							 statusUpdated = true;
						 }
					 }
					 else if (nt.mode == NotificationMode.Boardcast)
						 DataContext.Add(new DataModels.DataNotificationTarget
						 {
							 LogicState = EntityLogicState.Enabled,
							 Mode = NotificationMode.Boardcast,
							 NotificationId = nt.id,
							 ReadTime = now,
							 Time = nt.time,
							 UserId = uid
						 });
				 }
				 if (statusUpdated)
					 DataContext.Update(status);
				 await DataContext.SaveChangesAsync();
				 return 0;
			 }
			);
		}
	}
}
