using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using SF.Data;

namespace SF.Biz.Products.Entity
{

	public class ProductService<TPublicProduct, TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TUserFavorited> :
		IProductService<TPublicProduct>
		where TPublicProduct : Product<TKey>, new()
		where TProduct : DataModels.Product<TUserKey, TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TUserFavorited>
		where TProductDetail : DataModels.ProductDetail<TUserKey, TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TUserFavorited>
		where TProductType : DataModels.ProductType<TUserKey, TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TUserFavorited>
		where TCategory : DataModels.Category<TUserKey, TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TUserFavorited>
		where TCategoryItem : DataModels.CategoryItem<TUserKey, TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TUserFavorited>
		where TPropertyScope : DataModels.PropertyScope<TUserKey, TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TUserFavorited>
		where TProperty : DataModels.Property<TUserKey, TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TUserFavorited>
		where TPropertyItem : DataModels.PropertyItem<TUserKey, TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TUserFavorited>
		where TUserFavorited : DataModels.UserFavorited<TUserKey, TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TUserFavorited>
		where TKey : IEquatable<TKey>
	{
		public DataContext Context { get; }
		public ProductService(DataContext Context)
		{
			this.Context = Context;
		}
	
		public class ProductResult
		{
			public IQueryable<TProduct> Model { get; set; }
			public TProductDetail Detail { get; set; }
			public TPublicProduct Product { get; set; }
			public string Collections { get; set; }
			public virtual TPublicProduct Build()
			{
				if(Detail!=null)
					Product.Detail = new ProductDetail<TKey>
					{
						Descs = Json.Decode<ProductDescItem[]>(Detail?.Detail),
						Images = Json.Decode<ProductImage[]>(Detail?.Images),
					};
				Product.Collections = Collections?.Split(',');
				return Product;
			}
		}
		
		protected virtual IQueryable<ProductResult> LoadProductResult(IQueryable<TProduct> q, bool withDetail)
		{
			return
				from product in q
				select new ProductResult
				{
					Product = new TPublicProduct
					{
						Id = product.Id,
						Title = product.Title,
						MarketPrice = product.MarketPrice,
						Price = product.Price,
						Image = product.Image,
						PublishDate = product.PublishedTime,
						Visited = product.Visited,
						SellCount = product.SellCount,
						Type = new ProductType
						{
							Id = product.TypeId,
							Icon = product.Type.Icon,
							Image = product.Type.Image,
							ProductCount = product.Type.ProductCount,
							Title = product.Type.Title
						}
					},
					Detail = withDetail ? product.Detail : null
				};
			
		}
		
		public async Task<TPublicProduct> GetProductAsync(TKey id,bool detail)
		{
			var re= await LoadProductResult(Context.ReadOnly<TProduct>().Where(p => p.Id.Equals(id) && p.ObjectState==EntityLogicState.Enabled), detail)
				.SingleOrDefaultAsync();
			if (re == null) return null;
			return re.Build();
		}
		public async Task<TPublicProduct[]> GetProductsAsync(TKey[] ids)
		{
			var re = await LoadProductResult(Context.ReadOnly<TProduct>().Where(p => ids.Contains(p.Id) && p.ObjectState == EntityLogicState.Enabled), false)
				.ToArrayAsync();
			return re.Select(r => r.Build()).ToArray();
		}


		static PagingQueryBuilder<TProduct> pagingBuilder = new PagingQueryBuilder<TProduct>(
			"hot",
			i =>
			  i.Add("new", p => p.PublishedTime, true)
			  .Add("hot", p => p.Visited, true)
			  .Add("price", p => p.Price)
		);

		public virtual PagingQueryBuilder<TProduct> PagingBuilder { get { return pagingBuilder; } }

		public async Task<QueryResult<TPublicProduct>> QueryProducts(int? type, IDictionary<int, int> properties, Paging paging)
		{
			var q = Context.ReadOnly<TProduct>()
				.Where(p=>p.ObjectState==EntityLogicState.Enabled);

			if (type.HasValue)
				q = q.Where(p => p.TypeId == type.Value);

			var re= await q.ToQueryResultAsync(
				pq => LoadProductResult(pq, false),
				r => r.Build(),
				PagingBuilder,
				paging
				);
			return re;
		}
		public async Task<QueryResult<TPublicProduct>> SearchProducts(string key, Paging paging)
		{
			var q = Context.ReadOnly<TProduct>()
				.Where(p => p.ObjectState == EntityLogicState.Enabled && p.Title.Contains(key));

			var re = await q.ToQueryResultAsync(
				pq => LoadProductResult(pq, false),
				r => r.Build(),
				PagingBuilder,
				paging
				);
			return re;
		}
	
		static PagingQueryBuilder<TCategoryItem> defaulTCategoryPagingBuilder = new PagingQueryBuilder<TCategoryItem>(
			"order",
			i =>
			  i.Add("order", p => p.Order)
		);
		public async Task<QueryResult<TPublicProduct>> QueryCollectionProducts(TKey collection, Paging paging)
		{
			var q = from c in Context.ReadOnly<TCategory>()
					where c.Id.Equals(collection) && c.ObjectState == EntityLogicState.Enabled
					from pi in c.Items
					select pi;

			if (paging == null || paging.SortMethod == null)
			{
				var re = await q.ToQueryResultAsync(
					tpq => LoadProductResult(tpq.Select(ci=>ci.Product), false),
					r => r.Build(),
					defaulTCategoryPagingBuilder,
					paging
					);
				return re;
			}
			else
			{
				var re = await q.Select(qi=>qi.Product).ToQueryResultAsync(
					tpq => LoadProductResult(tpq, false),
					r => r.Build(),
					PagingBuilder,
					paging
					);
				return re;
			}
		}
	}
}
