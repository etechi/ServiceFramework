using SF.Sys.Entities;

namespace SF.Biz.Trades.Managements
{
    public class TradeItemManager :
        AutoQueryableEntitySource<ObjectKey<long>, TradeItem, TradeItem, TradeItemQueryArguments, DataModels.DataTradeItem>,
        ITradeItemManager
    {

        public TradeItemManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
        {
        }
    }
}
