using SF.Sys;
using SF.Sys.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Biz.Trades.Services
{
    /// <summary>
    /// 交易折扣批价
    /// </summary>
    public class TradeDiscountAmountResolver : ITradeDiscountAmountResolver
    {
        IEntityReferenceResolver EntityReferenceResolver { get; }
        public TradeDiscountAmountResolver(IEntityReferenceResolver EntityReferenceResolver)
        {
            this.EntityReferenceResolver = EntityReferenceResolver;
        }
        public Task Resolve(TradeInternal trade)
        {
            trade.AmountAfterDiscount = 0;
            trade.DiscountAmount = 0;

            foreach (var item in trade.Items)
            {
                item.DiscountAmount = 0;
                item.PriceAfterDiscount = item.Price;
                item.AmountAfterDiscount = item.Amount;
                trade.AmountAfterDiscount += item.AmountAfterDiscount;

                if (!item.DiscountEntityIdent.HasContent())
                {
                    var e = (IDiscountItem)EntityReferenceResolver.Resolve(item.DiscountEntityIdent, null);
                    e.Apply(item, trade);
                    
                    Ensure.Equal(item.AmountAfterDiscount - item.Amount, item.DiscountAmount,"则扣金额错误");
                    Ensure.Equal(item.AmountAfterDiscount,item.Quantity*item.PriceAfterDiscount, "则扣后小计错误");
                }                
            }

            if (trade.DiscountEntityId.HasContent())
            {
                var e = (IDiscountItem)EntityReferenceResolver.Resolve(trade.DiscountEntityId, null);
            }


            return Task.CompletedTask;
        }
    }

}
