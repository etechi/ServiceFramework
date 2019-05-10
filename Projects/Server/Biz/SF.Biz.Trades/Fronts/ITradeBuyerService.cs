using SF.Sys.Entities;
using System;
using System.Threading.Tasks;

namespace SF.Biz.Trades
{
    public class BuyerTradeQueryArgument : QueryArgument
    {

    }
    public interface IBuyerTradeService
    {
		Task<Trade> Get(long tradeId,bool withItems);
		Task<QueryResult<Trade>> Query(BuyerTradeQueryArgument Arg);
	}
	

}
