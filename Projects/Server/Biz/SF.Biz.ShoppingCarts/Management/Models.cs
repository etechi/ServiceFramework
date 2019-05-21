using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities.Models;

namespace SF.Biz.ShoppingCarts.Managements
{
    /// <summary>
    /// 购物车项目
    /// </summary>
    [EntityObject]
    public class ShoppingCartItem : UIObjectEntityBase
    {
        /// <summary>
        /// 购物车类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 买家
        /// </summary>
        [EntityIdent(typeof(User),nameof(BuyerName))]
        public long BuyerId { get; set; }

        /// <summary>
        /// 买家
        /// </summary>
        [TableVisible]
        [Ignore]
        public string BuyerName { get; set; }

        /// <summary>
        /// 卖家
        /// </summary>
        [EntityIdent(typeof(User), nameof(SellerName))]
        public long SellerId { get; set; }
        /// <summary>
        /// 卖家
        /// </summary>
        [TableVisible]
        [Ignore]
        public string SellerName { get; set; }

        /// <summary>
        /// 商品
        /// </summary>
        [EntityIdent]
        public string ItemId { get; set; }

        /// <summary>
        /// 卖家标题
        /// </summary>
        public string SellerTitle { get; set; }


        /// <summary>
        /// 规格
        /// </summary>
        public string Spec { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// 市场价
        /// </summary>
        public decimal MarketPrice { get; set; }

        /// <summary>
        /// 售价
        /// </summary>
        public decimal Price { get; set; }
    }


}
