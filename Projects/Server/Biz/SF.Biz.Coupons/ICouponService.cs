using SF.Sys.Auth;
using SF.Sys.Entities;
using System;
using System.Threading.Tasks;
using SF.Sys.Annotations;
using SF.Sys;

namespace SF.Biz.Coupons
{
   
    public enum CouponErrorCode
    {
        Missing,
        Expired,
        Used,
        Invalid,
        ConditionCheckFailed,
        NotReady

    }
    public class CouponException : PublicArgumentException
    {
        public CouponErrorCode Code { get; }
        public CouponException(CouponErrorCode Code,string Message) : base(Message)
        {
            this.Code = Code;
        }
    }

    public class CouponApplyArgument
    {
        public DateTime Time { get; set;}
        public string Code { get; set; }
        public int Count { get; set; }
        public long UserId { get; set; }
        public string DstDesc { get; set; }
        public string DstEntityId { get; set; }
        public decimal OrgAmount { get; set; }
    }

    public class CouponCreateArgument
    {
        public int OwnerId { get; set; }
        public int TemplateId { get; set; }
        public DateTime Time { get; set; }
        public string SrcDesc { get; set; }
        public string SrcEntityId { get; set; }
        public int Count { get; set; }
    }
    
    public interface ICouponService
    {
        Task<int> GetTotalCount(long OwnerId,long? TemplateId=null);

        Task<ICouponDiscountConfig> GetCouponDiscountConfig(long OwnerId, string Code,int Count,decimal OrgAmount);
        Task<long> Create(CouponCreateArgument Arg);

        Task<ICouponDiscountConfig> GetCouponDiscountConfig(CouponApplyArgument Arg);
        Task<decimal> Apply(ICouponDiscountConfig cfg, CouponApplyArgument Arg);
        Task<decimal> Apply(CouponApplyArgument Arg);

        double MaxDiscountPercent { get; }

    }
}
