using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Biz.Coupons;
using SF.Sys;
using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys.TimeServices;

namespace SF.Biz.Coupons
{
    public class CouponServiceSetting
    {
       
        ///<title>最大折扣百分比</title>
        /// <summary>
        /// 不填或为0时不限最大折扣率。最大促销比例是指能优惠的比例, 自动四舍五入，比如： 100元产品，最大促销比例是36.6,那么用户最低能买到的价格是63元。
        /// </summary>
        public double MaxDiscountPercent { get; set; } = 80;

        public IDataScope DataScope { get; set; }
        public Lazy<IIdentGenerator> IdentGenerator { get; set; }
        public Lazy<ITimeService> TimeService { get; set; }
    }
    public class CouponService : ICouponService
    {
        CouponServiceSetting Setting { get; }
        public CouponService(CouponServiceSetting Setting)
        {
            this.Setting = Setting;
            
        }
        public double MaxDiscountPercent => Setting.MaxDiscountPercent;


        public class CouponDataConfig : ICouponDiscountConfig
        {
            public DataModels.DataCoupon Coupon { get; set; }
            public int Count { get; set; }
            public int LeftCount { get; set; }

            public decimal ConditionValue => Coupon.ConditionValue;
            public decimal AdjustValue => Coupon.AdjustValue;

            public CouponType Type => Coupon.Type;
            public string Name => Coupon.Name;
        }


        public Task<ICouponDiscountConfig> GetCouponDiscountConfig(CouponApplyArgument Arg)
        {
            return GetCouponDiscountConfig(Arg.UserId, Arg.Code, Arg.Count, Arg.OrgAmount);
        }
        public Task<ICouponDiscountConfig> GetCouponDiscountConfig(long OwnerId, string Code, int Count, decimal OrgAmount)
        {
            return Setting.DataScope.Use("获取则扣配置", async ctx =>
            {
                long id;
                var re = long.TryParse(Code, out id) ?
                    (await ctx.Queryable<DataModels.DataCoupon>()
                        .Where(c => c.Id == id && c.OwnerId == OwnerId)
                        .Select(c => new
                        {
                            TmplObjectState = c.Template.LogicState,
                            coupon = c
                        }).SingleOrDefaultAsync()) :
                        null;
                if (re == null)
                    throw new CouponException(CouponErrorCode.Missing, "找不到红包");

                if (re.TmplObjectState != EntityLogicState.Enabled)
                    throw new CouponException(CouponErrorCode.Invalid, "红包无效，不能使用");

                var coupon = re.coupon;
                var cid = coupon.Id;
                var Status = await FilterValidCoupon(
                        ctx.Queryable<DataModels.DataCouponRewardRecord>()
                        .Where(rr => rr.CouponId == cid),
                        Setting.TimeService.Value.Now)
                    .GroupBy(rr => rr.CouponId)
                    .Select(g => new
                    {
                        Count = g.Sum(rr => rr.Count),
                        LeftCount = g.Sum(rr => rr.LeftCount)
                    })
                    .SingleOrDefaultAsync();

                if (Status.LeftCount < Count)
                    throw new CouponException(CouponErrorCode.Used, "红包数量不足");

                if (Count > 1 && coupon.Type == CouponType.DiscountCoupon)
                    throw new CouponException(CouponErrorCode.ConditionCheckFailed, "折扣红包不能重叠使用");

                if (coupon.ConditionValue > OrgAmount)
                    throw new CouponException(CouponErrorCode.ConditionCheckFailed, "红包使用条件不满足");
                return (ICouponDiscountConfig)new CouponDataConfig
                {
                    LeftCount = Status.LeftCount,
                    Count = Status.Count,
                    Coupon = coupon
                };
            });
        }




