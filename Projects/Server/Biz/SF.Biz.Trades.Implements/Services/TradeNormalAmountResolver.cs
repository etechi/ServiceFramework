using SF.Sys;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Biz.Trades.Services
{
    /// <summary>
    /// 交易常规批价
    /// </summary>
    public class TradeNormalAmountResolver : ITradeNormalAmountResolver
    {
        public Task Resolve(TradeInternal trade)
        {
            trade.Amount = 0;

            foreach (var item in trade.Items)
            {
                item.Amount = item.Quantity * item.Price;
                trade.Amount += item.Amount;
            }
            return Task.CompletedTask;
        }
    }

}
