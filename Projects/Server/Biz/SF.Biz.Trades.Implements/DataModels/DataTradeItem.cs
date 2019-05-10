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
        /// 交易项目排位
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// 产品类型
        /// </summary>
		[Required]
		[MaxLength(50)]
		[Index("product", Order =1 )]
        public string ProductType { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
		[Index("product", Order = 2)]
		[Index]
        public long ProductId { get; set; }

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
        /// 价格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 价格折扣描述
        /// </summary>
        [MaxLength(100)]
		public string PriceDiscountDesc { get; set; }

        /// <summary>
        /// 折扣后的结算价格
        /// </summary>
        public decimal SettlementPrice { get; set; }

        /// <summary>
        /// 小计
        /// </summary>
        public decimal Amount { get; set; } //ammont=SettlementPrice*Quantity

        /// <summary>
        /// 小计折扣描述
        /// </summary>
        [MaxLength(100)]
        public string AmountDiscountDesc { get; set; }

        /// <summary>
        /// 折扣来源ID
        /// </summary>
        [MaxLength(100)]
        public string DiscountEntityIdent { get; set; }


        /// <summary>
        /// 小计折扣后的结算金额
        /// </summary>
        public decimal SettlementAmount { get; set; }

        ///<title>项目结算金额</title>
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
