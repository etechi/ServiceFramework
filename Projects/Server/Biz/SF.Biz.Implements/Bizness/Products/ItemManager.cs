﻿#region Apache License Version 2.0
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
using SF.Biz.Products.Entity.DataModels;
using SF.Entities;
using SF.Data;

namespace SF.Biz.Products.Entity
{
	public class ItemManager :
		ItemManager<ItemInternal, ItemEditable, DataModels.Product, DataModels.ProductDetail, DataModels.ProductType, DataModels.Category, DataModels.CategoryItem, DataModels.PropertyScope, DataModels.Property, DataModels.PropertyItem, DataModels.Item, DataModels.ProductSpec>,
		IProductItemManager
	{
		public ItemManager(IEntityServiceContext ServiceContext, Lazy<IItemNotifier> ItemNotifier) : base(ServiceContext, ItemNotifier)
		{
		}
	}


	public class ItemManager<TInternal, TEditable, TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem,TItem,TProductSpec> :
		ModidifiableEntityManager<ObjectKey<long>, TInternal,ItemQueryArgument, TEditable,TItem>,
		IProductItemManager<TInternal,TEditable>
		where TInternal : ItemInternal,  new()
		where TEditable : ItemEditable, new()
		where TProduct : Product<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TProductDetail : ProductDetail<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TProductType : ProductType<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TCategory : Category<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TCategoryItem : CategoryItem<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TPropertyScope : PropertyScope<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TProperty : Property<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TPropertyItem : PropertyItem<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TItem : Item<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>,new()
        where TProductSpec : ProductSpec<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem, TProductSpec>
    {
		Lazy<IItemNotifier> ItemNotifier { get; set; }
		public ItemManager(IEntityServiceContext ServiceContext, Lazy<IItemNotifier> ItemNotifier) :
			base(ServiceContext)
		{
			this.ItemNotifier = ItemNotifier;
		}
		protected override PagingQueryBuilder<TItem> PagingQueryBuilder { get; } = new PagingQueryBuilder<TItem>(
			"time",
			b => b.Add("time", i => i.UpdatedTime)
			);
		protected override IContextQueryable<TItem> OnBuildQuery(IContextQueryable<TItem> Query, ItemQueryArgument Arg, Paging paging)
		{
			if (Arg == null)
				return Query;

			if (Arg.CategoryId.HasValue)
				Query = Query.Where(i => i.CategoryItems.Any(ci => ci.CategoryId == Arg.CategoryId.Value));

			if(Arg.CategoryTag.HasContent())
				Query = Query.Where(i => i.CategoryItems.Any(ci => ci.Category.Tag == Arg.CategoryTag));


			return Query.Filter(Arg.ProductId, i => i.ProductId)
				.Filter(Arg.SellerId, i => i.SellerId)
				.FilterContains(Arg.Title, i => i.Title)
				.Filter(Arg.TypeId, i => i.Product.TypeId)
				;
		}
		protected override IContextQueryable<TInternal> OnMapModelToDetail(IContextQueryable<TItem> Query)
		{
			return from c in Query
				   select new TInternal
				   {
						Id=c.Id,
						SourceItemId=c.SourceItemId,
						ProductId=c.ProductId,
						Price=c.Price,
						Title=c.Title,
						Image=c.Image
					};
		}
        protected override Task<TEditable> OnMapModelToEditable(IContextQueryable<TItem> Query)
		{
			return (from c in Query
				   select new TEditable
				   {
					   Id = c.Id,
					   SourceItemId = c.SourceItemId,
					   ProductId = c.ProductId,
					   Price = c.Price,
					   Title = c.Title,
					   Image = c.Image
				   }).SingleOrDefaultAsync();
		}
		
		protected override Task OnUpdateModel(IModifyContext ctx)
		{
			var Model = ctx.Model;
			var obj = ctx.Editable;
			Ensure.Equal(Model.ProductId, obj.ProductId,"不能修改ProductId");
			Ensure.Equal(Model.SourceItemId ?? 0, obj.SourceItemId??0, "不能修改SourceItemId");
			Ensure.Equal(Model.SellerId, obj.SellerId, "不能修改SellerId");

			Model.Title = obj.Title;
			Model.Image = obj.Image;
			Model.Price = obj.Price;
			Model.UpdatedTime = Now;
			
			return Task.CompletedTask;
		}
		protected override async Task OnNewModel(IModifyContext ctx)
		{
			var Model = ctx.Model;
			var obj = ctx.Editable;
			Model.Id = await IdentGenerator.GenerateAsync<TItem>();
			Model.CreatedTime = Now;
			Model.SellerId = obj.SellerId;
			Model.ProductId = obj.ProductId;
			Model.SourceItemId = obj.SourceItemId;
			if (Model.SourceItemId.HasValue)
			{
				var level = await DataSet.AsQueryable()
					.Where(i => i.Id == Model.SourceItemId)
					.Select(i => i.SourceLevel)
					.SingleOrDefaultAsync();
				Model.SourceLevel = level + 1;
			}
			
		}
		//public override async Task<TItem> UpdateAsync(TEditable obj)
		//{
		//	var re=await base.UpdateAsync(obj);
		//	ItemNotifier.Value.NotifyItemChanged(obj.Id);
		//	return re;
		//}
		//public override async Task<TItem> DeleteAsync(int Id)
		//{
		//	var re=await base.DeleteAsync(Id);
		//	if (re == null) return re;
		//	if(re.CategoryItems!=null)
		//		foreach(var cat in re.CategoryItems)
		//			ItemNotifier.Value.NotifyCategoryItemsChanged(cat.CategoryId);
		//	ItemNotifier.Value.NotifyItemChanged(Id);
		//	return re;
		//}
	}
}
