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


using SF.Biz.Products;
using SF.Biz.Products.Entity;
using SF.Sys.BackEndConsole;
using SF.Sys.Services.Management;

namespace SF.Sys.Services
{
    

    public static class ProductDIExtension
	{
		public static IServiceCollection AddProductServices(this IServiceCollection Services,string TablePrifex=null)
		{
			Services.EntityServices(
				"Product",
				"产品管理",
				b => b.Add<IProductManager, ProductManager>("Product", "产品",typeof(ProductBase))
					.Add<IProductTypeManager, ProductTypeManager>("ProductType", "产品类型",typeof(ProductType))
					.Add<IProductCategoryManager, CategoryManager>("ProductItemCategory", "商品目录",typeof(CategoryInternal))
					.Add<IProductItemManager, ItemManager>("ProductItem", "商品",typeof(Item))
				);
			Services.AddSingleton<IItemService, CacheItemService>();
			Services.AddSingleton<IItemSource, ItemSource>();
			Services.AddSingleton<IItemNotifier>(sp => (IItemNotifier)sp.Resolve<IItemService>());

			Services.AddDataModules(
				TablePrifex ?? "Biz",
				typeof(SF.Biz.Products.Entity.DataModels.Product),
				typeof(SF.Biz.Products.Entity.DataModels.ProductDetail),
				typeof(SF.Biz.Products.Entity.DataModels.ProductType),
				typeof(SF.Biz.Products.Entity.DataModels.Category),
				typeof(SF.Biz.Products.Entity.DataModels.CategoryItem),
				typeof(SF.Biz.Products.Entity.DataModels.PropertyScope),
				typeof(SF.Biz.Products.Entity.DataModels.Property),
				typeof(SF.Biz.Products.Entity.DataModels.PropertyItem),
				typeof(SF.Biz.Products.Entity.DataModels.Item),
				typeof(SF.Biz.Products.Entity.DataModels.ProductSpec)
				);
            Services.InitServices("Products", async (sp, sim, parent) =>
            {
                await sim.DefaultService<IProductManager, ProductManager>(null)
                   .WithConsolePages("业务管理/产品管理")
                   .Ensure(sp, parent);

                await sim.DefaultService<IProductTypeManager, ProductTypeManager>(null)
                   .WithConsolePages("业务内容/工单管理")
                   .Ensure(sp, parent);

                await sim.DefaultService<IProductCategoryManager, CategoryManager>(null)
                   .WithConsolePages("前端内容/工单管理")
                   .Ensure(sp, parent);

                await sim.DefaultService<IProductItemManager, ItemManager>(null)
                   .WithConsolePages("前端内容/工单管理")
                   .Ensure(sp, parent);
            });
            Services.AddInitializer("Products", "产品数据", async sp => {
                

            });
            return Services;
		}
		
	}
}
