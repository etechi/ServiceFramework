using SF.Sys.Entities;
using SF.Sys.NetworkService;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using SF.Sys.Annotations;
using SF.Sys.Auth;

namespace SF.Biz.Coupons.Managements
{
    public class CouponQueryArgument : ObjectQueryArgument
    {
        /// <summary>
        /// 用户
        /// </summary>
        [EntityIdent(typeof(User))]
        public long? OwnerId{get;set;}


        /// <summary>
        /// 模板
        /// </summary>
        [EntityIdent(typeof(CouponTemplate))]
        public long? TemplateId { get; set; }


        /// <summary>
        /// 隐藏用完
        /// </summary>
        public bool? ClearEmptyCoupons { get; set; }
    }
    /// <summary>
    /// 优惠券管理
    /// </summary>
    [NetworkService]
    [EntityManager]
    [DefaultAuthorize(PredefinedRoles.运营专员)]
    [DefaultAuthorize(PredefinedRoles.客服专员)]
    [DefaultAuthorize(PredefinedRoles.系统管理员)]
    public interface ICouponManager :
        IEntitySource<ObjectKey<long>, Coupon, CouponQueryArgument>
    {
    }

}
