using SF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Biz.Products
{
	public static class SetupExtension
	{
		public static async Task<ProductTypeEditable> ProductTypeEnsure<TProductTypeManager>(
			this TProductTypeManager ProductTypeManager, 
			ProductTypeEditable e
			)
			where TProductTypeManager:
			IEntityIdentQueryable<long,ProductTypeQueryArgument>,
			IEntityEditableLoader<long, ProductTypeEditable>,
			IEntityCreator<long, ProductTypeEditable>,
			IEntityUpdator<long, ProductTypeEditable>
		{
			return await ProductTypeManager.EnsureEntityEx<TProductTypeManager, ProductTypeEditable,ProductTypeQueryArgument>(
				new ProductTypeQueryArgument
				{
					Name=e.Name
				},
				o =>
				{
					o.ObjectState = e.ObjectState;
					o.Icon = e.Icon;
					o.Image = e.Image;
					o.ProductCount = e.ProductCount;
					o.Name = e.Name;
					o.Title = e.Title;
					o.Order = e.Order;
					o.Unit = e.Unit;
				}
				);
		}


		static void UpdateCategories(IEnumerable<CategoryEditable> cats, Dictionary<string, CategoryEditable> exists, long? parent)
		{
			foreach (var c in cats)
			{
				c.ParentId = parent;
				var e = exists.Get(c.Name);
				if (e == null)
					continue;
				c.Id = e.Id;
				if (c.Children != null)
					UpdateCategories(c.Children.Cast<CategoryEditable>(), exists, c.Id);
			}
		}
		//public static async Task<CategoryEditable[]> ProductCategoryEnsure<TCategoryManager>(
		//		this TCategoryManager CategoryManager,
		//		long SellerId,
		//		CategoryEditable[] cats
		//	)
		//	where TCategoryManager:
		//		IEntityEditableLoader<long,CategoryEditable>,
		//		IEntityIdentQueryable<long, CategoryEditable>,
		//		//IEntityEditableLoader<long, CategoryEditable>
		//{
		//	var exists = (await CategoryManager.QueryIdentsAsync(new CategoryQueryArgument { SellerId = SellerId })).ToDictionary(e => e.Name);
		//	UpdateCategories(cats, exists, null);

		//	var re = await CategoryManager.BatchUpdate(SellerId, cats);
		//	return re;
		//}
		//public static async Task ProductCategorySetItems(
		//	this IDIScope scope,
		//	int CategoryId,
		//	int[] Items
		//	)
		//{
		//	var m = scope.Resolve<IProductCategoryManager>();

		//	await m.UpdateItems(CategoryId, Items);
		//}
		//public static async Task ProductEnable(this IDIScope scope, int ProductId)
		//{
		//	var m = scope.Resolve<IProductManager>();
		//	await m.SetObjectState(ProductId, LogicObjectState.Enabled);
		//}

		//public static async Task<ProductItemEditable> ProductItemEnsure<TProductItemManager>(
		//	this TProductItemManager scope, 
		//	int SellerId, 
		//	int ProductId
		//	) where TProductItemManager : IEntityIdentQueryable<long, ProductInternalQueryArgument>,
		//					IEntityEditableLoader<long, ProductEditable>,
		//					IEntityUpdator<long, ProductItemEditable>,
		//					IEntityCreator<long, ProductEditable>
		//{
		//	var im = scope.Resolve<IItemManager>();

		//	var ctx = scope.Resolve<IDataContext>();
		//	var iid = await ctx.ReadOnly<CrowdMall.DataModels.ProductItem>()
		//		.Where(i => i.ProductId == ProductId && i.SellerId == SellerId)
		//		.Select(i => i.Id)
		//		.FirstOrDefaultAsync();
		//	if (iid == 0)
		//		iid = await im.CreateAsync(new ProductItemEditable
		//		{
		//			ProductId = ProductId,
		//			SellerId = SellerId,
		//		});
		//	return await im.LoadForEditAsync(iid);
		//}
		public static async Task<ProductEditable> ProductEnsure<TProductManager>(
				  this TProductManager ProductManager,
				  ProductEditable e
			) where TProductManager : IEntityIdentQueryable<long, ProductInternalQueryArgument>,
									IEntityEditableLoader<long, ProductEditable>,
								IEntityUpdator<long, ProductEditable>,
								IEntityCreator<long, ProductEditable>
		{
			return await ProductManager.EnsureEntityEx<TProductManager, ProductEditable, ProductInternalQueryArgument>(
				new ProductInternalQueryArgument
				{
					Name = e.Name,
				},
				o =>
				{
					o.Content = e.Content;
					o.Image = e.Image;
					o.IsVirtual = e.IsVirtual;
					o.CouponDisabled = e.CouponDisabled;
					o.MarketPrice = e.MarketPrice;
					o.Name = e.Name;
					o.OwnerUserId = e.OwnerUserId;
					o.Price = e.Price;
					o.PublishedTime = e.PublishedTime;
					o.Title = e.Title;
					o.TypeId = e.TypeId;
				});
		}




		public static async Task<ProductEditable> ProductEnsure<TProductManager>(
				this TProductManager ProductManager,
				long sellerId,
				long type,
				string name,
				decimal marketPrice,
				decimal price,
				int priceUnit,
				string image,
				string[] images,
				string[] contentImages,
				bool isVirtual = false
			)where TProductManager:IEntityIdentQueryable<long,ProductInternalQueryArgument>,
									IEntityEditableLoader<long,ProductEditable>,
								IEntityUpdator<long,ProductEditable>,
								IEntityCreator<long,ProductEditable>
		{
			return await ProductManager.EnsureEntityEx<TProductManager,ProductEditable,ProductInternalQueryArgument>(
				new ProductInternalQueryArgument
				{
					Name = name,
					ProductTypeId = type
				},
				o =>
				{
					o.Name = name;
					o.Title = name;
					o.TypeId = type;
					o.Price = price;
					o.MarketPrice = marketPrice;
					o.IsVirtual = isVirtual;
					o.CouponDisabled = false;
					o.Image = image;
					o.OwnerUserId = sellerId;
					o.Content = new ProductContent
					{
						Images = images?.Select(i => new ProductImage { Image = i }) ?? null,
						Descs = contentImages?.Select(i => new  ProductDescItem { Image = i }) ?? null
					};
					o.PublishedTime = DateTime.Now;
				}
				);
		}
	}
}
