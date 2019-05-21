using System.Threading.Tasks;

namespace SF.Biz.Trades
{
    /// <summary>
    /// 交易有效性验证
    /// </summary>
    public interface ITradeValidator
    {
        Task Validate(TradeInternal Trade);

    }

}
