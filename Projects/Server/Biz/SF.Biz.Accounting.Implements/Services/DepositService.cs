using SF.Biz.Payments;
using SF.Sys;
using SF.Sys.Auth;
using SF.Sys.Clients;
using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys.Events;
using SF.Sys.Linq;
using SF.Sys.Logging;
using SF.Sys.Reminders;
using SF.Sys.Services;
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
        public Lazy<IPaymentPlatformService> PaymentPlatformService { get; }

        public DepositService(
            IDataScope DataScope,
            ITimeService TimeService,
            ICollectService CollectService,
            Lazy<IIdentGenerator> IdentGenerator,
            ILogger<DepositService> Logger,
            Lazy<IRemindService> RemindService,
            Lazy<IEventEmitService> EventEmitService,
            Lazy<IUserProfileService> UserProfileService,
            Lazy<IPaymentPlatformService> PaymentPlatformService
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
            this.PaymentPlatformService = PaymentPlatformService;
        }


        public async Task<DepositStartResult> Start(DepositArgument Arg)
        {
           
            Arg.Amount = Math.Round(Arg.Amount);
            if (Arg.Amount <= 0 || Arg.Amount > 1000000)
                throw new ArgumentException("金额不合法");
            var desc = Arg.Description.Limit(200);
            var id = await IdentGenerator.Value.GenerateAsync<DataModels.DataDepositRecord>();
            

            return await DataScope.Use("开始充值操作", async ctx =>
            {
                var title = await ctx.Queryable<DataModels.DataAccountTitle>()
                         .Where(t => t.Ident == Arg.AccountTitle)
                         .Select(t => t.Id).SingleOrDefaultAsync();
                if (title == 0)
                    throw new ArgumentException("账户科目不存在:" + Arg.AccountTitle);

                var trackIdent = new Sys.Entities.TrackIdent("充值", "DepositRecord", id).ToString();
                

                var time = TimeService.Now;
                var record = ctx.Add(new DataModels.DataDepositRecord
                {
                    Id = id,
                    AccountTitleId = title,
                    DstId = Arg.DstId,
                    OperatorId = Arg.OperatorId,
                    Amount = Arg.Amount,
                    Title = desc,
                    Time = time,
                    PaymentPlatformId = Arg.PaymentPlatformId,
                    State = DepositState.Processing,
                    OpAddress = Arg.ClientInfo.ClientAddress,
                    OpDevice = Arg.ClientInfo.DeviceType,
                });

                var re = await CollectService.Start(new Payments.CollectRequest
                {
                    BizRoot = trackIdent,
                    BizParent = trackIdent,
                    Amount = Arg.Amount,
                    Desc = desc,
                    Title = desc,
                    PaymentPlatformId = Arg.PaymentPlatformId,
                    ClientInfo = Arg.ClientInfo,
                    HttpRedirect = Arg.HttpRedirest.Replace(new Dictionary<string, object> { { "DepositId", id.ToString() } })
                });

                record.CollectRecordId = re.Id;

                await RemindService.Value.Setup(new RemindSetupArgument
                {
                    BizSource = trackIdent,
                    Name = $"充值{record.Amount}元",
                    RemindableName = typeof(DepositRemindable).FullName,
                    RemindTime = re.Expires
                });

                await ctx.SaveChangesAsync();

                return new DepositStartResult
                {
                    PaymentArguments= re.Data,
                    Expires=re.Expires,
                    Id = record.Id
                };
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
                var r = record;
                return new DepositRecord
                {
                    Id = r.Id,
                    Time = r.Time,
                    DstId = r.DstId,
                    Amount = r.Amount,
                    CurValue = r.CurValue,
                    Title = r.Title,
                    AccountTitleId = r.AccountTitleId,
                    AccountTitle = await ctx.Queryable<DataModels.DataAccountTitle>().Where(t => t.Id == r.AccountTitleId).Select(t => t.Title).SingleAsync(),
                    CompletedTime = r.CompletedTime,
                    State = r.State,
                    PaymentDesc = r.PaymentDesc,
                    PaymentPlatformId = r.PaymentPlatformId,
                    Error = r.Error,
                    RefundRequest = r.DrawbackRequest,
                    RefundSuccess = r.DrawbackSuccess,
                    LastRefundRequestTime = r.LastDrawbackRequestTime,
                    LastRefundSuccessTime = r.LastDrawbackSuccessTime,
                    LastRefundReason = r.LastDrawbackReason,

                    OpAddress = r.OpAddress,
                    OpDevice = r.OpDevice
                };
            });

        }

        async Task Complete(IDataContext ctx,DataModels.DataDepositRecord record, CollectResponse resp,string desc)
        {
            if (resp.CompletedTime.HasValue)
            {
                if (resp.Error == null)
                {
                    var updater = new AccountUpdater(ctx,IdentGenerator.Value);
                    await updater.LoadAccounts((record.AccountTitleId, record.DstId));
                    
                    record.CurValue = await updater.Update(record.AccountTitleId, record.DstId, resp.AmountCollected, 0, TimeService.Now);
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

      


    }
}
