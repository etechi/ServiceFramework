using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Biz.Payments;
using SF.Sys;
using SF.Sys.Data;
using SF.Sys.Events;
using SF.Sys.Logging;
using SF.Sys.Reminders;
using SF.Sys.TimeServices;

namespace SF.Biz.Accounting
{

    public class DrawbackService :
        IDrawbackService
    {
        public IDataScope DataScope { get; }
        public ITimeService TimeService { get; }
        public Payments.IRefundService PaymentsRefundService { get; }
        public Lazy<IIdentGenerator<DataModels.DataDrawbackRecord>> IdentGenerator { get; }
        public ILogger Logger { get; }
        public Lazy<IEventEmitService> EventEmitService { get; }
        public DrawbackService(
            IDataScope DataScope,
            ITimeService TimeService,
            Payments.IRefundService PaymentsRefundService,
            Lazy<IIdentGenerator<DataModels.DataDrawbackRecord>> IdentGenerator,
            ILogger<DepositService> Logger,
            Lazy<IEventEmitService> EventEmitService
            )
        {
            this.DataScope = DataScope;
            this.TimeService = TimeService;
            this.PaymentsRefundService = PaymentsRefundService;
            this.IdentGenerator = IdentGenerator;
            this.Logger = Logger;
            this.EventEmitService = EventEmitService;
        }


        public async Task<long> Create(DrawbackArgument Arg)
        {
            return await DataScope.Use("创建退款操作", async ctx =>
            {

                if (Arg.Amount <= 0)
                    throw new ArgumentException("退款金额必须大于0");

                var title = await ctx.Queryable<DataModels.DataAccountTitle>()
                    .Where(t => t.Ident == Arg.AccountTitle)
                    .Select(t => t.Id).SingleOrDefaultAsync();
                if (title == 0)
                    throw new ArgumentException("账户科目不存在:" + Arg.AccountTitle);

                var depositRecord = await ctx.FindAsync<DataModels.DataDepositRecord>(Arg.DepositRecordId);
                if (depositRecord == null)
                    throw new ArgumentException("找不到充值记录");
                if (depositRecord.State != DepositState.Completed &&
                    depositRecord.State != DepositState.Refunded &&
                    depositRecord.State != DepositState.Refunding
                    )
                    throw new ArgumentException("充值未成功，不能退款");

                var refundedAmount = await ctx.Queryable<DataModels.DataDrawbackRecord>()
                    .Where(r => r.DepositRecordId == Arg.DepositRecordId)
                    .Select(r => r.Amount)
                    .DefaultIfEmpty(0)
                    .SumAsync();

                if (refundedAmount + Arg.Amount > depositRecord.Amount)
                    throw new ArgumentException("退款金额已超出充值未退款金额");

                if (depositRecord.DrawbackRequest + Arg.Amount > depositRecord.Amount)
                    throw new ArgumentException("退款金额已超出充值未退款金额");

                var time = TimeService.Now;
                depositRecord.State = DepositState.Refunding;
                depositRecord.DrawbackRequest += Arg.Amount;
                depositRecord.LastDrawbackRequestTime = time;
                depositRecord.LastDrawbackReason = Arg.Reason.Limit(100);
                ctx.Update(depositRecord);

                var rid = await IdentGenerator.Value.GenerateAsync();

                var prid=await PaymentsRefundService.Create(new Payments.RefundRequest
                {
                    PaymentPlatformId = depositRecord.PaymentPlatformId,
                    Amount = Arg.Amount,
                    Desc = Arg.Description,
                    Title = Arg.Description,
                    SubmitTime = time,
                    CollectIdent = depositRecord.CollectRecordId,
                    OpAddress = Arg.OpAddress,
                    OpDevice = Arg.OpDevice,
                    TrackEntityIdent = "账户退款记录-" + rid,
                    CurUserId = depositRecord.DstId
                });


                var record = ctx.Add(new DataModels.DataDrawbackRecord
                {
                    DepositRecordId = Arg.DepositRecordId,
                    AccountTitleId = title,
                    SrcId = depositRecord.DstId,
                    OperatorId = Arg.OperatorId,
                    Amount = Arg.Amount,
                    Title = Arg.Description.Limit(200),
                    TrackEntityIdent = Arg.TrackEntityIdent,
                    CreatedTime = time,
                    CallbackName = Arg.CallbackName,
                    CallbackContext = Arg.CallbackContext,
                    PaymentPlatformId = depositRecord.PaymentPlatformId,
                    State = DrawbackState.Processing,
                    DepositRecordCreateTime = depositRecord.Time,
                    Reason = Arg.Reason.Limit(100),
                    PaymentsRefundRecordId=prid
                });
                await ctx.SaveChangesAsync();

                return record.Id;
            });
        }

        public async Task<DrawbackState> Refresh(long Id, long DstId)
        {
            return await DataScope.Use("刷新退款结果", async ctx =>
            {
                var record = await ctx.FindAsync<DataModels.DataDrawbackRecord>(Id);
                if (record.State == DrawbackState.Error || record.State == DrawbackState.Success)
                    return record.State;


                var orgState = record.State;
                var time = TimeService.Now;

                try
                {
                    var result = await PaymentsRefundService.RefreshRefundRecord(record.PaymentsRefundRecordId);
                    switch (result.State)
                    {
                        case Payments.RefundState.Failed:
                            record.CompletedTime = result.UpdatedTime;
                            record.State = DrawbackState.Error;
                            break;
                        case Payments.RefundState.Processing:
                            //旧通知，忽略
                            if (record.State == DrawbackState.Sending)
                                record.SubmittedTime = result.UpdatedTime;
                            record.State = DrawbackState.Processing;
                            break;
                        case Payments.RefundState.Submitting:
                            record.State = DrawbackState.Sending;
                            break;
                        case Payments.RefundState.Success:
                            record.State = DrawbackState.Success;
                            record.CompletedTime = result.UpdatedTime;
                            break;
                        default:
                            throw new ArgumentException("不支持支付退款状态：" + result.State);
                    }

                    record.UpdatedTime = time;

                    if (result.State == Payments.RefundState.Success)
                    {
                        var acc = await ctx.Queryable<DataModels.DataAccount>()
                            .Where(a => a.AccountTitleId == record.AccountTitleId && a.OwnerId.Equals(record.SrcId))
                            .SingleOrDefaultAsync();

                        if (acc == null)
                            acc = ctx.Add(new DataModels.DataAccount
                            {
                                OwnerId = record.SrcId,
                                AccountTitleId = record.AccountTitleId,
                                Inbound = 0,
                                Outbound = record.Amount,
                                CurValue = -record.Amount,
                                UpdatedTime = time
                            });
                        else
                        {
                            acc.Outbound += record.Amount;
                            acc.CurValue = acc.Inbound - acc.Outbound;
                            acc.UpdatedTime = time;
                        }
                        record.CurValue = acc.CurValue;


                        var depositRecord = await ctx.FindAsync<DataModels.DataDepositRecord>(record.DepositRecordId);
                        depositRecord.DrawbackSuccess += depositRecord.Amount;
                        depositRecord.LastDrawbackSuccessTime = time;
                        if (depositRecord.DrawbackSuccess >= depositRecord.DrawbackRequest)
                            depositRecord.State = DepositState.Refunded;
                        ctx.Update(depositRecord);
                    }

                    record.PaymentDesc = result.Desc;
                    record.Error = result.Error;
                    record.UpdatedTime = result.UpdatedTime;

                }
                catch (Exception exp)
                {
                    record.Error = exp?.Message.Limit(200);
                }
                ctx.Update(record);
                await ctx.SaveChangesAsync();

                Logger.Warn($"退款状态更新,ID:{record.Id} 状态：{record.State} 错误:{record.Error}");
                if (record.State == DrawbackState.Error || record.State == DrawbackState.Success)
                {
                    //var e = Event.Create("账户", "充值完成", Tuple.Create(RecordId, record.DstId, record.Amount, Desc));
                    var e = await EventEmitService.Value.Create(new AccountRefundCompleted(
                        record.Id,
                        record.SrcId,
                        record.PaymentDesc,
                        record.Amount,
                        record.State == DrawbackState.Success
                        ));
                    await e.Commit();
                }

                return record.State;

                ////总是提交回调
                //if ((record.State == RefundState.Error || record.State == RefundState.Success)
                //    && record.CallbackName != null)
                //    await CallGuarantor.Value.Schedule(
                //        record.CallbackName,
                //        record.CallbackContext,
                //        record.Id.ToString(),
                //        exp,
                //        "退款操作已完成",
                //        DateTime.MinValue,
                //        0,
                //        60
                //        );

            });


        }



    }
}
