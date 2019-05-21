using SF.Sys.Annotations;
using SF.Sys.Entities.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Biz.Coupons
{
    public enum CouponType
    {
        /// <summary>
        /// 抵价券
        /// </summary>
        DecreaseCoupon,
        /// <summary>
        /// 折扣券
        /// </summary>
        DiscountCoupon,
    }
    /// <summary>
    /// 优惠券模板
    /// </summary>
    public class CouponTemplate : UIObjectEntityBase
    {
        ///<title>标题</title>
        /// <summary>
        /// 优惠券名称，如：1元直减红包
        /// </summary>
        [MaxLength(100)]
        [TableVisible]
        [Required]
        [Layout(1, 1, 2)]
        public override string Title { get; set; }

        ///<title>说明</title>
        /// <summary>
        /// 优惠券简要描述，如：直减1元，3天有效
        /// </summary>
        [MaxLength(200)]
        [Required]
        [Layout(1, 1, 3)]
        public override string Description { get; set; }

        ///<title>使用范围</title>
        /// <summary>
        /// 优惠券使用范围，暂不支持。已生成的优惠券会随之调整
        /// </summary>
        [Ignore]
        public string TargetEntityIdent { get; set; }

        ///<title>起效时间</title>
        /// <summary>
        /// 优惠券生效时间，如过设置，在此时间以后优惠券才能使用
        /// </summary>
        [TableVisible]
        [Layout(1, 1, 7)]
        public DateTime? BeginTime { get; set; }

        ///<title>失效时间</title>
        /// <summary>
        /// 优惠券失效时间，如过设置，在此时间以后优惠券不能再使用。
        /// </summary>
        [TableVisible]
        [Layout(1, 1, 8)]
        [Date(EndTime=true)]
        public DateTime? EndTime { get; set; }

        ///<title>有效期</title>
        /// <summary>
        /// 有效期2天的优惠券，1月2日15:30获得，1月4日15:30过期，已生成的优惠券不会随之调整。如果同时设置两个时间，以最先到达时间作为实际失效时间。0-不设有效期，默认为1年
        /// </summary>
        [TableVisible]
        [Range(0,1000)] 
        [Layout(1, 1, 9)]
        public int? ExpireDays { get; set; }

        ///<title>优惠券类型</title>
        /// <summary>
        /// 已生成的优惠券类型不会调整
        /// </summary>
        [TableVisible]
        [Layout(1, 1, 4)]
        public CouponType Type { get; set; }

        ///<title>折扣条件</title>
        /// <summary>
        /// 可使用的最小金额(如100表示订单金额必须>=100)，已生成的优惠券不会调整。
        /// </summary>
        [Range(0,1000000)]
        [TableVisible]
        [Layout(1, 1, 5)]
        public decimal ConditionValue { get; set; }

        ///<title>优惠额度</title>
        /// <summary>
        /// 抵价券为金额，折扣券为折扣比例（如20表示打八折），已生成的优惠券不会调整。
        /// </summary>
        [Range(0, 1000000)]
        [TableVisible]
        [Layout(1, 1, 6)]
        public decimal AdjustValue { get; set; }

        
    }
    
}

