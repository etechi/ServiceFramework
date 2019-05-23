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


    public class RefundService: 
        IRefundService

    {
        public TypedInstanceResolver<ICollectProvider> CollectProviderResolver { get; }
        public ITimeService TimeService { get; }
        public Lazy<IRemindService> RemindService { get; }
        public Lazy<IEventEmitService> EventEmitService { get; }
        public ILogger Logger { get; }

        public IDataScope DataScope { get; }

        public Lazy<IIdentGenerator<DataModels.DataRefundRecord>> IdentGenerator { get; }
        public RefundService(
            TypedInstanceResolver<ICollectProvider> CollectProviderResolver,
            ITimeService TimeService,
            Lazy<IRemindService> RemindService,
            Lazy<IEventEmitService> EventEmitService,
            ILogger<CollectService> Logger,
            IDataScope DataScope,
            Lazy<IIdentGenerator<DataModels.DataRefundRecord>> IdentGenerator
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

        public async Task<RefundRefreshResult> Create(RefundRequest Argument)
        {
            var provider = CollectProviderResolver(Argument.PaymentPlatformId);
            if (provider == null)
                throw new ArgumentException("不支持指定的支付平台:" + Argument.PaymentPlatformId);


            return await DataScope.Use("创建退款记录", async ctx =>
            {
                 var collectState = await ctx.Queryable<DataModels.DataCollectRecord>()
                     .Where(r => r.Id == Argument.CollectIdent)
                     .Select(r => new
                     {
                         Amount = r.Amount,
                         state = r.State,
                         platformId = r.PaymentPlatformId,
                         extIdent = r.ExtIdent
                     })
                     .SingleOrDefaultAsync();
                 if (collectState == null)
                     throw new ArgumentException("找不到收款记录：" + Argument.CollectIdent);

                 if (collectState.state != CollectState.Success)
                     throw new ArgumentException("未成功的收款不能退款:" + Argument.CollectIdent);

                 var existsRefunds = await ctx.Queryable<DataModels.DataRefundRecord>()
                     .Where(r => r.CollectIdent == Argument.CollectIdent)
                     .Select(r => new { amount = r.Amount, id = r.Id })
                     .ToArrayAsync();
                 if (existsRefunds.Length > 0)
                 {
                    //if (existsRefunds.Any(r => r.id == Argument.Ident))
                    //    throw new ArgumentException("退款记录已存在:" + Argument.Ident);


                    if (existsRefunds.Sum(r => r.amount) >= collectState.Amount)
                         throw new PublicArgumentException("退款金额超过限制");
                 }
                 var time = TimeService.Now;
                 var ident = await IdentGenerator.Value.GenerateAsync();
                 var record=ctx.Add(new DataModels.DataRefundRecord
                 {
                     Id= ident,
                     CollectIdent = Argument.CollectIdent,
                     CollectExtIdent = collectState.extIdent,
                     Title = Argument.Title,
                     Desc = Argument.Desc,
                     //CallbackName = Argument.CallbackName,
                     //CallbackContext = Argument.CallbackContext,
                     Time = time,
                     StartTime = time,
                     Amount = Argument.Amount,
                     PaymentPlatformId = collectState.platformId,
                     //ClientType=Argument.ClientType,
                     BizParent=Argument.BizParent,
                     BizRoot=Argument.BizRoot,
                     UserId = Argument.ClientInfo.OperatorId,
                     State = RefundState.Submitting,
                     OpAddress = Argument.ClientInfo.ClientAddress,
                     OpDevice = Argument.ClientInfo.DeviceType
                 });
                var response = await GetRefundResponse(ident, Argument, provider);
                UpdateRecord(record, response);
                await ctx.SaveChangesAsync();
                return new RefundRefreshResult
                {
                    Id=ident,
                    UpdatedTime = response.UpdatedTime ?? TimeService.Now,
                    Error = response.Error,
                    State = response.State,
                    Desc = Argument.Desc,
                    Expires=response.Expires
                }; 
                ////注册定时器，每小时检查退款状态，直到退款成功
                //await this.CallGuarantor.Schedule(
                //    "SP.Payments.Refund.Update",
                //    Argument.Ident,
                //    null,
                //    null,
                //    "退款跟踪",
                //    TimeService.Now.AddSeconds(1),
                //    0,
                //    60 * 60
                //    );
            });
        }

        async Task<RefundResponse> GetRefundResponse(
            long Ident,
            RefundRequest Request,
            ICollectProvider provider
            )
        {
            try
            {
                if (Request.SubmitTime == DateTime.MinValue)
                    Request.SubmitTime = TimeService.Now;
                return await((IRefundProvider)provider).TryRefund(Ident, Request);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "刷新退款状态时发生异常：" + ex);
                return new RefundResponse
                {
                    UpdatedTime = TimeService.Now,
                    Error = ex.Message,
                    Ident = Ident,
                    State = Request.CurState
                };
            }
        }
        void UpdateRecord(DataModels.DataRefundRecord record,RefundResponse Response)
        {
            switch (Response.State)
            {
                case RefundState.Failed:
                    record.CompletedTime = Response.UpdatedTime;
                    record.ExtIdent = Response.ExtIdent;
                    break;
                case RefundState.Processing:
                    record.ExtIdent = Response.ExtIdent;
                    if (record.SubmitTime == null)
                        record.SubmitTime = Response.UpdatedTime;
                    break;
                case RefundState.Submitting:
                    break;
                case RefundState.Success:
                    record.AmountRefund = Response.RefundAmount;
                    record.CompletedTime = Response.UpdatedTime;
                    record.ExtIdent = Response.ExtIdent;
                    break;
            }

            record.Error = Response.Error;
            record.State = Response.State;
            record.LastUpdateTime = TimeService.Now;
            record.UpdateCount++;
        }
        public async Task<RefundRefreshResult> RefreshRefundRecord(long Ident)
        {
            var Request=await GetRequest(Ident);

            var provider = CollectProviderResolver(Request.PaymentPlatformId);


            //string desc = "";
            //switch (Response.State)
            //{
            //    case RefundState.Init:
            //        desc = "退款初始化";
            //        break;
            //    case RefundState.Submitting:
            //        desc = "正在提交退款情况";
            //        break;
            //    case RefundState.Failed:
            //        desc = "退款失败";
            //        break;
            //    case RefundState.Success:
            //        desc = "退款成功";
            //        break;
            //    case RefundState.Processing:
            //        desc = "第三方支付正在处理退款";
            //        break;
            //}

            //await this.CallGuarantor.Schedule(
            //    Request.CallbackName,
            //    Request.CallbackContext,
            //    Response.State.ToString(),
            //    Response.Error == null ? null : new InvalidOperationException(Response.Error),
            //    Request.Title,
            //    DateTime.MinValue,
            //    0,
            //    5 * 60
            //    );
            var Response = await GetRefundResponse(Ident, Request, provider);
            await DataScope.Use("保存退款记录", async ctx =>
             {
                 var r = await ctx.Set<DataModels.DataRefundRecord>().FindAsync(Response.Ident);
                 if (r == null)
                     throw new ArgumentException("找不到收款支付记录:" + Response.Ident);
                 UpdateRecord(r, Response);
                 ctx.Update(r);
                 await ctx.SaveChangesAsync();
             });
            //await RefundStorage.SaveResponse(Response);
            return new RefundRefreshResult
            {
                Id=Ident,
                UpdatedTime = Response.UpdatedTime ?? TimeService.Now,
                Error = Response.Error,
                State = Response.State,
                Desc = Request.Desc,
                Expires=Response.Expires
            };
            //if (Response.State != RefundState.Failed && Response.State != RefundState.Success)
            //   throw new ServiceProtocol.CallGuarantors.RepeatCallException(TimeService.Now.AddHours(1));
        }



        //public async Task Start(string Ident, DateTime Time, string ExtIdent, string ExtraData)
        //{
        //	var r = await Context.Editable<TModel>().FindAsync(Ident);
        //	if (r == null)
        //		throw new ArgumentException("找不到收款支付记录");
        //	r.StartTime = Time;
        //	r.ExtraData = ExtraData;
        //          r.ExtIdent = ExtIdent;
        //          r.State = RefundState.Processing;
        //	Context.Update(r);
        //	await Context.SaveChangesAsync();
        //}

        async Task<RefundRequest> GetRequest(long Ident)
        {
            return await DataScope.Use("获取退款请求", async ctx =>
            {
                var q = ctx.Queryable<DataModels.DataRefundRecord>().Where(r => r.Id== Ident)
                       .Select(r => new
                       {
                           SubmitTime = r.SubmitTime,
                           req = new RefundRequest
                           {
                               Amount = r.Amount,
                               Desc = r.Desc,
                               CollectIdent = r.CollectIdent,
                               CollectExtIdent = r.CollectExtIdent,
                               BizRoot=r.BizRoot,
                               BizParent=r.BizParent,
                               ClientInfo=new Sys.Clients.ClientInfo
                               {
                                   OperatorId= r.UserId.HasValue ? r.UserId.Value : 0,
                                   ClientAddress = r.OpAddress,
                                   DeviceType = r.OpDevice
                               },
                               
                               //ClientType=r.ClientType,
                               Title = r.Title,
                               //CallbackName=r.CallbackName,
                               //CallbackContext=r.CallbackContext,
                               
                           }
                       });
                var re = await q.SingleOrDefaultAsync();
                if (re == null)
                    throw new ArgumentException("找不到收款支付记录");
                re.req.SubmitTime = re.SubmitTime ?? DateTime.MinValue;
                re.req.PaymentPlatformId = await ctx.Queryable<DataModels.DataCollectRecord>()
                    .Where(r => r.Id== re.req.CollectIdent)
                    .Select(r => r.PaymentPlatformId)
                    .SingleOrDefaultAsync();

                return re.req;
            });
        }

        
    }


}
