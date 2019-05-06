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

using System.Linq;
using System.Threading.Tasks;
using SF.Sys.Entities;
using SF.Sys.Data;
namespace SF.Biz.Products.Entity
{
	public class ProductTypeManager :
		ProductTypeManager<ProductTypeInternal, ProductTypeEditable, DataModels.Product, DataModels.ProductDetail, DataModels.ProductType, DataModels.Category, DataModels.CategoryItem, DataModels.PropertyScope, DataModels.Property, DataModels.PropertyItem, DataModels.Item, DataModels.ProductSpec> ,
		IProductTypeManager
	{
		public ProductTypeManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
		{
		}
	}


	public class ProductTypeManager<TInternal, TEditable, TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec> :
		ModidifiableEntityManager<ObjectKey<long>, TInternal,ProductTypeQueryArgument,  TEditable, TProductType>,
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
		public ProductTypeManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
		{
		}
        protected override IQueryable<TInternal> OnMapModelToDetail(IQueryable<TProductType> Query)
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
        protected override Task<TEditable> OnMapModelToEditable(IDataContext ctx,IQueryable<TProductType> Query)
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
		protected override async Task OnNewModel(IModifyContext ctx)
		{
			var Model = ctx.Model;
			Model.Id = await IdentGenerator.GenerateAsync<TProductType>();
			Model.CreatedTime = Now;
		}
		protected override Task<TProductType> OnLoadModelForUpdate(ObjectKey<long> Id, IQueryable<TProductType> ctx)
		{
			return ctx
				.Where(s => s.Id == Id.Id)
				//.Include(s => s.Items)
				.SingleOrDefaultAsync();
		}
		protected override IQueryable<TProductType> OnBuildQuery(IQueryable<TProductType> Query, ProductTypeQueryArgument Arg)
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
