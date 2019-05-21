using SF.Biz.Accounting;
using SF.Biz.Trades.DataModels;
using SF.Biz.Trades.Managements;
using SF.Sys.Entities;
using SF.Sys.TimeServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Biz.Trades.StateProviders
{
    public class BuyerCompleteProvider : BaseTradeStateProvider<NoArgumentRequired>
    {
        ITimeService TimeService { get; }
        public override string OpName => "买家确认收货";
        public override TimeSpan? AdvanceWaitTimeout => TimeSpan.FromDays(7);

        public BuyerCompleteProvider()
        {
        }
        public BuyerCompleteProvider(ITimeService TimeService)
        {
            this.TimeService = TimeService;

        }
        public override Task<TradeExecResult> AdvanceExpired(DataTrade trade)
        {
            return Advance(trade, (IArgumentWithExpires)null);
        }

        public override Task<TradeExecResult> ExecutingExpired(DataTrade trade)
        {
            throw new NotImplementedException();
        }

        public override Task<TradeExecResult> UpdateStatus(DataTrade trade, DateTime? Expires)
        {
            throw new NotImplementedException();
        }

        protected override async Task<TradeExecResult> Advance(DataTrade trade, NoArgumentRequired Argument)
        {
            return new TradeExecResult
            {
                NextState = TradeState.SellerSettlement,
                Status = TradeExecStatus.IsCompleted,
            };
        }
    }
}
