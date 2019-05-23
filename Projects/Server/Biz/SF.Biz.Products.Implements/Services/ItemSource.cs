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

using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Sys.Services;
using SF.Sys.Annotations;
using SF.Sys.Events;
using SF.Sys.Services.Internals;
using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys;

namespace SF.Biz.Products.Entity
{
	
	public class ItemSource:IItemSource
    {
        public  long MainCategoryId { get; }

		static async Task<T> NewDataContext<T>(IServiceScopeFactory ScopeFactory,Func<IDataContext,Task<T>> Callback)
		{
			using(var s = ScopeFactory.CreateServiceScope())
			{
				return await Callback(s.ServiceProvider.Resolve<IDataContext>());
			}
		}
		class CategoryChildrenLoaderInstance : IRelationLoader
		{
			ItemSource source { get; }
			public CategoryChildrenLoaderInstance(ItemSource source)
			{
				this.source = source;
			}

			public async Task<long[]> Load(long Id)
			{
                return await source.DataScope.Use("载入子目录商品", async ctx =>
                {
					return await ctx.Set<DataModels.DataCategory>().AsQueryable()
						.Where(c => c.ParentId == Id)
						.Select(c => c.Id)
						.ToArrayAsync();
				});
			}
		}
		class CategoryItemsLoaderInstance : IRelationLoader
		{
			ItemSource source { get; }
			public CategoryItemsLoaderInstance(ItemSource source)
			{
				this.source = source;
			}

			public async Task<long[]> Load(long Id)
			{
                return await source.DataScope.Use("载入目录商品", async ctx =>
                {
					return await ctx.Set<DataModels.DataCategoryItem>()
						.AsQueryable()
						.Where(c => c.CategoryId == Id)
						.OrderBy(c => c.Order)
						.Select(c => c.ItemId)
						.ToArrayAsync();
				});
			}
		}
		class CategoryLoaderInstance : IBatchLoader<ICategoryCached>
		{
			ItemSource source { get; }
			public CategoryLoaderInstance(ItemSource source)
			{
				this.source = source;
			}

			public async Task<ICategoryCached[]> Load(long[] Ids)
			{
                return await source.DataScope.Use("载入目录", async ctx =>
                {
					return await source.MapModelToPublic(
						ctx.Set<DataModels.DataCategory>().AsQueryable().Where(c => Ids.Contains(c.Id))
						).ToArrayAsync();
				});

			}
		}
        class TaggedCategoryLoaderInstance : IRelationLoader<string>
        {
            ItemSource source { get; }
            public TaggedCategoryLoaderInstance(ItemSource source)
            {
                this.source = source;
            }

            public async Task<long[]> Load(string tag)
            {
                return await source.DataScope.Use("载入标签目录", async ctx =>
                {
					return await ctx.Set<DataModels.DataCategory>()
						.AsQueryable()
						.Where(c =>
							c.Tag.Contains(tag) &&
							c.LogicState == EntityLogicState.Enabled
							)
						.OrderBy(c => c.Order)
						.Select(c => c.Id)
						.ToArrayAsync();
				});
            }
        }
        class ItemLoaderInstance : IBatchLoader<IItemCached>
		{
			ItemSource source { get; }
			public ItemLoaderInstance(ItemSource source)
			{
				this.source = source;
			}

			public async Task<IItemCached[]> Load(long[] Ids)
			{
                return await source.DataScope.Use("载入商品", async ctx =>
                {
					var re = await source.MapModelToPublic(
						ctx.Set<DataModels.DataItem>().AsQueryable().Where(c => Ids.Contains(c.Id))
						).ToArrayAsync();
					return re.ToArray();
				});

			}
		}
		class ProductLoaderInstance : IBatchLoader<IProductCached>
		{
			ItemSource source { get; }
			public ProductLoaderInstance(ItemSource source)
			{
				this.source = source;
			}

