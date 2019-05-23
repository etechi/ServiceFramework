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
    public class SellerCompleteArgument : IArgumentWithExpires
    {
        public DateTime? Expires { get; set; }
        public bool Aborted { get; set; }
        public string Error { get; set; }
    }
    public class SellerCompleteProvider : BaseTradeStateProvider<NoArgumentRequired>
    {
        public override string OpName => "卖家处理订单";
        NamedServiceResolver<ITradeDeliveryProvider> DeliveryProvider { get; }
        ITimeService TimeService { get; }
        public SellerCompleteProvider(NamedServiceResolver<ITradeDeliveryProvider> DeliveryProvider, ITimeService TimeService)
        {
            this.DeliveryProvider = DeliveryProvider;
            this.TimeService = TimeService;
        }
        public override Task<TradeExecResult> AdvanceExpired(DataTrade trade)
        {
            throw new NotImplementedException();
        }

        public override Task<TradeExecResult> ExecutingExpired(DataTrade trade)
        {
            return UpdateStatus(trade, null);
        }

        public override async Task<TradeExecResult> UpdateStatus(DataTrade trade, DateTime? Expires)
        {
            var completed = true;
            string error = null;
            var failed = false;
            foreach (var g in trade.Items.GroupBy(i => (i.DeliveryProvider, i.SellerId)))
            {
                var provider = DeliveryProvider(g.Key.DeliveryProvider);
                var re=await provider.QueryStatus(new TrackIdent("交易", "Trade", trade.Id, $"{g.Key.SellerId}"));
                if (re.State == TradeDeliveryState.WaitDeliverying)
                    completed = false;
                if (re.State == TradeDeliveryState.Failed)
                {
                    failed = true;
                    error = re.Error;
                }
            }

            if(completed)
                return new TradeExecResult
                {
                    NextState = failed? TradeState.SellerSettlement:TradeState.BuyerComplete,
                    Status = TradeExecStatus.IsCompleted,
                    EndReason=error,
                    EndType= failed ? TradeEndType.SellerAborted: TradeEndType.InProcessing
                };
            else
                return new TradeExecResult
                {
                    NextState = trade.State,
                    Status = TradeExecStatus.Executing,
                    Expires = TimeService.Now.AddDays(7)
                };
        }

        
        protected override async Task<TradeExecResult> Advance(DataTrade trade, NoArgumentRequired Argument)
        {
            //if (Argument.Aborted)
            //{
            //    return new TradeExecResult
            //    {
            //        NextState = TradeState.SellerSettlement,
            //        Status = TradeExecStatus.IsCompleted,
            //        EndReason = Argument.Error,
            //        EndType = TradeEndType.SellerAborted
            //    };
            //}
            var completed = true;
            foreach (var g in trade.Items.GroupBy(i =>(i.DeliveryProvider, i.SellerId)))
            {
                var provider = DeliveryProvider(g.Key.DeliveryProvider);

                var re = await provider.Create(new TradeDeliveryCreateArgument
                {
                    BizParent = new TrackIdent("交易", "Trade", trade.Id,$"{g.Key.SellerId}"),
                    BizRoot = trade.BizRoot,
                    DestAddressId=trade.DeliveryAddressId.Value,
                    ReceiverId=trade.BuyerId,
                    SenderId=trade.SellerId,
                    Name=trade.Name,
                    
                    Items = g.Select(i => new TradeDeliveryItemCreateArgument
                    {
                        Name=i.Title,
                        Image=i.Image,
                        PayloadEntityIdent = i.ProductId,
                        Quantity = i.Quantity
                    }).ToArray()
                });
                if (re.State == TradeDeliveryState.WaitDeliverying || re.State == TradeDeliveryState.Failed)
                    completed = false;
            }


            if (completed)
                return new TradeExecResult
                {
                    NextState = TradeState.BuyerComplete,
                    Status = TradeExecStatus.IsCompleted
                };
            else
                return new TradeExecResult
                {
                    NextState = trade.State,
                    Status = TradeExecStatus.Executing,
                    Expires = TimeService.Now.AddDays(7)
                };
        }
    }
}
