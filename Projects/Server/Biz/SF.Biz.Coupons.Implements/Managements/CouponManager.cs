#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0


using SF.Sys;
using SF.Sys.Collections.Generic;
using SF.Sys.Data;
using SF.Sys.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Biz.Coupons.Managements
{

    public class CouponManager :
        AutoQueryableEntitySource<ObjectKey<long>, Coupon, Coupon, CouponQueryArgument, DataModels.DataCoupon>,
        ICouponManager
    {
        public CouponManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
        {
        }
        
        Task<Coupon[]> PrepareCoupons(bool ClearEmptyCoupon, params Coupon[] Coupons)
        {
            if (Coupons.Length == 0)
                return Task.FromResult(Coupons);
            var now = Now;

            return DataScope.Use("查询优惠券获取记录", async ctx =>
            {
                var rq = ctx.Queryable<DataModels.DataCouponRewardRecord>();
                if (Coupons.Length == 1)
                {
                    var cid = Coupons[0].Id;
                    rq = rq.Where(rr => rr.CouponId == cid);
                }
                else
                {
                    var oids = Coupons.Select(c => c.OwnerId).Distinct().ToArray();
                    if (oids.Length == 1)
                    {
                        var oid = oids[0];
                        rq = rq.Where(rr => rr.UserId == oid);
                    }
                    else
                    {
                        rq = rq.Where(rr => rr.UserId.HasValue && oids.Contains(rr.UserId.Value));
                    }
                }
                rq = CouponService.FilterValidCoupon(rq, now);
                var dict = await rq
                    .GroupBy(rr => rr.CouponId)
                    .ToDictionaryAsync(
                        g => g.Key,
                        g => new
                        {
                            count = g.Sum(rr => rr.Count),
                            left = g.Sum(rr => rr.LeftCount),
                            expire = g.Min(rr => rr.ExpiresTime),
                            begin = g.Min(rr => rr.BeginTime)
                        }
                        );
                foreach (var c in Coupons)
                {
                    var g = dict.Get(c.Id);
                    if (g == null)
                        continue;
                    c.Count = g.count;
                    c.LeftCount = g.left;
                    c.ExpireTime = g.expire;
                    c.BeginTime = g.begin;
                }
                if (ClearEmptyCoupon)
                    return Coupons.Where(c => c.LeftCount > 0).ToArray();
                else
                    return Coupons;
            });
        }
        public override async Task<QueryResult<Coupon>> QueryAsync(CouponQueryArgument Arg)
        {
            var re=await base.QueryAsync(Arg);
            re.Items = await PrepareCoupons(Arg.ClearEmptyCoupons ?? false, re.Items.ToArray());
            return re;
        }
        public override async Task<Coupon> GetAsync(ObjectKey<long> Id, string[] Fields = null)
        {
            var re=await base.GetAsync(Id, Fields);
            if (re == null)
                return re;
            await PrepareCoupons(false, new[] { re });
            return re;
        }
        public override async Task<Coupon[]> BatchGetAsync(ObjectKey<long>[] Ids, string[] Properties)
        {
            var re=await base.BatchGetAsync(Ids, Properties);
            return await PrepareCoupons(false, re);
        }

    }

}
