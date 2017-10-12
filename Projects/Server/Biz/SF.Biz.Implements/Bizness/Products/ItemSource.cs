using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using SF.Data;
using SF.Core.ServiceManagement;
using SF.Entities;
using SF.Core;
using SF.Core.Events;
using SF.Core.ServiceManagement.Internals;
using SF.Metadata;
namespace SF.Biz.Products.Entity
{
	public class ItemSource : 
		ItemSource<
			ItemSource,
			ItemCached, ProductCached, ProductContentCached, CategoryCached,
			DataModels.Product, DataModels.ProductDetail, DataModels.ProductType, DataModels.Category, DataModels.CategoryItem, DataModels.PropertyScope, DataModels.Property, DataModels.PropertyItem, DataModels.Item, DataModels.ProductSpec
		>,
		IItemSource
	{
		public ItemSource(IServiceScopeFactory ScopeFactory, [EntityIdent(typeof(CategoryInternal), null, 0, null)] long MainCategoryId, IEventSubscriber<ServiceInstanceChanged> ServiceInstanceChanged) : base(ScopeFactory, MainCategoryId, ServiceInstanceChanged)
		{
		}
	}
	public class ItemSource<
		TItemSource,
		TItemCached, TProductCached,TProductContentCached,TCategoryCached,
		 TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec
		> :
		IItemSource
		where TItemSource:ItemSource<
			TItemSource,
			TItemCached, TProductCached, TProductContentCached, TCategoryCached,
			TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec
			>
		where TItemCached:ItemCached,new()
		where TProductCached:ProductCached, new()
		where TProductContentCached:ProductContentCached, new()
		where TCategoryCached:CategoryCached, new()
		where TProduct : DataModels.Product< TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TProductDetail : DataModels.ProductDetail<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TProductType : DataModels.ProductType<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TCategory : DataModels.Category<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TCategoryItem : DataModels.CategoryItem<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TPropertyScope : DataModels.PropertyScope<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TProperty : DataModels.Property<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TPropertyItem : DataModels.PropertyItem<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TItem : DataModels.Item<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
        where TProductSpec : DataModels.ProductSpec<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem, TProductSpec>
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
			TItemSource source { get; }
			public CategoryChildrenLoaderInstance(TItemSource source)
			{
				this.source = source;
			}

