using SF.Sys.Auth;
using SF.Sys.Entities;
using SF.Sys.Annotations;
using SF.Sys.NetworkService;
using System.Threading.Tasks;
using System;

namespace SF.Biz.Trades.Managements
{


    public class TradeQueryArguments: ObjectQueryArgument
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string Ident { get; set; }

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
        public long? BuyerId { get; set; }

        /// <summary>
        /// 卖家
        /// </summary>
        [EntityIdent(typeof(User))]
        public long? SellerId { get; set; }

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
    /*
     * 标的类型
     *      实物
     *      虚拟：抵扣券，团购套餐
     * 交易模式
     *      一口价 👍
     *      砍价
     *      竞价：拍卖 👍   
     *      众筹
     *          独占：一元购
     *          均分：p2p
     *          分组：标的/地址
     *          
     *      
     * 撮合：拍卖/竞价/招标
     * 众筹: 一元购/p2p/团购(同小区)
     * 
     * 
     * 
     * 
     */



    public enum TradeActionType
    {
        SellerCancel,
        SellerConfirm,
        SellerAbort,
        SellerComplete,
        SellerSettlementCompleted,
        BuyerCancel,
        BuyerConfirm,
        BuyerAbort,
        BuyerComplete,
        BuyerSettlementCompleted
    }

    public enum TradeExecStatus
    {
        IsCompleted,
        Executing,
        ArgumentRequired
    }

    public class TradeExecResult
    {
        public TradeExecStatus Status { get; set; }
        public DateTime? Expires { get; set; }
        public object Results { get; set; }
        public TradeEndType EndType { get; set; }
        public string EndReason { get; set; }
        public TradeState NextState { get; set; }
    }
    public interface IArgumentWithExpires
    {
        DateTime? Expires { get; set; }
    }
    public class TradeStateRemindArgument : IArgumentWithExpires
    {
        public DateTime? Expires { get; set; }
    }
    /// <summary>
    /// 交易管理器
    /// </summary>
    [NetworkService]
    [EntityManager]
    [DefaultAuthorize(PredefinedRoles.运营专员)]
    [DefaultAuthorize(PredefinedRoles.系统管理员)]

    public interface ITradeManager:
        IEntitySource<ObjectKey<long>, TradeInternal, TradeQueryArguments>,
        IEntityManager<ObjectKey<long>, TradeInternal>
    {


        /// <summary>
        /// 推进订单
        /// </summary>
        /// <param name="TradeId">订单ID</param>
        /// <param name="ExpectState">期望状态</param>
        /// <param name="Argument">参数, 必须是个状态的启动参数或激活参数</param>
        /// <returns></returns>
        Task<TradeExecResult> Advance(long TradeId, TradeState? ExpectState, IArgumentWithExpires Argument);


    }

}
