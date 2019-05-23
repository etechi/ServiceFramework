using SF.Biz.Accounting;
using SF.Common.Addresses;
using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Biz.Trades
{
    /// <summary>
    /// 交易
    /// </summary>
    public class TradeBase:ObjectEntityBase
	{
        ///<title>名称</title>
        /// <summary>
        /// 订单名称，一般为首个商品的名称
        /// </summary>
        [ReadOnly(true)]
        public override string Name { get; set; }


        ///<title>标题</title>
        /// <summary>
        /// 订单标题，一般为首个商品的标题
        /// </summary>
        [Layout(4)]
        [MaxLength(100)]
        [ReadOnly(true)]
        public string Title { get; set; }

        ///<title>照片</title>
        /// <summary>
        /// 订单图片，一般为首个商品的照片
        /// </summary>
        [Image]
        [Layout(4)]
        [ReadOnly(true)]
        public string Image { get; set; }

        ///<title>总金额</title>
        /// <summary>
        /// 订单总金额，所有订单项金额的合计
        /// </summary>
        [TableVisible]
        [Layout(10,1)]
        [ReadOnly(true)]
        public decimal Amount { get; set; }

        /// <summary>
        /// 合计折扣金额
        /// </summary>
        [TableVisible]
        [Layout(10, 1)]
        [ReadOnly(true)]
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// 合计折扣后金额
        /// </summary>
        [TableVisible]
        [Layout(10, 1)]
        [ReadOnly(true)]
        public decimal AmountAfterDiscount { get; set; }

        ///<title>结算金额</title>
        /// <summary>
        /// 扣除优惠后的剩余金额，买家需要支付的金额。
        /// </summary>
        [TableVisible]
        [Layout(10, 2)]
        public decimal TotalSettlementAmount { get; set; }

        ///<title>退回金额</title>
        /// <summary>
        /// 结算金额中已退回的金额
        /// </summary>
        [TableVisible]
        public decimal TotalSettlementRollbackAmount { get; set; }

        ///<title>剩余金额</title>
        /// <summary>
        /// 结算金额中剩余金额
        /// </summary>
        [TableVisible]
        public decimal TotalSettlementLeftAmount { get; set; }

        ///<title>折扣编号</title>
        /// <summary>
        /// 相关折扣对象ID，一般为优惠券的ID
        /// </summary>
        [Layout(11, 2)]
        [EntityIdent(null,nameof(DiscountDesc))]
        public string DiscountEntityId { get; set; }

        ///<title>折扣说明</title>
        /// <summary>
        /// </summary>
        [Layout(11, 1)]
        [TableVisible]
        public string DiscountDesc { get; set; }

        ///<title>折扣数量</title>
        /// <summary>
        /// 相关折扣对象数量
        /// </summary>
        [Layout(11, 2)]
        [TableVisible]
        public int DiscountEntityCount { get; set; }

        ///<title>订单状态</title>
        /// <summary>
        /// 订单进行状态
        /// </summary>
        [TableVisible]
        [Layout(20,1)]
        public TradeState State { get; set; }

        ///<title>结束状态</title>
        /// <summary>
        /// 当订单已经结束时，此项为订单结束结果状态
        /// </summary>
        [TableVisible]
        [Layout(20, 2)]
        public TradeEndType EndType { get; set; }

        ///<title>状态时间</title>
        /// <summary>
        /// 最后一次订单状态变化的时间
        /// </summary>
        [Layout(20, 3)]
        public DateTime LastStateTime { get; set; }

        ///<title>失败原因</title>
        /// <summary>
        /// 当订单未正常结束时，此项为订单未正常结束原因
        /// </summary>
        [Layout(21)]
        public string EndReason { get; set; }

        /// <summary>
        /// 买方
        /// </summary>
        [Display(Name ="")]
        [EntityIdent(typeof(User),nameof(BuyerName))]
        [Layout(30,1)]
        public long BuyerId { get; set; }

        /// <summary>
        /// 买方
        /// </summary>
        [TableVisible]
        [Ignore]
        public string BuyerName { get; set; }

        /// <summary>
        /// 卖方
        /// </summary>
        [EntityIdent(typeof(User), nameof(SellerName))]
        [Layout(30, 2)]
        public long SellerId { get; set; }

        /// <summary>
        /// 卖方
        /// </summary>
        [TableVisible]
        [Ignore]
        public string SellerName { get; set; }

        /// <summary>
        /// 买方备注
        /// </summary>
        public string BuyerRemarks { get; set; }

        /// <summary>
        /// 卖方备注
        /// </summary>
        public string SellerRemarks { get; set; }

        /// <summary>
        /// 结算记录
        /// </summary>
        [EntityIdent(typeof(SettlementRecord))]
        public long? SettlementRecordId { get; set; }


        /// <summary>
        /// 交易类型
        /// </summary>
        [TableVisible]
        public TradeType TradeType { get; set; }


        /// <summary>
        /// 收件地址
        /// </summary>
        [EntityIdent(typeof(UserAddress))]
        public long? DeliveryAddressId { get; set; }

        /// <summary>
        /// 是否需要收件地址
        /// </summary>
        [ReadOnly(true)]
        public bool DeliveryAddressRequired { get; set; }
    }
}
