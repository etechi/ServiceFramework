using SF.Sys.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Biz.Products;

namespace SF.Biz.Trades.Managements
{
    public class TradeItemQueryArguments : ObjectQueryArgument
    {
        /// <summary>
        /// 订单状态
        /// </summary>
        public TradeState? State { get; set; }

        /// <summary>
        /// 结束类型
        /// </summary>
        public TradeEndType? EndType { get; set; }

        /// <summary>
        /// 买家
        /// </summary>
        [EntityIdent(typeof(User))]
        public string BuyerId { get; set; }

        /// <summary>
        /// 交易
        /// </summary>
        [EntityIdent(typeof(Trade))]
        public int? TradeId { get; set; }

        /// <summary>
        /// 商品
        /// </summary>
        [EntityIdent(typeof(Item))]
        public virtual int? ItemId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateQueryRange CreatedTime { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 模拟交易
        /// </summary>
        public bool? SimulatedTrade { get; set; }

    }
    public interface ITradeItemManager:
        IEntitySource<ObjectKey<long>, TradeItem, TradeItemQueryArguments>
    {
    }

}
