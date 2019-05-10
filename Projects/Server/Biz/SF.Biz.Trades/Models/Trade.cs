using SF.Biz.Accounting;
using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Biz.Trades
{
    public enum TradeType
    {
        /// <summary>
        /// 常规交易
        /// </summary>
        Normal,
        /// <summary>
        /// 模拟交易
        /// </summary>
        Simulated
    }
    /// <summary>
    /// 交易
    /// </summary>
    public class Trade:UIObjectEntityBase
	{
        public static string CreateIdent(long Id,DateTime CreatedTime)
        {
            return "T" + CreatedTime.ToString("yyyyMMdd") + Id.ToString().PadLeft(8, '0');
        }

        ///<title>订单编号</title>
        /// <summary>
        /// 订单编号格式为：T+年月日+8位交易ID
        /// </summary>
        [TableVisible]
        [Layout(1, 2)]
        [EntityTitle]
        public string Ident { get { return CreateIdent(Id, CreatedTime); } }

        ///<title>下单时间</title>
        /// <summary>
        /// 用户创建订单的时间
        /// </summary>
        [TableVisible]
        [Layout(2)]
        public override DateTime CreatedTime { get; set; }

        ///<title>标题</title>
        /// <summary>
        /// 订单描述，一般为首个商品的标题
        /// </summary>
        [TableVisible]
        [Layout(3)]
        public override string Name{ get; set; }

        ///<title>照片</title>
        /// <summary>
        /// 订单图片，一般为首个商品的照片
        /// </summary>
        [Image]
        [Layout(4)]
        public override string Image { get; set; }

        ///<title>总金额</title>
        /// <summary>
        /// 订单总金额，所有订单项金额的合计
        /// </summary>
        [TableVisible]
        [Layout(10,1)]
        public decimal Amount { get; set; }

        ///<title>结算金额</title>
        /// <summary>
        /// 扣除优惠后的剩余金额，买家需要支付的金额。
        /// </summary>
        [TableVisible]
        [Layout(10, 2)]
        public decimal SettlementAmount { get; set; }

        ///<title>折扣编号</title>
        /// <summary>
        /// 相关折扣对象ID，一般为优惠券的ID
        /// </summary>
        [Layout(11, 2)]
        [EntityIdent(null,nameof(DiscountDesc))]
        public string DiscountEntityId { get; set; }

        ///<title>折扣说明</title>
        /// <summary>
        /// 当结算金额比总金额少时，说明折扣的来源
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
        /// 订单条目
        /// </summary>
        public IEnumerable<TradeItem> Items { get; set; }

        /// <summary>
        /// 充值记录
        /// </summary>
        [EntityIdent(typeof(DepositRecord))]
        public long? DepositRecordId { get; set; }

        /// <summary>
        /// 退款记录
        /// </summary>
        [EntityIdent(typeof(RefundRecord))]
        public long? RefundRecordId { get; set; }

        /// <summary>
        /// 交易类型
        /// </summary>
        [TableVisible]
        public TradeType TradeType { get; set; }
    }
}
