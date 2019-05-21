using System.Threading.Tasks;

namespace SF.Biz.Trades
{
    /// <summary>
    /// 订单标准结算金额生成接口
    /// </summary>
    public interface ITradeNormalAmountResolver
    {
        Task Resolve(TradeInternal Trade);

    }

}
