using SF.Sys;
using SF.Sys.Data;
using SF.Sys.Events;
using SF.Sys.Logging;
using SF.Sys.Reminders;
using SF.Sys.Services;
using SF.Sys.TimeServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SF.Biz.Payments
{
   

    /// <summary>
    /// 常规收款
    /// </summary>
    public class CollectService: 
        ICollectService,
        ICollectCallback,
        ICollectStartArgumentLoader        
    {
		public TypedInstanceResolver<ICollectProvider> CollectProviderResolver { get; }
		public ITimeService TimeService { get; }
		public Lazy<IRemindService> RemindService { get;}
		public Lazy<IEventEmitService> EventEmitService { get; }
        public ILogger Logger { get; }

        public IDataScope DataScope { get; }

        public Lazy<IIdentGenerator<DataModels.DataCollectRecord>> IdentGenerator { get; }

        public CollectService(
            TypedInstanceResolver<ICollectProvider> CollectProviderResolver,
            ITimeService TimeService,
			Lazy<IRemindService> RemindService,
			Lazy<IEventEmitService> EventEmitService,
            ILogger<CollectService> Logger,
            IDataScope DataScope,
            Lazy<IIdentGenerator<DataModels.DataCollectRecord>> IdentGenerator
            )
		{
			this.CollectProviderResolver = CollectProviderResolver;
            this.TimeService = TimeService;
			this.RemindService = RemindService;
			this.EventEmitService = EventEmitService;
            this.Logger = Logger;
            this.DataScope = DataScope;
            this.IdentGenerator = IdentGenerator;

        }

		public Task<long> Create(CollectRequest Argument)
		{
			if (CollectProviderResolver(Argument.PaymentPlatformId) == null)
				throw new ArgumentException("不支持指定的支付平台:" + Argument.PaymentPlatformId);

            return DataScope.Use("创建收款记录", async ctx =>
            {
                var id = await IdentGenerator.Value.GenerateAsync();
                ctx.Add(new DataModels.DataCollectRecord
                {
                    Id = id,
                    Title = Argument.Title,
                    Desc = Argument.Desc,
                    RemindId = Argument.RemindId,
                    CreatedTime = DateTime.Now,
                    Amount = Argument.Amount,
                    PaymentPlatformId = Argument.PaymentPlatformId,
                    ClientType = Argument.ClientType,
                    TrackEntityIdent = Argument.TrackEntityIdent,
                    UserId = Argument.CurUserId,
                    HttpRedirect = Argument.HttpRedirect,
                    State = CollectState.Init,
                    OpAddress = Argument.OpAddress,
                    OpDevice = Argument.OpDevice,
                    Time=TimeService.Now
                });
                await ctx.SaveChangesAsync();
                return id;
            });
		}

		public async Task<IReadOnlyDictionary<string, string>> Start(long Ident, StartRequestInfo RequestInfo)
		{
            try
            {
                var arg = await GetRequest(Ident);
                var provider = CollectProviderResolver(arg.PaymentPlatformId);

                
                var notifyUrl = new UriBuilder(new Uri(RequestInfo.Uri, "/api/CollectCallback/Notify/" + arg.PaymentPlatformId));
                //第三方支付平台可能不支持https方式异步通知，需要总是使用http方式。
                //notifyUrl.Scheme = "http";
                //notifyUrl.Port = 80;
                var re = await provider.Start(
                    Ident,
                    arg,
                    RequestInfo,
                    new Uri(RequestInfo.Uri, "/api/CollectCallback/Return/" + arg.PaymentPlatformId).ToString(),
                    notifyUrl.ToString()
                    );

                await DataScope.Use("开始收款", async ctx =>
                {
                    var r = await ctx.Set<DataModels.DataCollectRecord>().FindAsync(Ident);
                    if (r == null)
                        throw new ArgumentException("找不到收款支付记录");
                    r.StartTime = TimeService.Now;
                    r.ExtraData = re.ExtraData;
                    r.ExtIdent = re.ExtIdent;
                    r.State = CollectState.Collecting;
                    ctx.Update(r);
                    await ctx.SaveChangesAsync();
                });
                //await CollectStorage.Start(Ident, TimeService.Now, re.ExtIdent, re.ExtraData);
                //var redirect = re.Result.Get("redirect");
                //if(redirect!=null  && arg.ClientType== "desktop")
                //{
                //    re.Result["redirect"] = "/payment/status/" + Ident;
                //    re.Result["openWindow"] = redirect;
                //}
                return re.Result;
            }catch(Exception e)
            {
                Logger.Error(e, "第三方支付请求异常，请使用其他支付方式，或联系客服人员处理：" + e);
                throw new PublicInvalidOperationException("第三方支付请求异常，请使用其他支付方式，或联系客服人员处理", e);
            }
		}
		public async Task<string> GetStartProviderExtraData(long Ident)
		{
            return await DataScope.Use("获取提供者数据",ctx=>
                ctx.Queryable<DataModels.DataCollectRecord>()
                .Where(r => r.Id == Ident)
                .Select(r => r.ExtraData)
                .SingleOrDefaultAsync()
                  );
		}
        
		async Task<CollectResponse> TryComplete(long Ident, CollectRequest req, CollectResponse Response,ICollectProvider provider,bool Remind)
		{
            if (Response== null || !Response.CompletedTime.HasValue)
            {
                if (provider.CollectRequestTimeout.HasValue &&
                    req.StartTime.Value + provider.CollectRequestTimeout.Value + TimeSpan.FromMinutes(5) < TimeService.Now)
                    Response = new CollectResponse
                    {
                        Ident = Ident,
                        CompletedTime = TimeService.Now,
                        Error = "收款已过时"
                    };
                else
                    return null;
            }

            string desc;
			if (Response.Error == null)
			{
				var user = Response.ExtUserName ?? Response.ExtUserAccount ?? Response.ExtUserId;
				var platform = provider.Title;
				desc = $"充值{Response.AmountCollected}成功，来自{platform}的用户{user}";
			}
			else
				desc = "充值失败";
            

            return await DataScope.Use("保存收款应答", async ctx =>
            {
                var r = await ctx.Set<DataModels.DataCollectRecord>().FindAsync(Response.Ident);
                if (r == null)
                    throw new ArgumentException("找不到收款支付记录:" + Response.Ident);
                
                if(r.State==CollectState.Init)
                    throw new InvalidOperationException("收款操作未启动:" + Response.Ident);

                if (r.State==CollectState.Success || r.State==CollectState.Failed)
                {
                    var newState = Response.Error == null ? CollectState.Success : CollectState.Failed;
                    if(r.State!=newState)
                    {
                        Logger.Warn($"检测到收款结果变化: {Response.Ident} 旧状态:{r.State} 新状态:{newState}");
                    }
                    return null;
                }


                r.CompletedTime = Response.CompletedTime;
                r.ExtUserId = Response.ExtUserId;
                r.ExtUserName = Response.ExtUserName;
                r.ExtUserAccount = Response.ExtUserAccount;
                r.ExtIdent = Response.ExtIdent;
                r.AmountCollected = Response.AmountCollected;
                r.Error = Response.Error;
                r.State = Response.Error == null ? CollectState.Success : CollectState.Failed;
                ctx.Update(r);
                if (Remind)
                    ctx.AddCommitTracker(TransactionCommitNotifyType.AfterCommit, async (type, err) =>
                    {
                        await this.RemindService.Value.Remind(req.RemindId, null);
                    });

                await ctx.EmitEvent(EventEmitService.Value, new CollectComplete
                {
                    Id = Ident,
                    Amount = req.Amount,
                    AmountCompleted = Response.AmountCollected,
                    Name = desc,
                    UserId = req.CurUserId
                });
                
                await ctx.SaveChangesAsync();
                return Response;
            });

		}

		public async Task<CollectSession> GetResult(long Ident,bool Query=false,bool Remind=false )
		{
            var sess = await DataScope.Use("获取收款结果", ctx =>
                (from r in ctx.Queryable<DataModels.DataCollectRecord>()
                 where r.Id == Ident
                 select new CollectSession
                 {
                     Id = r.Id,
                     CreateTime = r.CreatedTime,
                     ExtraData = r.ExtraData,
                     Request = new CollectRequest
                     {
                         ClientType = r.ClientType,
                         HttpRedirect = r.HttpRedirect,
                         CurUserId = r.UserId.HasValue ? r.UserId.Value : 0,
                         TrackEntityIdent = r.TrackEntityIdent,
                         Amount = r.Amount,
                         Desc = r.Desc,
                         PaymentPlatformId = r.PaymentPlatformId,
                         Title = r.Title,
                         OpAddress = r.OpAddress,
                         OpDevice = r.OpDevice,
                         StartTime=r.StartTime,
                         RemindId=r.RemindId
                     },
                     Response = new CollectResponse
                     {
                         Ident = r.Id,
                         AmountCollected = r.AmountCollected,
                         Error = r.Error,
                         ExtIdent = r.ExtIdent,
                         ExtUserAccount = r.ExtUserAccount,
                         ExtUserId = r.ExtUserId,
                         ExtUserName = r.ExtUserName,
                         CompletedTime = r.CompletedTime
                     }
                 }).SingleOrDefaultAsync()
                 );
            if(!sess.Response.CompletedTime.HasValue && Query)
            {
                var pp = CollectProviderResolver(sess.Request.PaymentPlatformId);
                var re = await pp.GetResultByQuery(sess.Request);
                var req = (CollectRequest)sess.Request;
                var new_resp = await TryComplete(Ident, req, re, pp,Remind);
                sess.Response = new_resp;
            }
            return sess;
        }
        /// <summary>
        /// 第三方平台异步通知
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
		async Task<HttpResponseMessage> ICollectCallback.Notify(long Id)
		{
			var provider = CollectProviderResolver(Id);
			var re = await provider.GetResultByNotify(this);
			if (re.Session != null)
			{
                var req = (CollectRequest)re.Session.Request;
                var sess=await TryComplete(re.Session.Id, req, re.Session.Response,provider,true);
            }
            return re.HttpResponse;
		}

        /// <summary>
        /// 第三方平台跳回
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
		async Task<HttpResponseMessage> ICollectCallback.Return(long Id)
		{
            var provider = CollectProviderResolver(Id);
            var re = await provider.GetResultByCallback(this);
			if (re.Session != null)
			{
                var req = (CollectRequest)re.Session.Request;
                var sess=await TryComplete(re.Session.Id,req, re.Session.Response, provider,true);
            }
			return re.HttpResponse;
		}

        async Task<CollectStartArgument> ICollectStartArgumentLoader.GetRequest(long ident) =>
            await GetRequest(ident);
        
        public async Task<CollectRequest> GetRequest(long ident)
		{
            var req= await DataScope.Use("获取收款请求", ctx =>
                 ctx.Queryable<DataModels.DataCollectRecord>().Where(r => r.Id == ident)
                 .Select(r => new CollectRequest
                 {
                     Amount = r.Amount,
                     Desc = r.Desc,
                     TrackEntityIdent = r.TrackEntityIdent,
                     CurUserId = r.UserId.HasValue ? r.UserId.Value : 0,
                     PaymentPlatformId = r.PaymentPlatformId,
                     ClientType = r.ClientType,
                     HttpRedirect = r.HttpRedirect,
                     Title = r.Title,
                     RemindId = r.RemindId,
                     OpAddress = r.OpAddress,
                     OpDevice = r.OpDevice,
                     StartTime=r.StartTime.Value
                 })
                 .SingleOrDefaultAsync()
                );
            if (req == null)
                throw new ArgumentException("找不到收款支付记录");
            return req;

        }

		public async Task<QrCodePaymentStatus> GetQrCodePaymentStatus(long Ident)
		{
			var sess=await this.GetResult(Ident);
			if (sess == null)
				return null;
			var provider = CollectProviderResolver(sess.Request.PaymentPlatformId);
			var qrresolver = provider as ICollectQrCodeResolver;
			if (qrresolver == null)
				return null;
			if (sess .ExtraData== null)
				return null;
			var qrc = qrresolver.GetQrCode(sess.ExtraData);
			if (qrc == null)
				return null;

			return new QrCodePaymentStatus
			{
				QrCodeUri = qrc,
				Amount = sess.Request.Amount,
				Ident = Ident,
				PaymentPlatform = sess.Request.PaymentPlatformId,
				Title = sess.Request.Title,
				CreateTime = sess.CreateTime,
				CompletedTime = sess.Response.CompletedTime,
				HttpRedirect = sess.Request.HttpRedirect
			};
		}


    }


}
