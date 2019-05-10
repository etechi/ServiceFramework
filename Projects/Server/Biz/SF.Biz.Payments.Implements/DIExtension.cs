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

using SF.Sys.Entities;
using SF.Sys.BackEndConsole;
using SF.Biz.Payments.Managers;
using SF.Biz.Payments;

namespace SF.Sys.Services
{
    public static class PaymentsDIExtension
		
	{
		public static IServiceCollection AddPaymentServices(this IServiceCollection sc, string TablePrefix = null)
		{
			//支付
			sc.EntityServices(
				"Payment",
				"支付管理",
				d => d.Add<ICollectRecordManager, CollectRecordManager>("CollectRecord", "收款记录", typeof(CollectRecord))
					.Add<IRefundRecordManager, RefundRecordManager>("RefundRecord", "退款记录", typeof(RefundRecord))
				);

			sc.AddManagedScoped<ICollectService, CollectService>();
            sc.AddManagedScoped<IRefundService, RefundService>();
            sc.AddManagedScoped<IPaymentPlatformService, PaymentPlatformService>();


            sc.AddDataModules<
				SF.Biz.Payments.DataModels.DataCollectRecord,
                SF.Biz.Payments.DataModels.DataRefundRecord
                >(TablePrefix ?? "Biz");

			//sc.AddAutoEntityTest(NewDocumentManager);
			//sc.AddAutoEntityTest(NewDocumentCategoryManager);
			sc.InitServices("Payments", async (sp, sim, parent) =>
			 {
				 await sim.DefaultService<ICollectRecordManager, CollectRecordManager>(null)
					.WithConsolePages("支付管理/收款记录管理")
					.Ensure(sp, parent);

				 await sim.DefaultService<IRefundRecordManager, RefundRecordManager>(null)
					.WithConsolePages("支付管理/退款记录管理")
					.Ensure(sp, parent);

			 });


			return sc;
		}
	}
}
