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
using SF.Sys.Entities;
using System.Threading.Tasks;
using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Biz.Coupons.DataModels;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SF.Biz.Coupons.Managements
{

   
    public class CouponUseRecordManager :
        AutoQueryableEntitySource<ObjectKey<long>, CouponUseRecord, CouponUseRecord, CouponUseRecordQueryArgument, DataModels.DataCouponUseRecord>,
        ICouponUseRecordManager
    {
        public CouponUseRecordManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
        {
        }
        class CouponUseSummary : ISummaryWithCount
        {
            public int Count { get; set; }
            public int 使用数量 { get; set; }
            public decimal 抵价券优惠额 { get; set; }
        }
        protected override Expression<Func<IGrouping<int, DataCouponUseRecord>, ISummaryWithCount>> GetSummaryExpression()
        {
            return g => new CouponUseSummary
            {
                Count = g.Count(),
                使用数量 = g.Select(i => i.Count).DefaultIfEmpty().Sum(),
                抵价券优惠额 = g.
                        Where(i => i.Template.Type == CouponType.DecreaseCoupon)
                        .Select(i => i.Coupon.AdjustValue * i.Count)
                        .DefaultIfEmpty()
                        .Sum()
            };
        }



    }

}
