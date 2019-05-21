using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Entities.DataModels;
using SF.Sys.Data;

namespace SF.Biz.Coupons.DataModels
{
    /// <summary>
    /// 优惠券使用记录
    /// </summary>
    public class DataCouponUseRecord:DataEventEntityBase
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Index("owner",Order =1)]
        public override long? UserId { get; set; }

        /// <summary>
        /// 优惠券ID
        /// </summary>
        [Index]
        public long CouponId { get; set; }

        [ForeignKey(nameof(CouponId))]
        public DataCoupon Coupon { get; set; }


        /// <summary>
        /// 优惠券模板ID
        /// </summary>
        [Index]
        public long TemplateId { get; set; }

        [ForeignKey(nameof(TemplateId))]
        public DataCouponTemplate Template { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        [Index]
        [Index("owner", Order = 2)]
        [Display(Name = "")]
        public override DateTime Time { get; set; }

        /// <summary>
        /// 用途描述
        /// </summary>
        [MaxLength(100)]
        public string DstDesc { get; set; }

        /// <summary>
        /// 用途业务标识
        /// </summary>
        [MaxLength(100)]
        public string DstEntityIdent { get; set; }

        /// <summary>
        /// 使用数量
        /// </summary>
        public int Count { get; set; }
    }
}