        public Task<long> Create(CouponCreateArgument Arg)
        {
            if (Arg.Count == 0)
                throw new ArgumentException("红包数量为0");
            return Setting.DataScope.Use("发放优惠券", async ctx =>
            {
                var tmpl = await ctx.Queryable<DataModels.DataCouponTemplate>()
                .Where(t => t.Id == Arg.TemplateId && t.LogicState == EntityLogicState.Enabled)
                .Select(t => new
                {
                    ExpireDays = t.ExpireDays,
                    EndTime = t.EndTime,
                    BeginTime = t.BeginTime,
                    Type = t.Type,
                    ConditionValue = t.ConditionValue,
                    AdjustValue = t.AdjustValue,
                    TargetEntityIdent = t.TargetEntityIdent,
                    //EndTime=t.EndTime
                }).SingleOrDefaultAsync();
                if (tmpl == null)
                    throw new InvalidOperationException("找不到红包模板或模板无效：" + Arg.TemplateId);
                var now = Arg.Time;
                //var exptime = tmpl.ExpireDays == 0 ? null : (DateTime?)now.AddDays(tmpl.ExpireDays);
                var maxExpTime = now.AddYears(1);
                var tmplEndTime = tmpl.EndTime ?? maxExpTime;
                var tmplExpTime = tmpl.ExpireDays.HasValue ? now.AddDays(tmpl.ExpireDays.Value) : maxExpTime;
                var expTime = tmplExpTime < tmplEndTime ? tmplExpTime : tmplEndTime;
                var beginTime = tmpl.BeginTime;

                var coupon = await ctx.Queryable<DataModels.DataCoupon>(false).Where(c =>
                    c.TemplateId == Arg.TemplateId &&
                    c.OwnerId == Arg.OwnerId &&
                    c.Type == tmpl.Type &&
                    c.ConditionValue == tmpl.ConditionValue &&
                    c.AdjustValue == tmpl.AdjustValue
                ).SingleOrDefaultAsync();

                var rid = await Setting.IdentGenerator.Value.GenerateAsync<DataModels.DataCouponRewardRecord>();
                var cid = coupon?.Id ?? await Setting.IdentGenerator.Value.GenerateAsync<DataModels.DataCoupon>();

                var record = ctx.Add(new DataModels.DataCouponRewardRecord
                {
                    Id = rid,
                    UserId = Arg.OwnerId,
                    TemplateId = Arg.TemplateId,
                    Time = now,
                    ExpiresTime = expTime,
                    BeginTime = beginTime,
                    ValidTime = expTime,
                    LeftCount = Arg.Count,
                    Count = Arg.Count,
                    SrcDesc = Arg.SrcDesc,
                    SrcEntityIdent = Arg.SrcEntityId,
                    CouponId = cid,
                });

                if (coupon == null)
                    coupon = ctx.Add(new DataModels.DataCoupon
                    {
                        Id = cid,
                        OwnerId = Arg.OwnerId,
                        TemplateId = Arg.TemplateId,
                        CreatedTime = now,

                        Type = tmpl.Type,
                        ConditionValue = tmpl.ConditionValue,
                        AdjustValue = tmpl.AdjustValue,
                        UpdatedTime = now
                    });
                else
                    ctx.Update(coupon);

                coupon.LastRewardCount = Arg.Count;
                coupon.LastRewardTime = now;

                await ctx.SaveChangesAsync();
                return coupon.Id;
            });
        }

        internal static IQueryable<DataModels.DataCouponRewardRecord> FilterValidCoupon(
            IQueryable<DataModels.DataCouponRewardRecord> q,
            DateTime now
            )
        {
            return q.Where(rr =>
                rr.Template.LogicState == EntityLogicState.Enabled &&
                rr.ValidTime > now &&
                (rr.BeginTime == null || rr.BeginTime.Value < now)
                );
        }
        public Task<int> GetTotalCount(long OwnerId, long? TemplateId)
        {
            return Setting.DataScope.Use("获取可用优惠券数量", async ctx =>
            {
                var now = Setting.TimeService.Value.Now;
                var q = ctx.Queryable<DataModels.DataCouponRewardRecord>()
                     .Where(rr => rr.UserId == OwnerId);
                if (TemplateId.HasValue)
                    q = q.Where(rr => rr.TemplateId == TemplateId.Value);

                var re = await FilterValidCoupon(q, now)
                     .Select(rr => rr.LeftCount)
                     .DefaultIfEmpty()
                     .SumAsync();
                return re;
            });
        }


        public async Task<decimal> Apply(CouponApplyArgument Arg)
        {
            var cfg = await GetCouponDiscountConfig(Arg.UserId, Arg.Code, Arg.Count, Arg.OrgAmount);
            return await Apply(cfg, Arg);
        }
        public Task<decimal> Apply(ICouponDiscountConfig cfg, CouponApplyArgument Arg)
        {
            return Setting.DataScope.Use("使用优惠券", async ctx => {
                var now = Arg.Time;
                var coupon = ((CouponDataConfig)cfg).Coupon;

                var newAmount = cfg.CalcNewAmount(
                    Arg.OrgAmount,
                    Arg.Count,
                    Setting.MaxDiscountPercent
                    ).NewAmount;


                coupon.LastUseTime = now;
                coupon.LastUseCount = Arg.Count;
                coupon.TotalUsedCount += Arg.Count;

                var rewardRecords = await FilterValidCoupon(
                        ctx.Queryable<DataModels.DataCouponRewardRecord>()
                        .Where(rr => rr.CouponId == coupon.Id),
                        now)
                    .OrderBy(rr => rr.ValidTime)
                        .ToArrayAsync();

                var left = Arg.Count;

                foreach (var rr in rewardRecords)
                {
                    if (rr.LeftCount > left)
                    {
                        rr.LeftCount -= left;
                        rr.LastUsedCount = left;
                        rr.LastUsedTime = now;
                        ctx.Update(rr);
                        left = 0;
                        break;
                    }
                    else
                    {
                        left -= rr.LeftCount;
                        rr.LastUsedCount = rr.LeftCount;
                        rr.LeftCount = 0;
                        rr.ValidTime = now;
                        rr.LastUsedTime = now;
                        ctx.Update(rr);
                    }
                }
                if (left > 0)
                    throw new PublicArgumentException("红包数量不足");

                ctx.Update(coupon);

                ctx.Add(new DataModels.DataCouponUseRecord
                {
                    Id = await Setting.IdentGenerator.Value.GenerateAsync<DataModels.DataCouponUseRecord>(),
                    UserId = Arg.UserId,
                    CouponId = coupon.Id,
                    TemplateId = coupon.TemplateId,
                    Time = now,
                    Count = Arg.Count,
                    DstDesc = Arg.DstDesc.Limit(100),
                    DstEntityIdent = Arg.DstEntityId
                });

                await ctx.SaveChangesAsync();
                return newAmount;
            });
        }
    }
}
