using SF.Biz.Accounting;
using SF.Biz.Trades.DataModels;
using SF.Biz.Trades.Managements;
using SF.Sys.Entities;
using SF.Sys.Services;
using SF.Sys.TimeServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Biz.Trades.StateProviders
{
    public class BuyerCompleteProvider : BaseTradeStateProvider<NoArgumentRequired>
    {
        ITimeService TimeService { get; }
        public override string OpName => "买家确认收货";
        public NamedServiceResolver<ITradeDeliveryProvider> DeliveryProvider { get; }
        public BuyerCompleteProvider(NamedServiceResolver<ITradeDeliveryProvider> DeliveryProvider, ITimeService TimeService)
        {
            this.DeliveryProvider = DeliveryProvider;
            this.TimeService = TimeService;
        }
        public override Task<TradeExecResult> AdvanceExpired(DataTrade trade)
        {
            return Advance(trade, (IArgumentWithExpires)null);
        }

        public override Task<TradeExecResult> ExecutingExpired(DataTrade trade)
        {
            return Task.FromResult(new TradeExecResult
            {
                NextState = TradeState.SellerSettlement,
                Status = TradeExecStatus.IsCompleted
            });
        }

        public override Task<TradeExecResult> UpdateStatus(DataTrade trade, DateTime? Expires)
        {
            return UpdateState(trade,Expires);
        }

        async Task<TradeExecResult> UpdateState(DataTrade trade,DateTime? Expires)
        {
            var completed = true;
            foreach (var g in trade.Items.GroupBy(i => (i.DeliveryProvider, i.SellerId)))
            {
                var provider = DeliveryProvider(g.Key.DeliveryProvider);
                var re = await provider.QueryStatus(new TrackIdent("交易", "Trade", trade.Id, $"{g.Key.SellerId}"));
                if (re.State != TradeDeliveryState.Success)
                    completed = false;
            }

            return new TradeExecResult
            {
                NextState = completed ? TradeState.SellerSettlement : trade.State,
                Status = completed ? TradeExecStatus.IsCompleted : TradeExecStatus.Executing,
                Expires= Expires
            };
        }

        protected override Task<TradeExecResult> Advance(DataTrade trade, NoArgumentRequired Argument)
        {
            return UpdateState(trade,TimeService.Now.AddDays(7));
        }
    }
}
