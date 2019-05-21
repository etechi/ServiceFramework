using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Biz.Coupons
{
    public interface ICouponDiscountConfig
    {
        string Name { get; }
        decimal ConditionValue { get; }
        decimal AdjustValue { get; }
        int LeftCount { get; }
        CouponType Type { get; }
    }
    /// <summary>
    /// 优惠券
    /// </summary>
    public class Coupon : ObjectEntityBase, ICouponDiscountConfig
    {
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
        [EntityIdent(typeof(CouponTemplate),nameof(Name))]
        public int TemplateId { get; set; }

        /// <summary>
        /// 模板
        /// </summary>
        [TableVisible]
        public override string Name { get; set; }

        [Ignore]
        public string Image { get; set; }

        [Ignore]
        public string Description { get; set; }

        /// <summary>
        /// 启用时间
        /// </summary>
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? ExpireTime { get; set; }

        /// <summary>
        /// 最后使用时间
        /// </summary>
        [ReadOnly(true)]        
        [TableVisible]
        public DateTime? LastUseTime { get; set; }

        /// <summary>
        /// 最后使用数量
        /// </summary>
        [ReadOnly(true)]
        [TableVisible]
        public int LastUseCount { get; set; }

        /// <summary>
        /// 最后获得时间
        /// </summary>
        [ReadOnly(true)]
        [TableVisible]
        public DateTime? LastRewardTime { get; set; }

        /// <summary>
        /// 最后获得数量
        /// </summary>
        [ReadOnly(true)]
        [TableVisible]
        public int LastRewardCount { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [TableVisible]
        public CouponType Type { get; set; }

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

        //[Display(Name = "来源")]
        //[EntityIdent(null,nameof(SrcDesc))]
        //public string SrcEntityIdent { get; set; }

        //[Display(Name = "来源")]
        //[ReadOnly(true)]
        //[TableVisible]
        //public string SrcDesc { get; set; }

        //[Display(Name = "用途")]
        //[EntityIdent(null, nameof(DstDesc))]
        //public string DstEntityIdent { get; set; }

        //[Display(Name = "用途")]
        //[ReadOnly(true)]
        //public string DstDesc { get; set; }

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

