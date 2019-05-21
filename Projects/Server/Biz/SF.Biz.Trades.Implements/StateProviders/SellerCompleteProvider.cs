using SF.Biz.Trades.DataModels;
using SF.Biz.Trades.Managements;
using SF.Sys.Entities;
using SF.Sys.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Biz.Trades.StateProviders
{
    public class SellerCompleteArgument : IArgumentWithExpires
    {
        public DateTime? Expires { get; set; }
    }
    public class SellerCompleteProvider : BaseTradeStateProvider<SellerCompleteArgument>
    {
        public override string OpName => "卖家处理订单";
        NamedServiceResolver<ITradeDeliveryProvider> DeliveryProvider { get; }
        public SellerCompleteProvider(NamedServiceResolver<ITradeDeliveryProvider> DeliveryProvider)
        {
            this.DeliveryProvider = DeliveryProvider;
        }
        public override Task<TradeExecResult> AdvanceExpired(DataTrade trade)
        {
            throw new NotImplementedException();
        }

        public override Task<TradeExecResult> ExecutingExpired(DataTrade trade)
        {
            throw new NotImplementedException();
        }

        public override async Task<TradeExecResult> UpdateStatus(DataTrade trade, DateTime? Expires)
        {
            var trackIdent = new TrackIdent("交易", "Trade", trade.Id);
            var completed = true;
            foreach (var g in trade.Items.GroupBy(i => i.DeliveryProvider))
            {
                var provider = DeliveryProvider(g.Key);
                var re=await provider.QueryStatus(trackIdent);
                if (re.State == TradeDeliveryState.Processing)
                    completed = false;
            }

            return new TradeExecResult
            {
                NextState = completed ? TradeState.BuyerComplete : trade.State,
                Status = completed ? TradeExecStatus.IsCompleted : TradeExecStatus.Executing
            };
        }

        
        protected override async Task<TradeExecResult> Advance(DataTrade trade, SellerCompleteArgument Argument)
        {
            //var trackIdent = new TrackIdent("交易", "Trade", trade.Id);
            //var completed = true;
            //foreach (var g in trade.Items.GroupBy(i => i.DeliveryProvider))
            //{
            //    var provider = DeliveryProvider(g.Key);

            //    var re=await provider.Start(new StartDeliveryArgument
            //    {
            //        BizParent = trackIdent,
            //        BizRoot = trade.BizRoot,
            //        DeliveryAddressId = trade.DeliveryAddressId,
            //        Items = g.Select(i => new StartDeliveryItem
            //        {
            //            ProductId = i.ProductId,
            //            Quantity = i.Quantity

            //        }).ToArray()
            //    });
            //    if (re.State == TradeDeliveryState.Processing)
            //        completed = false;
            //}
            var completed = true;

            return new TradeExecResult
            {
                NextState = completed?TradeState.BuyerComplete:trade.State,
                Status = completed? TradeExecStatus.IsCompleted:TradeExecStatus.Executing
            };
        }
    }
}
