using SF.Biz.Accounting;
using SF.Biz.Trades.DataModels;
using SF.Biz.Trades.Managements;
using SF.Sys;
using SF.Sys.Clients;
using SF.Sys.Entities;
using SF.Sys.TimeServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Biz.Trades.StateProviders
{
    public class TradeBuyerConfirmArgument : IArgumentWithExpires
    {
        public bool UseBalance { get; set; }
        public ClientInfo ClientInfo { get; set; }
        public long? PaymentPlatformId { get; set; }
        public string HttpRedirect { get; set; }
        public DateTime? Expires { get; set; }
    }
    
    public class BuyerConfirmProvider : BaseTradeStateProvider<TradeBuyerConfirmArgument>
    {
        Lazy<IAccountService> AccountService { get; }
        Lazy<ISettlementManager> SettlementManager { get; }
        ITimeService TimeService { get; }
        public override string OpName => "买家支付";
        public override TimeSpan? AdvanceWaitTimeout => TimeSpan.FromDays(1);
        public override bool Restartable => true;

        public BuyerConfirmProvider(
            Lazy<IAccountService> AccountService, 
            Lazy<ISettlementManager> SettlementManager,
            ITimeService TimeService
            )
        {
            this.AccountService = AccountService;
            this.SettlementManager = SettlementManager;
            this.TimeService = TimeService;
        }

     

        protected override async Task<TradeExecResult> Advance(DataTrade trade, TradeBuyerConfirmArgument arg)
        {
            if(trade.SettlementRecordId.HasValue)
                await SettlementManager.Value.Cancel(trade.SettlementRecordId.Value);

            var titles = arg.UseBalance ? await AccountService.Value.GetSettlementAccounts(trade.BuyerId) : null;
            var leftAmount = trade.TotalSettlementAmount;
            var items = new List<SettlementItemArgument>();
            if (titles != null)
                foreach (var title in titles)
                {
                    var amountUsed = Math.Min(leftAmount, title.Value);
                    items.Add(new SettlementItemArgument
                    {
                        Amount = amountUsed,
                        AccountTitle = title.Title
                    });
                    leftAmount -= amountUsed;
                    if (leftAmount == 0)
                        break;
                }
            var settlementArgument = new SettlementStartArgument
            {
                BizParent = new TrackIdent("结算", "Trade", trade.Id,TimeService.Now.ToString("yyyyMMddHHmmss")),
                BizRoot = trade.BizRoot,
                BuyerId = trade.BuyerId,
                ClientInfo = arg.ClientInfo,
                Desc = trade.Title,
                Name = trade.Name,
                DstTitle = "balance",
                PrepayTitle = "trade-prepay",
                SellerId = trade.SellerId,
                TotalAmount = trade.TotalSettlementAmount
            };

            if(leftAmount>0)
            {
                settlementArgument.CollectHttpRedirect = arg.HttpRedirect;
                settlementArgument.CollectPaymentPlatformId = arg.PaymentPlatformId;
                settlementArgument.CollectTitle = "trade-collect";
                items.Add(new SettlementItemArgument
                {
                    AccountTitle = "trade-collect",
                    Amount = leftAmount
                });
            }
            settlementArgument.Items = items.ToArray();

            var result =await SettlementManager.Value.Start(settlementArgument);

            trade.SettlementRecordId = result.Id;
            
            return new TradeExecResult
            {
                Expires = result.Expires,
                Results = result.CollectStartResult,
                Status = result.State==SettlementState.WaitConfirm?TradeExecStatus.IsCompleted:TradeExecStatus.Executing,
                NextState=result.State==SettlementState.WaitConfirm?TradeState.SellerConfirm:trade.State
            };
        }

        async Task<TradeExecResult> UpdateStatus(DataTrade trade, DateTime? Expires,bool LastTime)
        {
            if (!trade.SettlementRecordId.HasValue)
                throw new InvalidOperationException($"交易没有结算记录:{trade.Id}");

            var result = await SettlementManager.Value.UpdateAndQueryStatus(trade.SettlementRecordId.Value);
            switch (result.State)
            {
                case SettlementState.Failed:
                    return new TradeExecResult
                    {
                        Status = TradeExecStatus.IsCompleted,
                        EndReason = "结算错误:" + result.Message,
                        EndType = TradeEndType.BuyerConfirmError,
                        NextState = TradeState.Closed
                    };
                case SettlementState.Processing:
                    if(LastTime)
                        return new TradeExecResult
                        {
                            Status = TradeExecStatus.IsCompleted,
                            EndReason = "订单支付过期",
                            EndType = TradeEndType.BuyerCancelledBeforeConfirm,
                            NextState = TradeState.Closed
                        };
                    else
                        return new TradeExecResult
                        {
                            Status = TradeExecStatus.Executing,
                            Expires = Expires
                        };
                case SettlementState.Success:
                case SettlementState.WaitConfirm:
                    return new TradeExecResult
                    {
                        Status = TradeExecStatus.IsCompleted,
                        NextState=TradeState.SellerConfirm
                    };
                default:
                    throw new InvalidOperationException($"无效状态:{result.State}");
            }
        }

        public override Task<TradeExecResult> UpdateStatus(DataTrade trade, DateTime? Expires)
        {
            return UpdateStatus(trade, Expires, false);
        }

        public override Task<TradeExecResult> ExecutingExpired(DataTrade trade)
        {
            return UpdateStatus(trade, null, true);
        }

        public override Task<TradeExecResult> AdvanceExpired(DataTrade trade)
        {
            return Task.FromResult(new TradeExecResult
            {
                Status = TradeExecStatus.IsCompleted,
                EndReason = "订单支付过期",
                EndType = TradeEndType.BuyerCancelledBeforeConfirm,
                NextState = TradeState.Closed
            });
        }
    }
}
