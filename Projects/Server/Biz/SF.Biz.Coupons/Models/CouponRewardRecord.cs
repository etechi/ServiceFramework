using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Biz.Coupons
{
    public enum CouponRewardState
    {
        /// <summary>
        /// 无效
        /// </summary>
        Invalid = 0,
        /// <summary>
        /// 有效
        /// </summary>
        Valid = 1,
        /// <summary>
        /// 过期
        /// </summary>
        Expired = 2,
        /// <summary>
        /// 已使用
        /// </summary>
        Used = 3,
        /// <summary>
        /// 未生效
        /// </summary>
        NotReady = 4,
    }
    /// <summary>
    /// 优惠券获取记录
    /// </summary>
    public class CouponRewardRecord : EventEntityBase
    {
        /// <summary>
        /// 用户
        /// </summary>
        [EntityIdent(typeof(User),nameof(OwnerName))]
        public long OwnerId { get; set; }

        /// <summary>
        /// 优惠券
        /// </summary>
        [EntityIdent(typeof(Coupon), nameof(CouponId))]
        [TableVisible]
        public int CouponId { get; set; }

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
        public int TemplateId { get; set; }

        /// <summary>
        /// 模板
        /// </summary>
        [TableVisible]
        public string Name { get; set; }


        /// <summary>
        /// 生效时间
        /// </summary>
        [TableVisible]
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        [TableVisible]
        public DateTime ExpiresTime { get; set; }

        /// <summary>
        /// 最后使用时间
        /// </summary>
        [ReadOnly(true)]
        [TableVisible]
        public DateTime? LastUsedTime { get; set; }

        /// <summary>
        /// 最后使用个数
        /// </summary>
        [ReadOnly(true)]
        [TableVisible]
        public int LastUsedCount { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [ReadOnly(true)]
        [TableVisible]
        public CouponRewardState State { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        [EntityIdent(null,nameof(SrcDesc))]
        public string SrcEntityIdent { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        [ReadOnly(true)]
        [TableVisible]
        public string SrcDesc { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [ReadOnly(true)]
        [TableVisible]
        public int Count { get; set; }

        /// <summary>
        /// 剩余
        /// </summary>
        [ReadOnly(true)]
        [TableVisible]
        public int LeftCount { get; set; }
    }
}

