using SF.Sys.Auth;
using SF.Sys.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using SF.Sys.Annotations;
using SF.Biz.Products;
using SF.Sys.Clients;
using SF.Sys;
using SF.Sys.NetworkService;

namespace SF.Biz.Trades.Managements
{
    public class TradeItemCreateArgument
    {
        public string Image { get; set; }
        public string Name { get; set; }
        public string ProductType { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }

        public decimal Price { get; set; }
        public string PriceDiscountDesc { get; set; }
        public decimal SettlementPrice { get; set; }

        public string AmmountDiscountDesc { get; set; }
        public string DiscountEntityIdent { get; set; }
        public decimal SettlementAmount { get; set; }

        public string BuyerRemarks { get; set; }
        public string SellerRemarks { get; set; }
    }


    public class TradeCreateArgument
    {
        public long BuyerId { get; set; }
        public long SellerId { get; set; }

        public string BuyerName { get; set; }
        public string SellerName { get; set; }

        public string Image { get; set; }
        public string Name { get; set; }

        public string DiscountDesc { get; set; }
        public string DiscountEntityId { get; set; }
        public int DiscountEntityCount { get; set; }
        public decimal SettlementAmount { get; set; }
        public string Remarks { get; set; }
        public string AddressId { get; set; }
        public TradeItemCreateArgument[] Items { get; set; }
        public DateTime Time { get; set; }

        public ClientDeviceType OpDevice { get; set; }
        public string OpAddress { get; set; }

        public TradeType TradeType { get; set; }
    }
    public class TradeCreateException : PublicInvalidOperationException
    {
        public TradeCreateException(string message) : base(message) { }
        public TradeCreateException(string message, System.Exception innerException) : base(message, innerException) { }
    }
    public class TradeSettlementException : PublicInvalidOperationException
    {
        public TradeSettlementException(string message) : base(message) { }
        public TradeSettlementException(string message, System.Exception innerException) : base(message, innerException) { }
    }
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
        /// 商品
        /// </summary>
        [EntityIdent(typeof(Item))]
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


    /// <summary>
    /// 交易管理器
    /// </summary>
    [NetworkService]
    [EntityManager]
    [DefaultAuthorize(PredefinedRoles.运营专员)]
    [DefaultAuthorize(PredefinedRoles.系统管理员)]

    public interface ITradeManager:
         IEntitySource<ObjectKey<long>, Trade, TradeQueryArguments>
    {
        Task<long> Create(TradeCreateArgument args);
        //卖家取消交易，买家付款前
        Task SellerCancel(long tradeId, string reason, bool expired);

        //卖家确认交易，一般是开始备货
        Task SellerConfirm(long tradeId);

        //卖家终止交易，买家付款后
        Task SellerAbort(long tradeId, string reason);

        //卖家完成交易，一般是发货
        Task SellerComplete(long tradeId);

        //买家结算完成，一般发生在代收结算方式
        Task SellerSettlementCompleted(long tradeId, string paymentRecordId);


        //买家取消交易,付款前
        Task BuyerCancel(long tradeId, string reason, bool expired);

        //买家确认交易，一般是付款成功
        Task BuyerConfirm(long tradeId, string paymentRecordId);

        //买家终止交易, 付款后
        Task BuyerAbort(long tradeId, string reason);

        //买家完成交易，一般是收货
        Task BuyerComplete(long tradeId, bool expired);

        //买家结算完成，一般是退款
        Task BuyerSettlementCompleted(long tradeId, string paymentRecordId);
    }

}
