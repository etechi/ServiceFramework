using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Entities.DataModels;
using SF.Sys.Data;

namespace SF.Biz.Coupons.DataModels
{
    /// <summary>
    /// 优惠券
    /// </summary>
    public class DataCoupon : DataObjectEntityBase
    {

        /// <summary>
        /// 所有人ID
        /// </summary>
        [Index("owner",Order =1)]
        [Index("merge",Order =1)]
        public override long? OwnerId { get; set; }

        /// <summary>
        /// 优惠券模板ID
        /// </summary>
        [Index]
        [Index("merge", Order = 2)]
        public long TemplateId { get; set; }

        [ForeignKey(nameof(TemplateId))]
        public DataCouponTemplate Template { get; set; }


        /// <summary>
        /// 优惠券类型
        /// </summary>
        public CouponType Type { get; set; }

        /// <summary>
        /// 优惠券使用条件
        /// </summary>
        public decimal ConditionValue { get; set; }

        /// <summary>
        /// 优惠券调整额
        /// </summary>
        public decimal AdjustValue { get; set; }

        /// <summary>
        /// 已用总数
        /// </summary>
        public int TotalUsedCount { get; set; }

        /// <summary>
        /// 最后获得时间
        /// </summary>
        public DateTime LastRewardTime { get; set; }
        /// <summary>
        /// 最后获得个数
        /// </summary>
        public int LastRewardCount { get; set; }

        /// <summary>
        /// 最后使用时间
        /// </summary>
        public DateTime? LastUseTime { get; set; }
        /// <summary>
        /// 最后使用个数
        /// </summary>
        public int LastUseCount { get; set; }


        [InverseProperty(nameof(DataCouponRewardRecord.Coupon))]
        public ICollection<DataCouponRewardRecord> RewardRecords { get; set; }

    }
}
