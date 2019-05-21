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


using SF.Biz.Coupons.DataModels;
using SF.Sys;
using SF.Sys.Data;
using SF.Sys.Entities;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SF.Biz.Coupons.Managements
{

    public class CouponRewardRecordManager :
        AutoQueryableEntitySource<ObjectKey<long>, CouponRewardRecord, CouponRewardRecord, CouponRewardRecordQueryArgument, DataModels.DataCouponRewardRecord>,
        ICouponRewardRecordManager
    {
        public CouponRewardRecordManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
        {
        }

        protected override IQueryable<DataCouponRewardRecord> OnBuildQuery(IQueryable<DataCouponRewardRecord> Query, CouponRewardRecordQueryArgument Arg)
        {

            if (Arg.State != null)
            {
                var now = Now;

                switch (Arg.State)
                {
                    case CouponRewardState.Expired:
                        Query = Query.Where(c => c.ValidTime < now && c.LeftCount > 0);
                        break;
                    case CouponRewardState.Used:
                        Query = Query.Where(c => c.ValidTime < now && c.LeftCount == 0);

                        if (Arg.Paging != null && Arg.Paging.SortOrder == SortOrder.Default)
                            Arg.Paging.SortOrder = SortOrder.Desc;
                        break;
                    case CouponRewardState.NotReady:
                        Query = Query.Where(c =>
                            c.BeginTime != null && c.BeginTime.Value > now &&
                            c.Template.LogicState == EntityLogicState.Enabled &&
                            c.LeftCount > 0
                            );
                        break;
                    case CouponRewardState.Valid:
                        Query = Query.Where(c =>
                            c.ValidTime > now &&
                            (c.BeginTime == null || c.BeginTime.Value < now) &&
                            c.Template.LogicState == EntityLogicState.Enabled &&
                            c.LeftCount > 0
                            );
                        break;
                    case CouponRewardState.Invalid:
                        Query = Query.Where(c =>
                            c.ValidTime > now &&
                            c.LeftCount > 0 &&
                            c.Template.LogicState != EntityLogicState.Enabled
                            );
                        break;
                }
            }

            return base.OnBuildQuery(Query, Arg);
        }

        class CouponRewardSummary : ISummaryWithCount
        {
            public int Count { get; set; }
            public int 数量 { get; set; }
            public int 剩余 { get; set; }
        }
        protected override Expression<Func<IGrouping<int, DataCouponRewardRecord>, ISummaryWithCount>> GetSummaryExpression()
        {
            return g => new CouponRewardSummary
            {
                Count = g.Count(),
                剩余 = g.Select(i => i.LeftCount).DefaultIfEmpty().Sum(),
                数量 = g.Select(i => i.Count).DefaultIfEmpty().Sum()
            };
        }
    }

}
