using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.ObjectManager;
using SF.Data.Entity;
using System.Data.Entity;
using System.Linq.Expressions;

namespace SF.Biz.Products.Entity
{
	
	public class ProducTCategoryService<TPublicProducTCategory, TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TUserFavorited> :
		IProducTCategoryService< TPublicProducTCategory>
		where TPublicProducTCategory :ProducTCategory<TKey>,new()
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
		public ProducTCategoryService(DataContext Context)
		{
			this.Context = Context;
		}
	
	
		public Task<TPublicProducTCategory> GetProducTCategory(TKey collection)
		{
			return Context.ReadOnly<TCategory>()
				.Where(c => c.Id.Equals(collection) && c.ObjectState == EntityLogicState.Enabled)
				.Select(c => new TPublicProducTCategory
				{
					Id = c.Id,
					Ident = c.Ident,
					Title = c.Title,
					Description = c.Description,
					Image = c.Image,
					Icon = c.Icon,
					ProductCount = c.ItemCount
				}).SingleOrDefaultAsync();
		}

		
	}
}
