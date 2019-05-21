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

namespace SF.Biz.Coupons.Managements
{

    public class CouponTemplateManager :
        AutoModifiableEntityManager<ObjectKey<long>, CouponTemplate, CouponTemplate, CouponTemplateQueryArgument, CouponTemplate, DataModels.DataCouponTemplate>,
        ICouponTemplateManager
    {
        public CouponTemplateManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
        {
        }
        protected override Task OnUpdateModel(IModifyContext ctx)
        {
            var Model = ctx.Model;
            var e = ctx.Editable;

            if (e.AdjustValue <= 0)
                throw new PublicArgumentException("优惠值必须大于0");
            if (e.Type == CouponType.DiscountCoupon)
            {
                if (e.AdjustValue > 100)
                    throw new PublicArgumentException("折扣券优惠比例不能超过100%");
            }
            if (!e.ExpireDays.HasValue && !e.EndTime.HasValue)
                throw new PublicArgumentException("必须设置过期时间或过期天数");

            return base.OnUpdateModel(ctx);
        }
    }

}
