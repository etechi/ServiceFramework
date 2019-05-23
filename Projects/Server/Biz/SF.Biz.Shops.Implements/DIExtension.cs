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

using SF.Biz.Shops;
using SF.Biz.Shops.Managements;
using SF.Sys.BackEndConsole;

namespace SF.Sys.Services
{
    public static class ShopsDIExtension
		
	{
		public static IServiceCollection AddShopServices(this IServiceCollection sc, string TablePrefix = null)
		{
            
            sc.EntityServices(
				"Shop",
                "店铺管理",
				d => d.Add<IShopManager, ShopManager>("Shop", "店铺", typeof(SF.Biz.Shops.Managements.ShopInternal))
                    .Add<IShopTypeManager, ShopTypeManager>("ShopType", "店铺类型", typeof(SF.Biz.Shops.Managements.ShopTypeInternal))

                );

			sc.AddManagedScoped<IShopService, ShopService>();

            sc.AddDataModules<
				SF.Biz.Shops.DataModels.DataShop,
                SF.Biz.Shops.DataModels.DataShopType
                > (TablePrefix ?? "Biz");

			sc.InitServices("Shop", async (sp, sim, parent) =>
			 {
				 await sim.DefaultService<IShopManager, ShopManager>(null)
					.WithConsolePages("店铺管理/店铺管理")
					.Ensure(sp, parent);

                 await sim.DefaultService<IShopTypeManager, ShopTypeManager>(null)
                    .WithConsolePages("店铺管理/店铺管理")
                    .Ensure(sp, parent);
                 await sim.DefaultService<IShopService, ShopService>(null)
                     .Ensure(sp, parent);
             });


			return sc;
		}
	}
}
