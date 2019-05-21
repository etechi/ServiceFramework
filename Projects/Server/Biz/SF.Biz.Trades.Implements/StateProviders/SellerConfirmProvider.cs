using SF.Biz.Trades.DataModels;
using SF.Biz.Trades.Managements;
using SF.Sys.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Biz.Trades.StateProviders
{
    public class SellerConfirmProvider : BaseTradeStateProvider<NoArgumentRequired>
    {
        public override string OpName => "卖家确认订单";

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

        protected override Task<TradeExecResult> Advance(DataTrade trade, NoArgumentRequired Argument)
        {
            return Task.FromResult(new TradeExecResult
            {
                NextState=TradeState.SellerComplete,
                Status=TradeExecStatus.IsCompleted
            });
        }
    }
}
