
using SF.Biz.Products;
using SF.Biz.Products.Entity;
using SF.Core.Hosting;
using SF.Core.ServiceManagement.Management;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Core.ServiceManagement
{
	public static class ProductDIExtension
	{
		public static IServiceCollection AddProductServices(this IServiceCollection Services,string TablePrifex=null)
		{
			Services.EntityServices(
				"Product",
				"产品管理",
				b => b.Add<IProductManager, ProductManager>("Product", "产品")
					.Add<IProductTypeManager, ProductTypeManager>("ProductType", "产品类型")
					.Add<ICategoryManager, CategoryManager>("ProductItemCatgeory", "商品目录")
					.Add<IItemManager, ItemManager>("ProductItem", "商品")
				);
			Services.AddSingleton<IItemService, CacheItemService>();
			Services.AddSingleton<IItemSource, ItemSource>();
			Services.AddSingleton<IItemNotifier>(sp => (IItemNotifier)sp.Resolve<IItemService>());

			Services.AddDataModules(
				TablePrifex,
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

			return Services;
		}
		public static IServiceInstanceInitializer<IProductManager> NewProductManager(this IServiceInstanceManager sim)
		{
			return sim.DefaultService<IProductManager, ProductManager>(new { });
		}
		public static IServiceInstanceInitializer<IProductTypeManager> NewProductTypeManager(this IServiceInstanceManager sim)
		{
			return sim.DefaultService<IProductTypeManager, ProductTypeManager>(new { });
		}
		public static IServiceInstanceInitializer<ICategoryManager> NewProductCategoryManager(this IServiceInstanceManager sim)
		{
			return sim.DefaultService<ICategoryManager, CategoryManager>(new { });
		}
		public static IServiceInstanceInitializer<IItemManager> NewProductItemManager(
			this IServiceInstanceManager sim
			)
		{
			return sim.DefaultService<IItemManager, ItemManager>(new { });
		}


	}
}
