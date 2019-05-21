using SF.Sys.Entities;

namespace SF.Biz.Trades.Managements
{
    public class TradeItemManager :
        AutoQueryableEntitySource<ObjectKey<long>, TradeItemInternal, TradeItemInternal, TradeItemQueryArguments, DataModels.DataTradeItem>,
        ITradeItemManager
    {

        public TradeItemManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
        {
        }
    }
}
