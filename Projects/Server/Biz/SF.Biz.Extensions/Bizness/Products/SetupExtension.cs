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
			IEntityIdentQueryable<ObjectKey<long>, ProductTypeQueryArgument>,
			IEntityEditableLoader<ObjectKey<long>,ProductTypeEditable>,
			IEntityCreator<ObjectKey<long>, ProductTypeEditable>,
			IEntityUpdator<ProductTypeEditable>
		{
			return await ProductTypeManager.EnsureEntity(
				await ProductTypeManager.QuerySingleEntityIdent(new ProductTypeQueryArgument{Name=e.Name}),
				(ProductTypeEditable o) =>
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


		static void UpdateCategories(IEnumerable<CategoryInternal> cats, Dictionary<string, CategoryInternal> exists, long? parent)
		{
			foreach (var c in cats)
			{
				c.ParentId = parent;
				var e = exists.Get(c.Name);
				if (e == null)
					continue;
				c.Id = e.Id;
				if (c.Children != null)
					UpdateCategories(c.Children.Cast<CategoryInternal>(), exists, c.Id);
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
		public static async Task SetItems<TEditable>(
			this ICategoryManager<TEditable> CategoryManager,
			long CategoryId,
			long[] Items
			) where TEditable:CategoryInternal
		{
			await CategoryManager.UpdateEntity< ObjectKey<long>,TEditable>(
				CategoryManager,
				ObjectKey.From(CategoryId),
				(TEditable e) =>
				{
					e.Items = Items;
				});
		}
		public static async Task ProductEnable(this IProductManager ProductManager, long ProductId)
		{
			await ProductManager.SetLogicState(ProductId, EntityLogicState.Enabled);
		}

		public static async Task<ItemEditable> ItemEnsure<TProductItemManager>(
			this TProductItemManager Manager,
			long SellerId,
			long ProductId
			) where TProductItemManager :
				IEntityIdentQueryable<ObjectKey<long>, ItemQueryArgument>,
				IEntityEditableLoader<ObjectKey<long>,ItemEditable>,
				IEntityUpdator<ItemEditable>,
				IEntityCreator<ObjectKey<long>,ItemEditable>
		{

			var re=await Manager.QueryIdentsAsync(new ItemQueryArgument
			{
				ProductId = ProductId,
				SellerId = SellerId
			}, Paging.Single
			);
			var id = re.Items.SingleOrDefault();
			if (id== null)
				id = await Manager.CreateAsync(new ItemEditable
				{
					ProductId = ProductId,
					SellerId = SellerId
				});
			return await Manager.LoadForEdit(id);
		}
		public static async Task<ProductEditable> ProductEnsure<TProductManager>(
				  this TProductManager ProductManager,
				  ProductEditable e
			) where TProductManager : IEntityIdentQueryable<ObjectKey<long>, ProductInternalQueryArgument>,
									IEntityEditableLoader<ObjectKey<long>,ProductEditable>,
								IEntityUpdator< ProductEditable>,
								IEntityCreator<ObjectKey<long>,ProductEditable>
		{
			return await ProductManager.EnsureEntity(
				await ProductManager.QuerySingleEntityIdent(new ProductInternalQueryArgument{Name = e.Name}),
				(ProductEditable o) =>
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
			)where TProductManager:IEntityIdentQueryable<ObjectKey<long>,ProductInternalQueryArgument>,
									IEntityEditableLoader<ObjectKey<long>,ProductEditable>,
								IEntityUpdator<ProductEditable>,
								IEntityCreator<ObjectKey<long>,ProductEditable>
		{
			return await ProductManager.EnsureEntity(
				await ProductManager.QuerySingleEntityIdent(new ProductInternalQueryArgument{Name = name,ProductTypeId = type}),
				(ProductEditable o) =>
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
