using SF.Data;
using SF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Biz.Products.Entity
{
	public class ProductTypeManager<TInternal, TEditable, TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec> :
		EntityManager<long, TInternal,ProductTypeQueryArgument,  TEditable, TProductType>,
		IProductTypeManager<TInternal, TEditable>
		where TInternal : ProductTypeInternal, new()
		where TEditable : ProductTypeEditable, new()
		where TProduct : DataModels.Product<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TProductDetail : DataModels.ProductDetail<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TProductType : DataModels.ProductType<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>,new()
		where TCategory : DataModels.Category<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TCategoryItem : DataModels.CategoryItem<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TPropertyScope : DataModels.PropertyScope<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TProperty : DataModels.Property<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TPropertyItem : DataModels.PropertyItem<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TItem : DataModels.Item<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
        where TProductSpec : DataModels.ProductSpec<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem, TProductSpec>
    {
		public ProductTypeManager(IDataSetEntityManager<TProductType> EntityManager) : base(EntityManager)
		{
		}
        protected override IContextQueryable<TInternal> OnMapModelToDetail(IContextQueryable<TProductType> Query)
		{
			return from c in Query
				   select new TInternal
				   {
					   Id = c.Id,
					   Name = c.Name,
					   CreatedTime = c.CreatedTime,
					   UpdatedTime = c.UpdatedTime,
					   Title = c.Title,
					   Unit = c.Unit,
					   ObjectState = c.ObjectState,
					   Icon = c.Icon,
					   Image = c.Image,
						ProductCount = c.ProductCount
					};
		}
        protected override Task<TEditable> OnMapModelToEditable(IContextQueryable<TProductType> Query)
		{
			return (from c in Query
					select new TEditable
					{
						Id = c.Id,
						Name = c.Name,
						Title = c.Title,
						Order = c.Order,
						Unit = c.Unit,
						ObjectState = c.ObjectState,
						Icon = c.Icon,
						Image = c.Image,
						ProductCount = c.ProductCount
				   }).SingleOrDefaultAsync();
		}
		
		protected override Task OnUpdateModel(IModifyContext ctx)
		{
			var Model = ctx.Model;
			var obj = ctx.Editable;
			Model.Name = obj.Name;
			Model.Title = obj.Title;
			Model.Icon = obj.Icon;
			Model.Image = obj.Image;
			Model.ObjectState = obj.ObjectState;
			Model.Unit = obj.Unit;
			Model.UpdatedTime = Now;
			return Task.CompletedTask;
		}
		protected override Task OnNewModel(IModifyContext ctx)
		{
			var Model = ctx.Model;
			Model.CreatedTime = Now;
			return Task.CompletedTask;
		}
		protected override Task<TProductType> OnLoadModelForUpdate(long Id, IContextQueryable<TProductType> ctx)
		{
			return ctx
				.Where(s => s.Id == Id)
				//.Include(s => s.Items)
				.SingleOrDefaultAsync();
		}
		protected override IContextQueryable<TProductType> OnBuildQuery(IContextQueryable<TProductType> Query, ProductTypeQueryArgument Arg, Paging paging)
		{
			return Query
				.Filter(Arg.ObjectState, p => p.ObjectState)
				.Filter(Arg.Name,p=>p.Name);
		}
		protected override PagingQueryBuilder<TProductType> PagingQueryBuilder =>new PagingQueryBuilder<TProductType>(
			"name",
			i => i
			.Add("name", c => c.Name)
			.Add("updated", c => c.UpdatedTime, true)
			.Add("created", c => c.CreatedTime, true)
			);
	

    }
}
