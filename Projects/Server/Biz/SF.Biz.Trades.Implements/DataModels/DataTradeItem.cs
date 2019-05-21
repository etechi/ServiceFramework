using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Entities.DataModels;
using SF.Sys.Data;
using SF.Sys.Clients;

namespace SF.Biz.Trades.DataModels
{
    /// <summary>
    /// 交易项目
    /// </summary>
    public class DataTradeItem : DataObjectEntityBase
	{
        /// <summary>
        /// 标题
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        /// <summary>
        /// 交易项目排位
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public string Image { get; set; }


        /// <summary>
        /// 商品Id
        /// </summary>
		[Index]
        [Required]
        [MaxLength(100)]
        public string ProductId { get; set; }

        /// <summary>
        /// 买家ID
        /// </summary>
		[Index]
        public long BuyerId { get; set; }

        /// <summary>
        /// 卖家ID
        /// </summary>
		[Index]
        public long SellerId { get; set; }

        /// <summary>
        /// 交付类型
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string DeliveryProvider { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }


        /// <summary>
        /// 折扣价格
        /// </summary>
        public decimal PriceAfterDiscount { get; set; }


        /// <summary>
        /// 小计
        /// </summary>
        public decimal Amount { get; set; } 

        /// <summary>
        /// 折扣描述
        /// </summary>
        [MaxLength(100)]
        public string DiscountDesc { get; set; }

        /// <summary>
        /// 折扣来源ID
        /// </summary>
        [MaxLength(100)]
        public string DiscountEntityIdent { get; set; }

        /// <summary>
        /// 折扣金额
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// 折扣后的小计金额
        /// </summary>
        public decimal AmountAfterDiscount { get; set; }

        /// <summary>
        /// 最终结算金额
        /// </summary>
        public decimal SettlementAmount { get; set; }

        ///<title>分摊单价</title>
        /// <summary>
        /// 订单结算金额按各项结算金额分摊后的单价
        /// </summary>
        public decimal ApportionPrice { get; set; }

        ///<title>分摊金额</title>
        /// <summary>
        /// 订单结算金额按各项结算金额分摊后的金额
        /// </summary>
        public decimal ApportionAmount { get; set; }

        ///<title>买家备注</title>
        /// <summary>
        /// 买家卖家都可见
        /// </summary>
        [MaxLength(100)]
        public string BuyerRemarks { get; set; }

        ///<title>卖家备注</title>
        /// <summary>
        ///卖家可见
        /// </summary>
        [MaxLength(100)]
        public string SellerRemarks { get; set; }

        /// <summary>
        /// 交易ID
        /// </summary>
		[Index]
        public long TradeId { get; set; }


		[ForeignKey(nameof(TradeId))]
		public DataTrade Trade { get; set; }

        /// <summary>
        /// 交易发起设备
        /// </summary>
        public ClientDeviceType OpDevice { get; set; }

        /// <summary>
        /// 交易发起地址
        /// </summary>
        [MaxLength(20)]
		public string OpAddress { get; set; }

        /// <summary>
        /// 交易类型
        /// </summary>
        [Index]
        public TradeType TradeType { get; set; }


        /// <summary>
        /// 最后状态变更时间
        /// </summary>
        public DateTime LastStateTime { get; set; }

        /// <summary>
        /// 交易状态
        /// </summary>
        [Index]
        public TradeState State { get; set; }

        /// <summary>
        /// 交易结束类型
        /// </summary>
        [Index]
        public TradeEndType EndType { get; set; }

        /// <summary>
        /// 交易结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

    }
}
