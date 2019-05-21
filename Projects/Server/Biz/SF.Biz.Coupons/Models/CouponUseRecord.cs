using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Biz.Coupons
{
    public enum CouponRecordType
    {
        /// <summary>
        /// 获得
        /// </summary>
        Increment,
        /// <summary>
        /// 使用
        /// </summary>
        Decrement
    }
    
    public enum CouponRecordState
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal,
        /// <summary>
        /// 未生效
        /// </summary>
        NotReady,
        /// <summary>
        /// 失效
        /// </summary>
        Expires
    }
    /// <summary>
    /// 优惠券使用记录
    /// </summary>
    public class CouponUseRecord : EventEntityBase
    {
        /// <summary>
        /// 优惠券
        /// </summary>
        [EntityIdent(typeof(Coupon), nameof(CouponId))]
        [TableVisible]
        public long CouponId { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        [EntityIdent(typeof(User),nameof(OwnerName))]
        public long OwnerId { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        [Ignore]
        [TableVisible]
        public string OwnerName { get; set; }

        /// <summary>
        /// 模板
        /// </summary>
        [EntityIdent(typeof(CouponTemplate), nameof(Name))]
        public long TemplateId { get; set; }

        /// <summary>
        /// 模板
        /// </summary>
        [TableVisible]
        public string Name { get; set; }

        /// <summary>
        /// 获得时间
        /// </summary>
        [TableVisible]
        public DateTime CreatedTime { get; set; }

        [Display(Name = "类型")]
        [TableVisible]
        public CouponType Type { get; set; }

        /// <summary>
        /// 使用时间
        /// </summary>
        [ReadOnly(true)]        
        [TableVisible]
        public override DateTime Time { get; set; }

        /// <summary>
        /// 使用数量
        /// </summary>
        [ReadOnly(true)]
        [TableVisible]
        public int Count { get; set; }

        /// <summary>
        /// 最低金额
        /// </summary>
        [TableVisible]
        public decimal ConditionValue { get; set; }

        /// <summary>
        /// 优惠额
        /// </summary>
        [TableVisible]
        public decimal AdjustValue { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        [EntityIdent(null, nameof(SrcDesc))]
        public string SrcEntityIdent { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        [ReadOnly(true)]
        [TableVisible]
        public string SrcDesc { get; set; }

        /// <summary>
        /// 用途
        /// </summary>
        [EntityIdent(null, nameof(DstDesc))]
        public string DstEntityIdent { get; set; }

        /// <summary>
        /// 用途
        /// </summary>
        [ReadOnly(true)]
        [TableVisible]
        public string DstDesc { get; set; }
    }


}

