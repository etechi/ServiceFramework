using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.ObjectManager;
using SF.Times;
using SF.Data.Entity;

namespace SF.Biz.Products.Entity
{
    [DataObjectLoader("产品类型")]
	public class ProductTypeManager<TInternal, TEditable, TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec> :
		EntityServiceObjectManager<int, TInternal, TEditable, TProductType>,
		IProductTypeManager<TInternal, TEditable>,
        IDataObjectLoader
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
		Lazy<ITimeService> TimeService { get; }
		public ProductTypeManager(IDataContext Context, Lazy<ITimeService> TimeService,Lazy<IModifyFilter> ModifyFilter) : base(Context, ModifyFilter)
		{
			this.TimeService = TimeService;
		}
        protected override IContextQueryable<TInternal> MapModelToInternal(IContextQueryable<TProductType> Query)
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
        protected override Task<TEditable> MapModelToEditable(IContextQueryable<TProductType> Query)
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
		
		protected override Task OnUpdateModel(ModifyContext ctx)
		{
			var Model = ctx.Model;
			var obj = ctx.Editable;
			Model.Name = obj.Name;
			Model.Title = obj.Title;
			Model.Icon = obj.Icon;
			Model.Image = obj.Image;
			Model.ObjectState = obj.ObjectState;
			Model.Unit = obj.Unit;
			Model.UpdatedTime = TimeService.Value.Now;
			return Task.CompletedTask;
		}
		protected override Task OnNewModel(ModifyContext ctx)
		{
			var Model = ctx.Model;
			Model.CreatedTime = TimeService.Value.Now;
			return Task.CompletedTask;
		}
		protected override Task<TProductType> OnLoadModelForUpdate(ModifyContext ctx)
		{
            var Id=ctx.Id;
			return Set
				.Where(s => s.Id==Id)
				//.Include(s => s.Items)
				.SingleOrDefaultAsync();
		}
		

		static PagingQueryBuilder<TCategory> pagingBuilder = new PagingQueryBuilder<TCategory>(
			"name",
			i => i
			.Add("name", c => c.Name)
			.Add("updated", c => c.UpdatedTime, true)
			.Add("created", c => c.CreatedTime, true)
			);
		public async Task<TInternal[]> List(ProductTypeQueryArgument Arg)
		{
			return await MapModelToInternal(
				Context.ReadOnly<TProductType>().Filter(Arg.ObjectState,t=>t.ObjectState).OrderBy(t=>t.Order)
				).ToArrayAsync();
		}

        async Task<IDataObject[]> IDataObjectLoader.Load(string Type, string[][] Keys)
        {
            var re = await DataObjectLoader.Load(
                Keys,
                id => int.Parse( id[0]),
                id => FindByIdAsync(id),
                async (ids) => {
                    var tmps = await MapModelToInternal(Context.ReadOnly<TProductType>().Where(a => ids.Contains(a.Id))).ToArrayAsync();
                    return tmps;
                });
            return re;
        }

    }
}
