using SF.Sys.Entities;
using System;
using System.Threading.Tasks;

namespace SF.Biz.Trades
{
    public class SellerTradeQueryArgument : QueryArgument
    {

    }
    

    public interface ISellerTradeService
    {
        Task<Trade> Get(long tradeId, bool withItems);
        Task<QueryResult<Trade>> Query(SellerTradeQueryArgument Arg);
    }


}
