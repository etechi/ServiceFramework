using SF.Sys.Entities;
using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.NetworkService;

namespace SF.Biz.Shops.Managements
{

    public class ShopQueryArgument : ObjectQueryArgument
    {
       
        /// <summary>
        /// 卖家
        /// </summary>
        [EntityIdent(typeof(User))]
        public long? SellerId { get; set; }

    }

    /// <summary>
    /// 店铺管理器
    /// </summary>
    [NetworkService]
    [EntityManager]
    [DefaultAuthorize(PredefinedRoles.运营专员)]
    [DefaultAuthorize(PredefinedRoles.系统管理员)]
    public interface IShopManager:
        IEntityManager<ObjectKey<long>, ShopInternal>,
        IEntitySource<ObjectKey<long>, ShopInternal, ShopQueryArgument>
    {
    }
}
