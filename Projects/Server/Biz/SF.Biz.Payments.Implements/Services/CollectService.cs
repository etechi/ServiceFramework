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
    public class CollectExpired :
          SF.Sys.Reminders.IRemindable
    {
        ICollectService CollectService { get; }
        public CollectExpired(ICollectService CollectService)
        {
            this.CollectService = CollectService;
        }
        
        public async Task Remind(IRemindContext Context)
        {
            await CollectService.LastTryCompleteByQuery(Context.BizIdent);
        }
    }

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

        public Lazy<IIdentGenerator <CollectService>> IdentGenerator { get; }

        public CollectService(
            TypedInstanceResolver<ICollectProvider> CollectProviderResolver,
            ITimeService TimeService,
			Lazy<IRemindService> RemindService,
			Lazy<IEventEmitService> EventEmitService,
            ILogger<CollectService> Logger,
            IDataScope DataScope,
            Lazy<IIdentGenerator<CollectService>> IdentGenerator
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
                    CallbackName = Argument.CallbackName,
                    CallbackContext = Argument.CallbackContext,
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
                });
                await ctx.SaveChangesAsync();
                return id;
            });
		}

		public async Task<IDictionary<string, string>> Start(long Ident, StartRequestInfo RequestInfo)
		{
            try
            {
                var arg = await GetRequest(Ident);
                var provider = CollectProviderResolver(arg.PaymentPlatformId);

                var timeout = provider.CollectRequestTimeout;
                if (timeout != null)
                {
                    var RealTimeout = timeout.Value.Add(TimeSpan.FromMinutes(5));

                    //注册定时器，在收款超时时，查询收款状态
                    await this.RemindService.Value.Setup(new RemindSetupArgument
                    {
                        BizIdent = Ident,
                        BizIdentType = "CollectRecord",
                        BizType = "收款超时",
                        Name = arg.Title,
                        RemindableName = typeof(CollectExpired).FullName,
                        //RemindData = Ident,
                        RemindTime = TimeService.Now.Add(RealTimeout),
                        UserId = arg.CurUserId
                    });
                }
                
                var notifyUrl = new UriBuilder(new Uri(RequestInfo.Uri, "/api/CollectCallback/Notify/" + arg.PaymentPlatformId));
                //第三方支付平台可能不支持https方式异步通知，需要总是使用http方式。
                //notifyUrl.Scheme = "http";
                //notifyUrl.Port = 80;
                var re = await provider.Start(
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
        
		async Task<CollectSession> TryComplete(long Ident,CollectRequest req, CollectResponse Response,bool RemoveExpireTimer)
		{
			string desc;
			if (Response.Error == null)
			{
				var user = Response.ExtUserName ?? Response.ExtUserAccount ?? Response.ExtUserId;
				var provider = CollectProviderResolver(req.PaymentPlatformId);
				var platform = provider.Name;
				desc = $"充值{Response.AmountCollected}成功，来自{platform}的用户{user}";
			}
			else
				desc = "充值失败";
            var remider=await this.RemindService.Value.Setup(new RemindSetupArgument
            {
                BizIdent = Ident,
                BizIdentType = "CollectRecord",
                BizType = "收款结束",
                Name = desc,
                RemindableName = req.CallbackName,
                RemindData = req.CallbackContext,
                RemindTime = TimeService.Now,
                UserId = req.CurUserId
            });

            await DataScope.Use("保存收款应答", async ctx =>
            {
                var r = await ctx.Set<DataModels.DataCollectRecord>().FindAsync(Response.Ident);
                if (r == null)
                    throw new ArgumentException("找不到收款支付记录:" + Response.Ident);
                r.CompletedTime = Response.CompletedTime;
                r.ExtUserId = Response.ExtUserId;
                r.ExtUserName = Response.ExtUserName;
                r.ExtUserAccount = Response.ExtUserAccount;
                r.ExtIdent = Response.ExtIdent;
                r.AmountCollected = Response.AmountCollected;
                r.Error = Response.Error;
                r.State = Response.Error == null ? CollectState.Success : CollectState.Failed;
                ctx.Update(r);
                await ctx.SaveChangesAsync();
            });

            await this.RemindService.Value.Remind(remider,null);
            if (RemoveExpireTimer)
                await this.RemindService.Value.Remove("收款超时", typeof(CollectExpired).FullName, Ident);

            var e = await this.EventEmitService.Value.Create(new CollectComplete
            {
                Id=Ident,
                Amount=req.Amount,
                AmountCompleted=Response.AmountCollected,
                Name=desc,
                UserId=req.CurUserId
            });
            await e.Commit();
                //("支付", "收款完成", Ident, false);
			return new CollectSession { Request = req, Response = Response };
		}

		public async Task<CollectSession> GetResult(long Ident)
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
                         OpDevice = r.OpDevice
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
            return sess;
        }

		public async Task<HttpResponseMessage> Notify(long Id)
		{
			var provider = CollectProviderResolver(Id);
			var re = await provider.GetResultByNotify(this);
			if (re.Session != null)
			{
				await TryComplete(re.Session.Id, (CollectRequest)re.Session.Request, re.Session.Response,true);
			}
			return re.HttpResponse;
		}

		public async Task<HttpResponseMessage> Return(long Id)
		{
            var provider = CollectProviderResolver(Id);
            var re = await provider.GetResultByCallback(this);
			if (re.Session != null)
			{
				await TryComplete(re.Session.Id,(CollectRequest)re.Session.Request, re.Session.Response, true);
			}
			return re.HttpResponse;
		}

		public async Task<CollectSession> TryCompleteByQuery(long Ident)
		{
			var req = await GetRequest(Ident);
			var pp = CollectProviderResolver(req.PaymentPlatformId);
			var re = await pp.GetResultByQuery(req);
			if (re == null || !re.CompletedTime.HasValue) return null;
			return await TryComplete(Ident, req, re, true);
		}
        public async Task<CollectSession> LastTryCompleteByQuery(long Ident)
        {
            var req = await GetRequest(Ident);
            var pp = CollectProviderResolver(req.PaymentPlatformId);
            var re = await pp.GetResultByQuery(req);
            if (re == null || !re.CompletedTime.HasValue)
                re = new CollectResponse
                {
                    Ident = Ident,
                    CompletedTime = TimeService.Now,
                    Error = "收款已过时"
                };
            return await TryComplete(Ident,req, re,false);
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
                     CallbackName = r.CallbackName,
                     CallbackContext = r.CallbackContext,
                     OpAddress = r.OpAddress,
                     OpDevice = r.OpDevice
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
