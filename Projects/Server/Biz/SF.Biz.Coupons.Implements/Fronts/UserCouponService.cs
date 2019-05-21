using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Biz.Coupons.Managements;
using SF.Sys;
using SF.Sys.Auth;
using SF.Sys.Clients;
using SF.Sys.Entities;

namespace SF.Biz.Coupons
{
    public class UserCouponServiceSetting
    {
        public Lazy<ICouponManager> CouponManager { get; set; }
        public Lazy<ICouponUseRecordManager> CouponUseRecordManager { get; set; }
        public Lazy<ICouponRewardRecordManager> CouponRewardRecordManager { get; set; }
        public Lazy<ICouponService> CouponService { get; set; }
        public IAccessToken AccessToken { get; set; }


    }
    public class UserCouponService : IUserCouponService
    {
        UserCouponServiceSetting Setting { get; }

        long EnsureUserIdent()
            => Setting.AccessToken.User.EnsureUserIdent();


        public UserCouponService(UserCouponServiceSetting Setting)
        {
            this.Setting = Setting;
        }
        
        
        public async Task<CouponCalcResult> CalcNewAmount(CalcNewAmountArgument Arg)
        {
            var coupon = await Setting.CouponService.Value.GetCouponDiscountConfig(
                EnsureUserIdent(),
                Arg.Code, 
                Arg.Count, 
                Arg.OrgAmount
                );
            return coupon.CalcNewAmount(
                Arg.OrgAmount,
                Arg.Count,
                Setting.CouponService.Value.MaxDiscountPercent
                );
        }

        public async Task<Coupon> Get(long Id)
        {
            var uid = EnsureUserIdent();
            var c=await Setting.CouponManager.Value.GetAsync(Id);
            if (c == null)
                throw new PublicArgumentException($"找不到优惠券{Id}");

            if (c.OwnerId != uid)
                throw new PublicDeniedException();

            return c;
        }

        public async Task<int> GetTotalCount()
        {
            return await Setting.CouponService.Value.GetTotalCount(EnsureUserIdent());
        }

        public Task<QueryResult<Coupon>> Query(CouponQueryArgument Arg)
        {
            return Setting.CouponManager.Value.QueryAsync(new Managements.CouponQueryArgument
            {
                LogicState = Arg.LogicState,
                OwnerId = EnsureUserIdent(),
                Paging = Arg.Paging,
                TemplateId=Arg.TemplateId
            });
        }

        public Task<QueryResult<CouponRewardRecord>> QueryRewardRecord(CouponRewardRecordQueryArgument Arg)
        {
            return Setting.CouponRewardRecordManager.Value.QueryAsync(new Managements.CouponRewardRecordQueryArgument
            {
                UserId = EnsureUserIdent(),
                Paging = Arg.Paging,
                TemplateId = Arg.TemplateId,
                CouponId=Arg.CouponId
            });
        }

        public Task<QueryResult<CouponUseRecord>> QueryUseRecord(CouponUseRecordQueryArgument Arg)
        {
            return Setting.CouponUseRecordManager.Value.QueryAsync(new Managements.CouponUseRecordQueryArgument
            {
                UserId = EnsureUserIdent(),
                Paging = Arg.Paging,
                TemplateId = Arg.TemplateId,
                CouponId = Arg.CouponId
            });
        }
    }
}
