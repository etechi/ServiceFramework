using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities.Models;
using System;

namespace SF.Biz.Trades
{
    /// <summary>
    /// 交易项目
    /// </summary>
    public class TradeItem : UIObjectEntityBase
    {
        ///<title>商品类型</title>
        /// <summary>
        /// 对应商品类型，一般为系统默认商品类型
        /// </summary>
        [Layout(0,1)]
        public virtual string ProductType { get; set; }

        ///<title>商品</title>
        /// <summary>
        /// 对应商品
        /// </summary>
        [Layout(0, 2)]
        public virtual long ProductId { get; set; }

        ///<title>标题</title>
        /// <summary>
        /// 订单项目标题，一般为相关商品的标题。
        /// </summary>
        [TableVisible(10)]
        public override string Name { get; set; }


        /// <summary>
        /// 销售价格
        /// </summary>
        [TableVisible(20)]
        [Layout(10, 1)]
        public decimal Price { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        [TableVisible(30)]
        [Layout(10, 2)]
        public int Quantity { get; set; }

        ///<title>金额</title>
        /// <summary>
        /// 商品总金额，等于销售价格 x 购买数量
        /// </summary>
        [TableVisible(40)]
        [Layout(20, 1)]
        public decimal Amount { get; set; }

        ///<title>结算金额</title>
        /// <summary>
        /// 扣除优惠后的剩余金额，买家需要支付的金额。
        /// </summary>
        [Ignore]
        [Layout(20, 2)]
        public decimal SettlementAmount { get; set; }

        ///<title>分摊金额</title>
        /// <summary>
        /// 订单结算价格按照各项结算金额分摊后的金额，前几项分摊金额元整到分，最后一项分摊金额为去除前几项后的余额。
        /// </summary>
        [TableVisible(41)]
        public decimal ApportionAmount { get; set; }

        /// <summary>
        /// 交易类型
        /// </summary>
        [TableVisible(60)]
        public TradeType TradeType { get; set; }

        ///<title>折扣说明</title>
        /// <summary>
        /// 当结算金额比总金额少时，说明折扣的来源
        /// </summary>
        public string DiscountDesc { get; set; }

        /// <summary>
        /// 折扣编号
        /// </summary>
        public string DiscountEntityIdent { get; set; }

        /// <summary>
        /// 买方备注
        /// </summary>
        public string BuyerRemarks { get; set; }

        /// <summary>
        /// 卖方备注
        /// </summary>
        public string SellerRemarks { get; set; }

        /// <title>照片</title>
        /// <summary>
        /// 一般为相应产品的照片
        /// </summary>
        [Image]
        public override string Image { get; set; }

    }

    public class TradeItemInternal :  TradeItem
    {

        /// <summary>
        /// 交易状态
        /// </summary>
        [TableVisible(100)]
        public TradeState State{ get; set; }

        /// <summary>
        /// 完成类型
        /// </summary>
        [TableVisible(110)]
        public TradeEndType EndType { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime? EndTime{ get; set; }


        /// <summary>
        /// 买家
        /// </summary>
        [EntityIdent(typeof(User),nameof(BuyerName))]
        public long BuyerId { get; set; }

        /// <summary>
        /// 买家
        /// </summary>
        [Ignore]
        [TableVisible]
        public string BuyerName { get; set; }

        /// <summary>
        /// 交易
        /// </summary>
        [TableVisible]
        [EntityIdent(typeof(Trade),nameof(TradeType))]
        public long TradeId { get; set; }
    }

}
