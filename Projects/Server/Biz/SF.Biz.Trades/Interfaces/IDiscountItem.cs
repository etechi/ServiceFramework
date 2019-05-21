using SF.Sys.Entities;
using System.Threading.Tasks;

namespace SF.Biz.Trades
{
  
    public interface IDiscountItem
    {
        Task Apply(TradeInternal trade);
        Task Apply(TradeItemInternal item, TradeInternal trade);
        Task Commit();
    }
}
