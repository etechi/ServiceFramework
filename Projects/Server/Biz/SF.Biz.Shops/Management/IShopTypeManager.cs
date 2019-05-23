using SF.Sys.Entities;
using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.NetworkService;

namespace SF.Biz.Shops.Managements
{

    public class ShopTypeQueryArgument : ObjectQueryArgument
    {
       

    }

    /// <summary>
    /// 店铺类型管理器
    /// </summary>
    [NetworkService]
    [EntityManager]
    [DefaultAuthorize(PredefinedRoles.运营专员)]
    [DefaultAuthorize(PredefinedRoles.系统管理员)]
    public interface IShopTypeManager:
        IEntityManager<ObjectKey<long>, ShopTypeInternal>,
        IEntitySource<ObjectKey<long>, ShopTypeInternal, ShopTypeQueryArgument>
    {
    }
}
