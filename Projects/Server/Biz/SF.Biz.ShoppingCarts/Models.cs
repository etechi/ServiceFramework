using SF.Sys.Annotations;
using SF.Sys.Entities.Models;

namespace SF.Biz.ShoppingCarts
{
    /// <summary>
    /// 购物车项目
    /// </summary>
    [EntityObject]
    public class ShoppingCartItem : UIObjectEntityBase
    {
        /// <summary>
        /// 卖家
        /// </summary>
        public long SellerId { get; set; }

        /// <summary>
        /// 商品
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Spec { get; set; }

        /// <summary>
        /// 折扣
        /// </summary>
        public string Discount { get; set; }

        /// <summary>
        /// 不适用优惠券
        /// </summary>
        public bool CouponDisabled { get; set; }

        /// <summary>
        /// 结算价格
        /// </summary>
        public decimal SettlementPrice { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 小计
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 是否选择
        /// </summary>
        public bool Selected { get; set; }
    }


    public class ItemStatus
    {
        public bool Selected { get; set; }
        public int Quantity { get; set; }
        public string ItemId { get; set; }

    }
}
