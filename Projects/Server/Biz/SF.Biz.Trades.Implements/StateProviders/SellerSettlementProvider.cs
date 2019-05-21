using SF.Biz.Accounting;
using SF.Biz.Trades.DataModels;
using SF.Biz.Trades.Managements;
using SF.Sys.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Biz.Trades.StateProviders
{
    public class SellerSettlementProvider : BaseTradeStateProvider<NoArgumentRequired>
    {
        public override string OpName => "卖家结算";

        ISettlementManager SettlementManager { get; }
        public SellerSettlementProvider(ISettlementManager SettlementManager)
        {
            this.SettlementManager = SettlementManager;
        }
        public override Task<TradeExecResult> AdvanceExpired(DataTrade trade)
        {
            throw new NotImplementedException();
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
            await SettlementManager.Confirm(trade.SettlementRecordId.Value);
            return new TradeExecResult
            {
                EndType = TradeEndType.Completed,
                Status = TradeExecStatus.IsCompleted,
                NextState = TradeState.Closed
            };
        }
    }
}
