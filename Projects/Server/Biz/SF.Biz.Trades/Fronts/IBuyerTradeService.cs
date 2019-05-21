using SF.Sys.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Biz.Trades
{
    public class BuyerTradeQueryArgument : QueryArgument
    {

    }

    public class PaymentResult
    {
        public long TradeId { get; set; }
        public bool Completed { get; set; }
        public IReadOnlyDictionary<string,string> PaymentArguments { get; set; }
    }
    public class PaymentArgument
    {
        public long TradeId { get; set; }
        public bool UseBalance { get; set; }
        public long? PaymentPlatformId { get; set; }
        public string HttpRedirect { get; set; }
        public string DiscountCode { get; set; }
        public int DiscountCount { get; set; }
    }

    public interface IBuyerTradeService
    {
		Task<Trade> Get(long tradeId,bool withItems);
		Task<QueryResult<Trade>> Query(BuyerTradeQueryArgument Arg);

        Task<PaymentResult> Payment(PaymentArgument Arg);

        Task<bool> Confirm(long TradeId);
    }
	

}