			public async Task<long[]> Load(long Id)
			{
				return await NewDataContext(source.ScopeFactory, async ctx =>
				{
					return await ctx.Set<TCategory>().AsQueryable()
						.Where(c => c.ParentId == Id)
						.Select(c => c.Id)
						.ToArrayAsync();
				});
			}
		}
		class CategoryItemsLoaderInstance : IRelationLoader
		{
			TItemSource source { get; }
			public CategoryItemsLoaderInstance(TItemSource source)
			{
				this.source = source;
			}

			public async Task<long[]> Load(long Id)
			{
				return await NewDataContext(source.ScopeFactory, async ctx =>
				{
					return await ctx.Set<TCategoryItem>()
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
			TItemSource source { get; }
			public CategoryLoaderInstance(TItemSource source)
			{
				this.source = source;
			}

			public async Task<ICategoryCached[]> Load(long[] Ids)
			{
				return await NewDataContext(source.ScopeFactory, async ctx =>
				{
					return await source.MapModelToPublic(
						ctx.Set<TCategory>().AsQueryable().Where(c => Ids.Contains(c.Id))
						).ToArrayAsync();
				});

			}
		}
        class TaggedCategoryLoaderInstance : IRelationLoader<string>
        {
            TItemSource source { get; }
            public TaggedCategoryLoaderInstance(TItemSource source)
            {
                this.source = source;
            }

            public async Task<long[]> Load(string tag)
            {
				return await NewDataContext(source.ScopeFactory, async ctx =>
				{
					return await ctx.Set<TCategory>()
						.AsQueryable()
						.Where(c =>
							c.Tag.Contains(tag) &&
							c.ObjectState == EntityLogicState.Enabled
							)
						.OrderBy(c => c.Order)
						.Select(c => c.Id)
						.ToArrayAsync();
				});
            }
        }
        class ItemLoaderInstance : IBatchLoader<IItemCached>
		{
			TItemSource source { get; }
			public ItemLoaderInstance(TItemSource source)
			{
				this.source = source;
			}

			public async Task<IItemCached[]> Load(long[] Ids)
			{
				return await NewDataContext(source.ScopeFactory, async ctx =>
				{
					var re = await source.MapModelToPublic(
						ctx.Set<TItem>().AsQueryable().Where(c => Ids.Contains(c.Id))
						).ToArrayAsync();
					return re.ToArray();
				});

			}
		}
		class ProductLoaderInstance : IBatchLoader<IProductCached>
		{
			TItemSource source { get; }
			public ProductLoaderInstance(TItemSource source)
			{
				this.source = source;
			}

			public async Task<IProductCached[]> Load(long[] Ids)
			{
				return await NewDataContext(source.ScopeFactory, async ctx =>
				{
					var re = await source.MapModelToPublic(
						ctx.Set<TProduct>().AsQueryable().Where(c => Ids.Contains(c.Id))
						).ToArrayAsync();
					return re.Select(r => r.Build()).ToArray();
				});

			}
		}
		class ProductContentLoaderInstance : IBatchLoader<IProductContentCached>
		{
			TItemSource source { get; }
			public ProductContentLoaderInstance(TItemSource source)
			{
				this.source = source;
			}

			public async Task<IProductContentCached[]> Load(long[] Ids)
			{
				return await NewDataContext(source.ScopeFactory, async ctx =>
				{
					var re = await source.MapModelToPublic(
						ctx.Set<TProductDetail>().AsQueryable().Where(c => Ids.Contains(c.Id))
						).ToArrayAsync();
					return re.Select(r => r.Build()).ToArray();
				});

			}
		}
		public IServiceScopeFactory ScopeFactory { get; }
		public IRelationLoader CategoryChildrenLoader { get; }
		public IRelationLoader CategoryItemsLoader { get; }
		public IBatchLoader<ICategoryCached> CategoryLoader { get; }
		public IBatchLoader<IItemCached> ItemLoader { get; }
		public IBatchLoader<IProductCached> ProductLoader { get; }
		public IBatchLoader<IProductContentCached> ProductContentLoader { get; }

        public IRelationLoader<string> TaggedCategoryLoader { get; }

        public class ProductContentResult
		{
			public TProductDetail Detail { get; set; }
			public virtual IProductContentCached Build()
			{
				return new TProductContentCached
				{
					Id = Detail.Id,
					Descs = Json.Parse<ProductDescItem[]>(Detail.Detail),
					Images = Json.Parse<ProductImage[]>(Detail.Images)
				};
			}
		}
		protected virtual IContextQueryable<ProductContentResult> MapModelToPublic(IContextQueryable<TProductDetail> query)
		{
			return from c in query
				   select new ProductContentResult
				   {
					   Detail=c					  
				   };
		}
		protected virtual IContextQueryable<TCategoryCached> MapModelToPublic(IContextQueryable<TCategory> query)
		{
            return from c in query
                   select new TCategoryCached
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
			public TProductCached Product { get; set; }
			public string Tags { get; set; }
			public virtual IProductCached Build()
			{
				Product.Tags= Tags?.Split(',');
				return Product;
			}
		}


		protected virtual IContextQueryable<ProductResult> MapModelToPublic(IContextQueryable<TProduct> query)
		{
			return
				from p in query
				select new ProductResult
				{
					Product = new TProductCached
					{
						Id = p.Id,
                        IsVirtual=p.IsVirtual,
                        CouponDisabled=p.CouponDisabled,
						Title = p.Title,
						MarketPrice = p.MarketPrice,
						Price = p.Price,
						Image = p.Image,
						PublishedTime = p.PublishedTime,
						Visited = p.Visited,
						MainItemId = p.Items.Where(it => it.SellerId == p.OwnerUserId).Select(it => it.Id).FirstOrDefault(),
						OnSale=p.ObjectState==EntityLogicState.Enabled
					}
				};
		}

		protected virtual IContextQueryable<TItemCached> MapModelToPublic(IContextQueryable<TItem> query)
		{
			return
				from item in query
				select new TItemCached
				{
					Id = item.Id,
					SellerId = item.SellerId,
					ProductId = item.ProductId
				};
		}
		public ItemSource(
			IServiceScopeFactory ScopeFactory,
			[EntityIdent(typeof(CategoryInternal))]
			long MainCategoryId,
			IEventSubscriber<ServiceInstanceChanged> ServiceInstanceChanged
			)
		{
			this.MainCategoryId = MainCategoryId;

			this.ScopeFactory = ScopeFactory;
			this.CategoryChildrenLoader = new CategoryChildrenLoaderInstance((TItemSource)this);
			this.CategoryItemsLoader = new CategoryItemsLoaderInstance((TItemSource)this);
			this.CategoryLoader = new CategoryLoaderInstance((TItemSource)this);
			this.ItemLoader = new ItemLoaderInstance((TItemSource)this);
			this.ProductContentLoader = new ProductContentLoaderInstance((TItemSource)this);
			this.ProductLoader= new ProductLoaderInstance((TItemSource)this);
            this.TaggedCategoryLoader=new TaggedCategoryLoaderInstance((TItemSource)this); 
        }

	}
}
