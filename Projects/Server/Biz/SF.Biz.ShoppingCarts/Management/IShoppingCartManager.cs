using SF.Sys.Entities;
using SF.Sys.Annotations;
using SF.Biz.Products;
using SF.Sys.Auth;

namespace SF.Biz.ShoppingCarts.Managements
{
    public class ShoppingCartItemQueryArgument : ObjectQueryArgument
    {
        /// <summary>
        /// 产品
        /// </summary>
        [EntityIdent(typeof(ProductBase))]
        public long? ProductId { get; set; }

        /// <summary>
        /// 商品
        /// </summary>
        [EntityIdent(typeof(Item))]
        public long? ItemId { get; set; }

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
        /// 数量
        /// </summary>
        public QueryRange<int> Quantity { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public QueryRange<decimal> Price { get; set; }
    }
    public interface IShoppingCartManager:
        IEntityManager<ObjectKey<long>, ShoppingCartItem>,
        IEntitySource<ObjectKey<long>, ShoppingCartItem, ShoppingCartItemQueryArgument>
    {
    }
}
