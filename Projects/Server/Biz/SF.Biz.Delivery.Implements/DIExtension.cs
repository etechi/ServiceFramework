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

using SF.Biz.Delivery;
using SF.Biz.Delivery.Init;
using SF.Biz.Delivery.Management;
using SF.Biz.Trades;
using SF.Sys.BackEndConsole;
using SF.Sys.Collections.Generic;
using SF.Sys.Data;

namespace SF.Sys.Services
{
    public static class DeliveryDIExtension
		
	{
		public static IServiceCollection AddDeliveryServices(this IServiceCollection sc, string TablePrefix = null)
		{
            //发货
            sc.EntityServices(
				"Delivery",
				"发货管理",
				d => d.Add<IDeliveryManager, DeliveryManager>("Delivery", "发货", typeof(DeliveryInternal))
                    .Add<IDeliveryTransportManager, DeliveryTransportManager>("DeliveryTransport", "快递公司", typeof(DeliveryTransport))
                );

            sc.AddTransient(sp => (ITradeDeliveryProvider)sp.Resolve<IDeliveryManager>(), "实物");
            sc.AddDataModules<
				SF.Biz.Delivery.DataModels.DataDelivery,
                SF.Biz.Delivery.DataModels.DataDeliveryItem,
                SF.Biz.Delivery.DataModels.DataDeliveryPrice,
                SF.Biz.Delivery.DataModels.DataDeliveryTransport
                >(TablePrefix ?? "Biz");

			sc.InitServices("Delivery", async (sp, sim, parent) =>
			 {
				 await sim.DefaultService<IDeliveryManager, DeliveryManager>(null)
					.WithConsolePages("发货管理/发货管理")
					.Ensure(sp, parent);


                 await sim.DefaultService<IDeliveryTransportManager, DeliveryTransportManager>(null)
                    .WithConsolePages("发货管理/快递公司管理")
                    .Ensure(sp, parent);
            });

            sc.AddInitializer("data", "delivery", async (sp,args) =>
            {
                var useTransaction = args.Get("disableTransaction") != "true";
                var testMode = args.Get("unittest") == "true";
                if (testMode)
                    useTransaction = false;

                await sp.EnsureTransport(useTransaction);
            });

			return sc;
		}
	}
}
