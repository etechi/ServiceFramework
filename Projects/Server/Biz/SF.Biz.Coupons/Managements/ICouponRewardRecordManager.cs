using SF.Sys.Entities;
using SF.Sys.NetworkService;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using SF.Sys.Annotations;
using SF.Sys.Auth;
using System;

namespace SF.Biz.Coupons.Managements
{
    public class CouponRewardRecordQueryArgument : EventQueryArgument
    {
        /// <summary>
        /// 用户
        /// </summary>
        [EntityIdent(typeof(User))]
        public long? UserId { get; set; }

        /// <summary>
        /// 优惠券
        /// </summary>
        [EntityIdent(typeof(Coupon))]
        public long? CouponId { get; set; }

        /// <summary>
        /// 优惠券模板
        /// </summary>
        [EntityIdent(typeof(CouponTemplate))]
        public long? TemplateId { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public CouponRewardState? State { get; set; }

    }

   

    /// <summary>
    /// 优惠券发放记录管理
    /// </summary>
    [NetworkService]
    [EntityManager]
    [DefaultAuthorize(PredefinedRoles.运营专员)]
    [DefaultAuthorize(PredefinedRoles.客服专员)]
    [DefaultAuthorize(PredefinedRoles.系统管理员)]
    public interface ICouponRewardRecordManager :
        IEntitySource<ObjectKey<long>, CouponRewardRecord, CouponRewardRecordQueryArgument>
    {

    }

}
