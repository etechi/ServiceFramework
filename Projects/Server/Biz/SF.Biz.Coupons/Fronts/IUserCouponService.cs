using SF.Sys.Entities;
using System.Threading.Tasks;
using SF.Sys.Annotations;

namespace SF.Biz.Coupons
{
    public class CouponQueryArgument : ObjectQueryArgument
    {
        /// <summary>
        /// 模板
        /// </summary>
        [EntityIdent(typeof(CouponTemplate))]
        public long? TemplateId { get; set; }

    }
    public class CouponRewardRecordQueryArgument : EventQueryArgument
    {
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
    public class CouponUseRecordQueryArgument : EventQueryArgument
    {

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

    }
    public class CalcNewAmountArgument
    {
        public string Code { get; set; }
        public int Count { get; set; }
        public decimal OrgAmount { get; set; }
    }
    public interface IUserCouponService
    {
        Task<Coupon> Get(long Id);
        Task<QueryResult<Coupon>> Query(CouponQueryArgument Arg);
        Task<QueryResult<CouponRewardRecord>> QueryRewardRecord(CouponRewardRecordQueryArgument Arg);
        Task<QueryResult<CouponUseRecord>> QueryUseRecord(CouponUseRecordQueryArgument Arg);
        Task<int> GetTotalCount();
        Task<CouponCalcResult> CalcNewAmount(CalcNewAmountArgument Arg);
    }
}
