using SF.Sys.Entities;
using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.NetworkService;

namespace SF.Biz.ShoppingCarts.Managements
{
    public class ShoppingCartItemQueryArgument : ObjectQueryArgument
    {
        
        /// <summary>
        /// 商品
        /// </summary>
        [EntityIdent]
        public string ItemId { get; set; }

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

    /// <summary>
    /// 购物车管理器
    /// </summary>
    [NetworkService]
    [EntityManager]
    [DefaultAuthorize(PredefinedRoles.运营专员)]
    [DefaultAuthorize(PredefinedRoles.系统管理员)]


    public interface IShoppingCartManager:
        IEntityManager<ObjectKey<long>, ShoppingCartItem>,
        IEntitySource<ObjectKey<long>, ShoppingCartItem, ShoppingCartItemQueryArgument>
    {
    }
}
