using System.Collections.Generic;
using System.ComponentModel;

namespace SF.Biz.Trades
{

    public class Trade : TradeBase
    {
        /// <summary>
        /// 订单条目
        /// </summary>
        public IEnumerable<TradeItem> Items { get; set; }

    }
}
