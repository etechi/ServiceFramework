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

using SF.Biz.Coupons;
using SF.Biz.Coupons.Managements;
using SF.Sys.BackEndConsole;
using SF.Sys.Collections.Generic;
using SF.Sys.Data;

namespace SF.Sys.Services
{
    public static class CouponsDIExtension
		
	{
		public static IServiceCollection AddCouponsServices(this IServiceCollection sc, string TablePrefix = null)
		{
            sc.EntityServices(
				"Coupons",
				"优惠券管理",
				d => d.Add<ICouponManager, CouponManager>("Coupon", "优惠券", typeof(Coupon))
                    .Add<ICouponRewardRecordManager, CouponRewardRecordManager>("CouponRewardRecord", "优惠券领取记录", typeof(CouponRewardRecord))
                    .Add<ICouponUseRecordManager, CouponUseRecordManager>("CouponUseRecord", "优惠券使用记录", typeof(CouponUseRecord))
                    .Add<ICouponTemplateManager, CouponTemplateManager>("CouponTemplate", "优惠券模板", typeof(CouponTemplate))
                );

			sc.AddManagedScoped<IUserCouponService, UserCouponService>();
            sc.AddManagedScoped<ICouponService, CouponService>();

            sc.AddDataModules<
				SF.Biz.Coupons.DataModels.DataCoupon,
                SF.Biz.Coupons.DataModels.DataCouponRewardRecord,
                SF.Biz.Coupons.DataModels.DataCouponTemplate,
                SF.Biz.Coupons.DataModels.DataCouponUseRecord
                >(TablePrefix ?? "Biz");

			sc.InitServices("Coupons", async (sp, sim, parent) =>
			 {
				 await sim.DefaultService<ICouponManager, CouponManager>(null)
					.WithConsolePages("促销管理/优惠券管理")
					.Ensure(sp, parent);

                 await sim.DefaultService<ICouponTemplateManager, CouponTemplateManager>(null)
                    .WithConsolePages("促销管理/优惠券管理")
                    .Ensure(sp, parent);

                 await sim.DefaultService<ICouponRewardRecordManager, CouponRewardRecordManager>(null)
                    .WithConsolePages("促销管理/优惠券管理")
                    .Ensure(sp, parent);

                 await sim.DefaultService<ICouponUseRecordManager, CouponUseRecordManager>(null)
                    .WithConsolePages("促销管理/优惠券管理")
                    .Ensure(sp, parent);

                 await sim.DefaultService<IUserCouponService, UserCouponService>(new { Setting=new UserCouponServiceSetting { } })
                    .Ensure(sp, parent);

                 await sim.DefaultService<ICouponService, CouponService>(new { Setting = new CouponServiceSetting { } })
                    .Ensure(sp, parent);
            });

			return sc;
		}
	}
}
