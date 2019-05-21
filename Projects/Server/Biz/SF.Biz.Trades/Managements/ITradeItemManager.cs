using SF.Sys.Entities;
using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.NetworkService;

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
        public long? TradeId { get; set; }

        /// <summary>
        /// 商品类型
        /// </summary>
        [EntityType(Tag ="可交易")]
        public virtual string ItemType { get; set; }

        /// <summary>
        /// 商品
        /// </summary>
        [EntityIdent(EntityTypeField =nameof(ItemType))]
        public virtual long? ItemId { get; set; }

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

    /// <summary>
    /// 交易项目管理器
    /// </summary>
    [NetworkService]
    [EntityManager]
    [DefaultAuthorize(PredefinedRoles.运营专员)]
    [DefaultAuthorize(PredefinedRoles.系统管理员)]

    public interface ITradeItemManager:
        IEntitySource<ObjectKey<long>, TradeItemInternal, TradeItemQueryArguments>
    {
    }

}
