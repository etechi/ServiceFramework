using SF.Biz.Payments;
using SF.Sys;
using SF.Sys.Auth;
using SF.Sys.Data;
using SF.Sys.Events;
using SF.Sys.Linq;
using SF.Sys.Logging;
using SF.Sys.Reminders;
using SF.Sys.TimeServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Biz.Accounting
{
    public class DepositService :
        IDepositService
    {
        public IDataScope DataScope { get; }
        public ITimeService TimeService { get; }
        public ICollectService CollectService { get; }
        public Lazy<IIdentGenerator> IdentGenerator { get; }
        public ILogger Logger { get; }
        public Lazy<IRemindService> RemindService { get; }
        public Lazy<IEventEmitService> EventEmitService { get; }
        public Lazy<IUserProfileService> UserProfileService { get; }
        public DepositService(
            IDataScope DataScope,
            ITimeService TimeService,
            ICollectService CollectService,
            Lazy<IIdentGenerator> IdentGenerator,
            ILogger<DepositService> Logger,
            Lazy<IRemindService> RemindService,
            Lazy<IEventEmitService> EventEmitService,
            Lazy<IUserProfileService> UserProfileService
            )
        {
            this.DataScope = DataScope;
            this.TimeService = TimeService;
            this.CollectService = CollectService;
            this.IdentGenerator = IdentGenerator;
            this.Logger = Logger;
            this.RemindService = RemindService;
            this.EventEmitService = EventEmitService;
            this.UserProfileService = UserProfileService;
        }


        public async Task<long> Create(DepositArgument Arg)
        {
           
            Arg.Amount = Math.Round(Arg.Amount);
            if (Arg.Amount <= 0 || Arg.Amount > 1000000)
                throw new ArgumentException("金额不合法");
            var desc = Arg.Description.Limit(200);
            var id = await IdentGenerator.Value.GenerateAsync<DataModels.DataDepositRecord>();
            

            return await DataScope.Use("创建充值操作", async ctx =>
            {
                var title = await ctx.Queryable<DataModels.DataAccountTitle>()
                         .Where(t => t.Ident == Arg.AccountTitle)
                         .Select(t => t.Id).SingleOrDefaultAsync();
                if (title == 0)
                    throw new ArgumentException("账户科目不存在:" + Arg.AccountTitle);

                var cid = await CollectService.Create(new Payments.CollectRequest
                {
                    Amount = Arg.Amount,
                    Desc = desc,
                    Title = desc,
                    PaymentPlatformId = Arg.PaymentPlatformId,
                    ClientType = Arg.ClientType,
                    OpAddress = Arg.OpAddress,
                    OpDevice = Arg.OpDevice,
                    TrackEntityIdent = "账户充值记录-" + id,
                    CurUserId = Arg.DstId,
                    HttpRedirect = Arg.HttpRedirest.Replace(new Dictionary<string, object> { { "DepositId", id.ToString() } }),
                    RemindId= Arg.RemindId
                });

                var time = TimeService.Now;
                var record = ctx.Add(new DataModels.DataDepositRecord
                {
                    Id = id,
                    AccountTitleId = title,
                    DstId = Arg.DstId,
                    OperatorId = Arg.OperatorId,
                    Amount = Arg.Amount,
                    Title = desc,
                    TrackEntityIdent = Arg.TrackEntityIdent,
                    Time = time,
                    RemindId = Arg.RemindId,
                    PaymentPlatformId = Arg.PaymentPlatformId,
                    State = DepositState.New,
                    OpAddress = Arg.OpAddress,
                    OpDevice = Arg.OpDevice,
                    CollectRecordId=cid
                });

                return id;
            });
        }
        public async Task<DepositStartResult> Start(long Id, Biz.Payments.StartRequestInfo RequestInfo)
        {
            return await DataScope.Use("开始充值操作", async ctx =>
            {
                var record = await ctx.Set<DataModels.DataDepositRecord>().FindAsync(Id);
                if (record == null)
                    throw new ArgumentException("充值记录不存在");
                if (record.State != DepositState.New)
                    throw new ArgumentException("充值记录状态错误:" + record.State + ":" + record.Id);

                try
                {
                    var re = await CollectService.Start(record.CollectRecordId, RequestInfo);
                    record.State = DepositState.Processing;
                    ctx.Update(record);
                    await ctx.SaveChangesAsync();

                    return new DepositStartResult
                    {
                        PaymentStartResult = re,
                        Id = record.Id
                    };
                }
                catch (Exception e)
                {
                    await Complete(
                        ctx,
                        record,
                        new CollectResponse
                        {
                            CompletedTime = TimeService.Now,
                            Error = "充值失败:"+e.Message
                        },
                        "充值失败"
                        );
                    try
                    {
                        await RemindService.Value.Remind(record.RemindId, null);
                    }catch(Exception ex)
                    {
                        Logger.Error(ex, "充值失败提醒激活异常");
                    }
                    throw;
                }
            });
        }
        public async Task<DepositRecord> GetResult(long Id, bool Query=false, bool Remind=false)
        {
            return await DataScope.Use("刷新充值结果", async ctx =>
            {
                var record = await ctx.FindAsync<DataModels.DataDepositRecord>(Id);
                if(record==null)
                {
                    Logger.Warn("充值记录不存在,ID:" + Id);
                    return null;
                }

                if (record.State == DepositState.Processing && Query)
                {
                    var re = await this.CollectService.GetResult(record.CollectRecordId, Query, Remind);
                    await Complete(ctx, record, re.Response, re.Request.Desc);
                }
               
                return await MapDepositRecord(EnumerableEx.From(record).AsQueryable())
                       .SingleOrDefaultAsync();
            });

        }

        async Task Complete(IDataContext ctx,DataModels.DataDepositRecord record, CollectResponse resp,string desc)
        {
            if (resp.CompletedTime.HasValue)
            {
                if (resp.Error == null)
                {
                    var acc = await ctx.Queryable<DataModels.DataAccount>()
                        .Where(a => a.AccountTitleId == record.AccountTitleId && a.OwnerId.Equals(record.DstId))
                        .SingleOrDefaultAsync();

                    if (acc == null)
                        acc = ctx.Add(new DataModels.DataAccount
                        {
                            Id = await IdentGenerator.Value.GenerateAsync<DataModels.DataAccount>(),
                            OwnerId = record.DstId,
                            AccountTitleId = record.AccountTitleId,
                            Inbound = resp.AmountCollected,
                            Outbound = 0,
                            CurValue = resp.AmountCollected,
                            UpdatedTime = TimeService.Now,
                            CreatedTime = TimeService.Now,
                            Name = (await UserProfileService.Value.GetUser(record.DstId)).Name,
                        });
                    else
                    {
                        acc.Inbound += resp.AmountCollected;
                        acc.CurValue = acc.Inbound - acc.Outbound;
                        acc.UpdatedTime = TimeService.Now;
                    }
                    record.CurValue = acc.CurValue;
                }
                record.AmountCollected = resp.AmountCollected;
                record.State = resp.Error == null ? DepositState.Completed : DepositState.Failed;
                record.CompletedTime = TimeService.Now;
                record.PaymentDesc = desc;
                record.Error = resp.Error?.Limit(200);
                ctx.Update(record);
                await ctx.SaveChangesAsync();

                if (record.Error == null)
                {
                    var titleIdent = await ctx
                        .Queryable<DataModels.DataAccountTitle>()
                        .Where(t => t.Id == record.AccountTitleId)
                        .Select(t => t.Ident)
                        .SingleOrDefaultAsync();

                    await ctx.EmitEvent(EventEmitService.Value, new AccountDepositCompleted(
                        record.Id,
                        record.DstId,
                        desc,
                        record.AmountCollected,
                        titleIdent
                        ));
                }
            }
        }

        public virtual IQueryable<DepositRecord> MapDepositRecord(IQueryable<DataModels.DataDepositRecord> query)
        {
            return from r in query
                   select new DepositRecord
                   {
                       Id = r.Id,
                       Time = r.Time,
                       DstId = r.DstId,
                       Amount = r.Amount,
                       CurValue = r.CurValue,
                       Title = r.Title,
                       AccountTitleId = r.AccountTitleId,
                       AccountTitle = r.AccountTitle.Title,
                       CompletedTime = r.CompletedTime,
                       State = r.State,
                       PaymentDesc = r.PaymentDesc,
                       PaymentPlatformId = r.PaymentPlatformId,
                       TrackEntityIdent = r.TrackEntityIdent,
                       Error = r.Error,
                       RefundRequest = r.DrawbackRequest,
                       RefundSuccess = r.DrawbackSuccess,
                       LastRefundRequestTime = r.LastDrawbackRequestTime,
                       LastRefundSuccessTime = r.LastDrawbackSuccessTime,
                       LastRefundReason = r.LastDrawbackReason,

                       OpAddress = r.OpAddress,
                       OpDevice = r.OpDevice
                   };
        }



    }
}
