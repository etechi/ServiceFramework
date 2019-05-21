using SF.Sys.Entities;
using System;
using System.Threading.Tasks;

namespace SF.Biz.Trades
{
    public interface ITradableItem
    {
        string Id { get; }
        string Name { get; }
        string Title { get; }
        string Image { get; }
        decimal MarketPrice { get; }
        decimal Price { get; }
        int? StockLeft { get; }
        long SellerId { get; }
        EntityLogicState LogicState { get; }
        bool CouponDisabled { get;  }
        string DeliveryProvider { get; }
    }
    public interface ITradableItemResolver
    {
        Task<ITradableItem[]> Resolve(string[] Idents);
    }
}
