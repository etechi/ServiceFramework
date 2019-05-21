using SF.Sys;
using System;

namespace SF.Biz.Coupons
{
    public class CouponCalcResult
    {
        public decimal NewAmount { get; set; }
        public bool Limited { get; set; }
    }
    public static class CouponDiscountConfig
    {
        public static CouponCalcResult CalcNewAmount(
            this ICouponDiscountConfig cfg, 
            decimal orgAmount,
            int count,
            double maxDiscountPercent
            )
        {
            var maxDiscount =
                maxDiscountPercent > 0 && maxDiscountPercent <= 100 ?
                Math.Min(orgAmount, Math.Floor((decimal)maxDiscountPercent * orgAmount/100)):
                orgAmount;

            decimal discount;
            switch (cfg.Type)
            {
                case CouponType.DiscountCoupon:
                    if (count != 1)
                        throw new PublicArgumentException("折扣优惠券不能重叠使用");
                    discount = orgAmount * cfg.AdjustValue / 100m;
                    break;
                case CouponType.DecreaseCoupon:
                    discount = cfg.AdjustValue* count;
                    break;
                default:
                    throw new NotSupportedException();
            }
            var limited = false;
            if (discount > maxDiscount)
            {
                discount = maxDiscount;
                limited = true;
            }
            return new CouponCalcResult
            {
                Limited = limited,
                NewAmount = Math.Max(orgAmount - discount, 0)
            };
        }
    }
}