			public async Task<IProductCached[]> Load(long[] Ids)
			{
                return await source.DataScope.Use("载入产品", async ctx =>
                {
					var re = await source.MapModelToPublic(
						ctx.Set<DataModels.DataProduct>().AsQueryable().Where(c => Ids.Contains(c.Id))
						).ToArrayAsync();
					return re.Select(r => r.Build()).ToArray();
				});

			}
		}
		public IDataScope DataScope { get; }
		public IRelationLoader CategoryChildrenLoader { get; }
		public IRelationLoader CategoryItemsLoader { get; }
		public IBatchLoader<ICategoryCached> CategoryLoader { get; }
		public IBatchLoader<IItemCached> ItemLoader { get; }
		public IBatchLoader<IProductCached> ProductLoader { get; }

        public IRelationLoader<string> TaggedCategoryLoader { get; }

       
		protected virtual IQueryable<CategoryCached> MapModelToPublic(IQueryable<DataModels.DataCategory> query)
		{
            return from c in query
                   select new CategoryCached
                   {
                       Id = c.Id,
                       Tag = c.Tag,
                       Title = c.Title,
                       Description = c.Description,
                       Image = c.Image,
                       Icon = c.Icon,
                        BannerUrl = c.BannerUrl,
                        BannerImage = c.BannerImage,
                        MobileBannerImage=c.MobileBannerImage,
                        MobileBannerUrl=c.MobileBannerUrl,

						Order =c.Order,
						ItemCount =c.ItemCount
					};
		}
		
		public class ProductResult
		{
			public ProductCached Product { get; set; }
			public string Tags { get; set; }
            public string Detail { get; set; }
            public string Images { get; set; }
            public virtual IProductCached Build()
			{
				Product.Tags= Tags?.Split(',');
                Product.Images = Json.Parse<ProductImage[]>(Images);
                Product.Descs = Json.Parse<ProductDescItem[]>(Images);
                return Product;
			}
		}


		protected virtual IQueryable<ProductResult> MapModelToPublic(IQueryable<DataModels.DataProduct> query)
		{
			return
				from p in query
				select new ProductResult
				{
					Product = new ProductCached
					{
						Id = p.Id,
                        IsVirtual=p.IsVirtual,
                        CouponDisabled=p.CouponDisabled,
						Title = p.Title,
                        Name=p.Name,
						MarketPrice = p.MarketPrice,
						Price = p.Price,
						Image = p.Image,
						PublishedTime = p.PublishedTime,
						Visited = p.Visited,
						MainItemId = p.Items.Where(it => it.SellerId == p.OwnerId).Select(it => it.Id).FirstOrDefault(),
						OnSale=p.LogicState==EntityLogicState.Enabled,
                        DeliveryProvider=p.Type.DeliveryProvider
					},
                    Detail=p.Detail,
                    Images=p.Images
				};
		}

		protected virtual IQueryable<ItemCached> MapModelToPublic(IQueryable<DataModels.DataItem> query)
		{
			return
				from item in query
				select new ItemCached
				{
					Id = item.Id,
					SellerId = item.SellerId,
					ProductId = item.ProductId
				};
		}
		public ItemSource(
			IDataScope DataScope,
			[EntityIdent(typeof(CategoryInternal))]
			long MainCategoryId,
			IEventSubscriber<ServiceInstanceChanged> ServiceInstanceChanged
			)
		{
			this.MainCategoryId = MainCategoryId;

			this.DataScope = DataScope;
			this.CategoryChildrenLoader = new CategoryChildrenLoaderInstance((ItemSource)this);
			this.CategoryItemsLoader = new CategoryItemsLoaderInstance((ItemSource)this);
			this.CategoryLoader = new CategoryLoaderInstance((ItemSource)this);
			this.ItemLoader = new ItemLoaderInstance((ItemSource)this);
			this.ProductLoader= new ProductLoaderInstance((ItemSource)this);
            this.TaggedCategoryLoader=new TaggedCategoryLoaderInstance((ItemSource)this); 
        }

	}
}
