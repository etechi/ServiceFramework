using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Entities.DataModels;

namespace SF.Biz.Coupons.DataModels
{
    /// <summary>
    /// 优惠券模板
    /// </summary>
    public class DataCouponTemplate: DataUIObjectEntityBase
    {
        /// <summary>
        /// 生效时间
        /// </summary>
        public DateTime? BeginTime { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 过期天数
        /// </summary>
        public int? ExpireDays { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public CouponType Type { get; set; }

        /// <summary>
        /// 使用条件
        /// </summary>
        public decimal ConditionValue { get; set; }

        /// <summary>
        /// 调整额
        /// </summary>
        public decimal AdjustValue { get; set; }
        /// <summary>
        /// 目标标识
        /// </summary>
        [MaxLength(100)]        
        public string TargetEntityIdent { get; set; }
    }
}
