using SF.Sys.Entities;
using SF.Sys.NetworkService;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using SF.Sys.Annotations;
using SF.Sys.Auth;

namespace SF.Biz.Coupons
{
    public class CouponTemplateQueryArgument : ObjectQueryArgument
    {
    }
    /// <summary>
    /// 优惠券模板管理
    /// </summary>
    [NetworkService]
    [EntityManager]
    [DefaultAuthorize(PredefinedRoles.运营专员)]
    [DefaultAuthorize(PredefinedRoles.客服专员)]
    [DefaultAuthorize(PredefinedRoles.系统管理员)]
    public interface ICouponTemplateManager :
        IEntityManager<ObjectKey<long>, CouponTemplate>,
        IEntitySource<ObjectKey<long>, CouponTemplate, CouponTemplateQueryArgument>

    {
    }

}
