using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Entities.DataModels;
using SF.Sys.Data;

namespace SF.Biz.Coupons.DataModels
{

    /// <summary>
    /// 优惠券领取记录
    /// </summary>
    public class DataCouponRewardRecord : DataEventEntityBase
    {
        /// <summary>
        /// 所有人ID
        /// </summary>
        [Index("owner",Order =1)]        
        public override long? UserId { get; set; }

        /// <summary>
        /// 优惠券ID
        /// </summary>
        [Index("coupon", Order = 1)]
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
        /// 有效时间
        /// </summary>
        ///<remarks>使用前为过期时间，使用后为使用时间</remarks>
        [Index("owner", Order = 2)]
        [Index("coupon", Order = 2)]
        public DateTime ValidTime { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpiresTime { get; set; }

        /// <summary>
        /// 生效时间
        /// </summary>
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// 最后使用个数
        /// </summary>
        public int LastUsedCount { get; set; }

        /// <summary>
        /// 最后使用时间
        /// </summary>
        public DateTime? LastUsedTime { get; set; }

        /// <summary>
        /// 来源描述
        /// </summary>
        [MaxLength(100)]
        public string SrcDesc { get; set; }

        /// <summary>
        /// 来源标识
        /// </summary>
        [MaxLength(100)]
        public string SrcEntityIdent { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 剩余数量
        /// </summary>
        public int LeftCount { get; set; }

        /// <summary>
        /// 乐观锁时间戳
        /// </summary>
        [ConcurrencyCheck]
        [Timestamp]
        public byte[] TimeStamp { get; set; }

    }
}
