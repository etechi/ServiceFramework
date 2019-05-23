using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities.Models;
using System;
using System.ComponentModel;

namespace SF.Biz.Trades
{
    /// <summary>
    /// 交易项目
    /// </summary>
    public class TradeItemBase : ObjectEntityBase
    {
        public static string CreateIdent(long Id, DateTime CreatedTime)
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

        
        /// <summary>
        /// 卖家
        /// </summary>
        [EntityIdent(typeof(User),nameof(SellerName))]
        public long SellerId { get; set; }

        /// <summary>
        /// 卖家
        /// </summary>
        [Ignore]
        [TableVisible]
        public string SellerName { get; set; }


        ///<title>商品</title>
        /// <summary>
        /// 商品
        /// </summary>
        [Layout(0, 2)]
        [EntityIdent]
        [Uneditable]
        public virtual string ProductId { get; set; }

       
        ///<title>商品名</title>
        /// <summary>
        /// 订单项目标题，默认为商品的标题。
        /// </summary>
        [TableVisible(10)]
        [ReadOnly(true)]
        public override string Name { get; set; }

        ///<title>商品标题</title>
        /// <summary>
        /// 订单项目标题，默认为商品的标题。
        /// </summary>
        [TableVisible(10)]
        [ReadOnly(true)]
        public string Title { get; set; }

        /// <summary>
        /// 销售价格
        /// </summary>
        [TableVisible(20)]
        [Layout(10, 1)]
        [ReadOnly(true)]
        public decimal Price { get; set; }


        /// <summary>
        /// 发货类型
        /// </summary>
        [ReadOnly(true)]
        public string DeliveryProvider { get; set; }

        ///<title>折扣后单价</title>
        /// <summary>
        /// 优惠后的单价
        /// </summary>
        [ReadOnly(true)]
        public decimal PriceAfterDiscount { get; set; }

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
        [ReadOnly(true)]
        public decimal Amount { get; set; }

        /// <summary>
        /// 折扣金额
        /// </summary>
        [ReadOnly(true)]
        public decimal DiscountAmount{ get; set; }

        ///<title>折扣后小计</title>
        /// <summary>
        /// 优惠后的小计
        /// </summary>
        [ReadOnly(true)]
        public decimal AmountAfterDiscount { get; set; }


        ///<title>结算金额</title>
        /// <summary>
        /// 最终结算金额，买家需要支付的金额。
        /// </summary>
        [Layout(20, 2)]
        public decimal SettlementAmount { get; set; }

        ///<title>分摊单价</title>
        /// <summary>
        /// 订单结算价格按照各项结算金额分摊后的金额，前几项分摊金额元整到分，最后一项分摊金额为去除前几项后的余额，最后一项单价*数量可能比分摊金额少。
        /// </summary>
        [TableVisible(41)]
        [ReadOnly(true)]
        public decimal ApportionPrice { get; set; }

        ///<title>分摊金额</title>
        /// <summary>
        /// 订单结算价格按照各项结算金额分摊后的金额，前几项分摊金额元整到分，最后一项分摊金额为去除前几项后的余额。
        /// </summary>
        [TableVisible(41)]
        [ReadOnly(true)]
        public decimal ApportionAmount { get; set; }

        /// <summary>
        /// 交易类型
        /// </summary>
        [TableVisible(60)]
        public TradeType TradeType { get; set; }



        ///<title>折扣说明</title>
        /// <summary>
        /// 当使用则扣时，说明折扣的来源
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
        public string Image { get; set; }

    }

}
