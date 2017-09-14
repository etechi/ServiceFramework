using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.ObjectManager;
using SF.Data.Entity;
using System.Data.Entity;

namespace SF.Biz.Products.Entity
{
	public class ProductTypeService<TPublicProductType, TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TUserFavorited> :
		IProductTypeService<TPublicProductType>
		where TPublicProductType: ProductType, new()
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
		public ProductTypeService(DataContext Context)
		{
			this.Context = Context;
		}

		public async Task<TPublicProductType[]> GetProductTypes()
		{
			var q = from pt in Context.ReadOnly<TProductType>()
					where pt.ObjectState == EntityLogicState.Enabled
					orderby pt.Order
					select new TPublicProductType
					{
						Id = pt.Id,
						Title = pt.Title,
						Icon = pt.Icon,
						Image = pt.Image,
						ProductCount=pt.ProductCount
					};
			return await q.ToArrayAsync();
		}
	}
}
