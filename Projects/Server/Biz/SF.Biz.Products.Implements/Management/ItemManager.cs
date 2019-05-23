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
using SF.Biz.Products.Entity.DataModels;
using SF.Sys.Entities;
using SF.Sys;
using SF.Sys.Data;
namespace SF.Biz.Products.Entity
{
	

	public class ItemManager:
		ModidifiableEntityManager<ObjectKey<long>, ItemInternal,ItemQueryArgument, ItemEditable,DataModels.DataItem>,
		IProductItemManager
    {
		Lazy<IItemNotifier> ItemNotifier { get; set; }
		public ItemManager(IEntityServiceContext ServiceContext, Lazy<IItemNotifier> ItemNotifier) :
			base(ServiceContext)
		{
			this.ItemNotifier = ItemNotifier;
		}
		protected override PagingQueryBuilder<DataModels.DataItem> PagingQueryBuilder { get; } = new PagingQueryBuilder<DataModels.DataItem>(
			"time",
			b => b.Add("time", i => i.UpdatedTime)
			);
		protected override IQueryable<DataModels.DataItem> OnBuildQuery(IQueryable<DataModels.DataItem> Query, ItemQueryArgument Arg)
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
		protected override IQueryable<ItemInternal> OnMapModelToDetail(IQueryable<DataModels.DataItem> Query)
		{
			return from c in Query
				   select new ItemInternal
				   {
						Id=c.Id,
						SourceItemId=c.SourceItemId,
						ProductId=c.ProductId,
						Price=c.Price,
						Title=c.Title,
						Image=c.Image
					};
		}
		protected override Task<ItemEditable> OnMapModelToEditable(IDataContext Context, IQueryable<DataModels.DataItem> Query)
		{
			return (from c in Query
				   select new ItemEditable
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
			Model.Id = await IdentGenerator.GenerateAsync<DataModels.DataItem>();
			Model.CreatedTime = Now;
			Model.SellerId = obj.SellerId;
			Model.ProductId = obj.ProductId;
			Model.SourceItemId = obj.SourceItemId;
			if (Model.SourceItemId.HasValue)
			{
				var level = await ctx.DataContext.Queryable<DataModels.DataItem>()
					.Where(i => i.Id == Model.SourceItemId)
					.Select(i => i.SourceLevel)
					.SingleOrDefaultAsync();
				Model.SourceLevel = level + 1;
			}
			
		}
		//public override async Task<DataModels.DataItem> UpdateAsync(ItemEditable obj)
		//{
		//	var re=await base.UpdateAsync(obj);
		//	ItemNotifier.Value.NotifyItemChanged(obj.Id);
		//	return re;
		//}
		//public override async Task<DataModels.DataItem> DeleteAsync(int Id)
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
