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

using SF.Sys;
using SF.Sys.Data;
using SF.Sys.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Biz.Products.Entity
{
	public class ProductManager:
		ModidifiableEntityManager<ObjectKey<long>, ProductInternal, ProductInternalQueryArgument, ProductEditable, DataModels.DataProduct>,
		IProductManager
    {
		public Lazy<IItemNotifier> ItemNotifier { get; }
        public ProductManager(
			IEntityServiceContext ServiceContext, 
			Lazy<IItemNotifier> ItemNotifier
            ) : base(ServiceContext)
		{
			this.ItemNotifier = ItemNotifier;
		}
        protected override IQueryable<ProductInternal> OnMapModelToDetail(IQueryable<DataModels.DataProduct> Query)
		{
			return from c in Query
				   select new ProductInternal
				   {
					   Id = c.Id,
					   Title = c.Title,
					   MarketPrice = c.MarketPrice,
					   Price = c.Price,
                       IsVirtual=c.IsVirtual,
                       CouponDisabled=c.CouponDisabled,
					   Image = c.Image,
					   Name = c.Name,
					   UpdatedTime = c.UpdatedTime,
					   CreatedTime = c.CreatedTime,
					   PublishedTime = c.PublishedTime,
					   LogicState = c.LogicState,
					   ProductTypeId=c.TypeId
					   
				   };
		}

		protected virtual Task OnInitEditable(ProductEditable editable,DataModels.DataProduct product)
		{
			return Task.CompletedTask;
		}
        protected override async Task<ProductEditable> OnMapModelToEditable(IDataContext ctx, IQueryable<DataModels.DataProduct> Query)
		{
			var q = from m in Query
					select new {
						product = m,
						cats= from it in m.Items
							  where it.SellerId==m.OwnerId
							  from cit in it.CategoryItems
							  select cit.CategoryId,
                              
                        Specs = m.Specs.Select(i =>
                              new ProductSpecDetail
                              {
                                  Id = i.Id,
                                  Name = i.Name,
                                  CreatedTime = i.CreatedTime,
                                  Description = i.Description,
                                  Image = i.Image,
                                  LogicState = i.LogicState,
                                  UpdatedTime = i.UpdatedTime,
								  VIADSpecId=i.VIADSpecId
                              })
                    };

			var data=await q.SingleOrDefaultAsync();
			if (data == null) return null;
			var e= new ProductEditable
			{
				Id= data.product.Id,
				Title= data.product.Title,
				MarketPrice= data.product.MarketPrice,
				Price= data.product.Price,
                IsVirtual=data.product.IsVirtual,
                CouponDisabled=data.product.CouponDisabled,
				Image= data.product.Image,
				Detail= Json.Parse<ProductDescItem[]>(data.product.Detail),
			    Images= Json.Parse<ProductImage[]>(data.product.Images),
				Name= data.product.Name,
				TypeId= data.product.TypeId,
				OwnerId= data.product.OwnerId,
				PublishedTime= data.product.PublishedTime,
				LogicState= data.product.LogicState,
				CategoryIds= data.cats,
                Specs=data.Specs,
				VIADSpecId=data.product.VIADSpecId
				//Properties=null
			};
			await OnInitEditable(e, data.product);
			return e;
		}
		
		protected override async Task OnUpdateModel(IModifyContext ctx)
		{
			var Model = ctx.Model;
			var obj = ctx.Editable;
			Model.Name = obj.Name;
            Model.IsVirtual = obj.IsVirtual;
            Model.CouponDisabled = obj.CouponDisabled;
			Model.Title = obj.Title;
			Model.Image = obj.Image;
			Model.MarketPrice = obj.MarketPrice;
			Model.Price = obj.Price;
			Model.PublishedTime = obj.PublishedTime;
			Model.OwnerId = obj.OwnerId;
			Model.Detail = Json.Stringify(obj.Detail);
			Model.Images = Json.Stringify(obj.Images);
			Model.UpdatedTime = Now;
			Model.LogicState = obj.LogicState;
			Model.VIADSpecId = obj.VIADSpecId;
			if (Model.TypeId != obj.TypeId)
			{
				if (Model.LogicState == EntityLogicState.Enabled)
				{
					if(Model.TypeId!=0)
						await UpdateTypeProductCount(ctx.DataContext,Model.TypeId, -1);
					await UpdateTypeProductCount(ctx.DataContext, obj.TypeId, 1);

				}
				Model.TypeId = obj.TypeId;
			}
			

			if (obj.Specs != null)
            {
				if (obj.Specs.Count() > 0 && obj.VIADSpecId.HasValue)
					throw new PublicArgumentException("包含规格的产品需要针对每个规格设置自动发货规格。");
				var idx = 1;
                foreach (var s in obj.Specs)
                {
                    UIEnsure.HasContent(s.Name, "请输入规格名称");
                    s.Order = idx++;
                }
            }

			ctx.DataContext.Set<DataModels.DataProductSpec>().Merge(
                Model.Specs,
                obj.Specs,
                (m, e) => m.Id == e.Id,
                e => new DataModels.DataProductSpec
                {
                    ProductId = Model.Id,
                    CreatedTime = Now,
                    Name = e.Name,
                    Description = e.Description,
                    Image = e.Image,
                    LogicState = e.LogicState,
                    UpdatedTime = Now,
					VIADSpecId=e.VIADSpecId
                },
                (m, e) =>
                {
                    m.Id = e.Id;
                    m.Name = e.Name;
                    m.Description = e.Description;
                    m.Image = e.Image;
                    m.LogicState = e.LogicState;
                    m.UpdatedTime = Now;
					m.VIADSpecId = e.VIADSpecId;

				}
                );

            if (Model.Id!=0)
			{
				var notifier = this.ItemNotifier.Value;
				ctx.DataContext.AddCommitTracker(
					TransactionCommitNotifyType.AfterCommit,
					(t, ex) =>
					{ 
						notifier.NotifyProductChanged(Model.Id);
						
					}
					);

				var defaultItemId = await ctx.DataContext
					.Set<DataModels.DataItem>().AsQueryable()
					.Where(i => i.ProductId == Model.Id && i.SellerId == Model.OwnerId)
					.Select(i => i.Id)
					.SingleOrDefaultAsync();
				if (defaultItemId != 0)
				{
					var cats = await ctx.DataContext
						.Set<DataModels.DataCategoryItem>().AsQueryable(false)
						.Where(ci => ci.ItemId == defaultItemId)
						.ToArrayAsync();

					var Removed = new List<long>();
					var Added = new List<long>();
					ctx.DataContext.Set<DataModels.DataCategoryItem>().Merge(
						cats,
						obj.CategoryIds,
						(m, e) => m.CategoryId == e,
						e =>
						{
							Added.Add(e);
							return new DataModels.DataCategoryItem { CategoryId = e, ItemId = defaultItemId };
						},
						null,
						(e) =>
						{
							Removed.Add(e.CategoryId);
							ctx.DataContext.Remove(e);
						}
						);
					ctx.DataContext.AddCommitTracker(
						TransactionCommitNotifyType.AfterCommit,
						(t, ex) =>
						{ 
								foreach (var i in Added)
								notifier.NotifyCategoryItemsChanged(i);
							foreach (var i in Removed)
								notifier.NotifyCategoryItemsChanged(i);
						}
						);
				}
			}
		}

		protected override async Task OnNewModel(IModifyContext ctx)
		{
			var Model = ctx.Model;
			var obj = ctx.Editable;
			Model.Id = await IdentGenerator.GenerateAsync<DataModels.DataProduct>();
			Model.LogicState = EntityLogicState.Disabled;
			Model.CreatedTime = Now;
			var itemId = await IdentGenerator.GenerateAsync<DataModels.DataItem>();
			Model.Items = new[]
			{
				new DataModels.DataItem
				{
					Id= itemId,
					SellerId = obj.OwnerId.Value,
					CreatedTime=Model.CreatedTime,
					UpdatedTime=Model.CreatedTime,
					CategoryItems=obj.CategoryIds==null?
						null:
						obj.CategoryIds.Select(
							cid=>new DataModels.DataCategoryItem { CategoryId =cid,ItemId=itemId}
							)
						.ToArray()
				}
			};
            var notifier = this.ItemNotifier.Value;
            if(obj.CategoryIds!=null)
				ctx.DataContext.AddCommitTracker(
					TransactionCommitNotifyType.AfterCommit,
					(t, ex) =>
					{ 
						foreach (var cid in obj.CategoryIds)
								notifier.NotifyCategoryItemsChanged(cid);
					});
		}
		protected override IQueryable<DataModels.DataProduct> OnLoadChildObjectsForUpdate(ObjectKey<long> Id, IQueryable<DataModels.DataProduct> query)
		{
			return query.Include(p => p.Specs);
		}
		

		async Task UpdateTypeProductCount(IDataContext DataContext,long type,int diff)
		{
			var t = await DataContext.Set<DataModels.DataProductType>().FindAsync(type);
			if (t == null)
				throw new ArgumentException("产品类型无效:" + type);
			t.ProductCount = Math.Max(0, t.ProductCount + diff);
			DataContext.Set<DataModels.DataProductType>().Update(t);
		}
		protected override async Task OnRemoveModel(IModifyContext ctx)
		{
			var Id = ctx.Editable.Id;
			var prd = await ctx.DataContext.Set<DataModels.DataProduct>().AsQueryable(false).Where(p => p.Id.Equals(Id))
				.Include(p => p.Detail)
				.Include(p => p.PropertyItems)
				.Include(p => p.Specs)
				.Include(p => p.Items.Select(i => i.CategoryItems))
				.SingleOrDefaultAsync();
			if (prd == null)
				return;

			var cids = prd.Items.SelectMany(i => i.CategoryItems).Select(ci => ci.CategoryId).ToArray();


			if (prd.LogicState == EntityLogicState.Enabled)
				await UpdateTypeProductCount(ctx.DataContext,prd.TypeId, -1);

			foreach (var item in prd.Items)
				ctx.DataContext.RemoveRange(item.CategoryItems);


			ctx.DataContext.RemoveRange(prd.Items);
			ctx.DataContext.RemoveRange(prd.PropertyItems);
			ctx.DataContext.RemoveRange(prd.Specs);
			ctx.DataContext.Remove(prd.Detail);

			var notifier = this.ItemNotifier.Value;
			ctx.DataContext.AddCommitTracker(
				TransactionCommitNotifyType.AfterCommit,
				(t, ex) =>
				{
					foreach (var ci in cids)
						notifier.NotifyCategoryItemsChanged(ci);
				});
			await base.OnRemoveModel(ctx);
		}
		

        static PagingQueryBuilder<DataModels.DataProduct> pagingBuilder = new PagingQueryBuilder<DataModels.DataProduct>(
			"updated",
			i => i
			.Add("visited", c => c.Visited,true)
			.Add("sells", c => c.SellCount,true)
			.Add("updated", c => c.UpdatedTime, true)
			.Add("created", c => c.CreatedTime, true)
			);
		protected virtual Task OnObjectStateChanged(DataModels.DataProduct Model, EntityLogicState OrgState)
		{
			return Task.CompletedTask;
		}
		protected virtual Task OnObjectStateChangeSaved(DataModels.DataProduct Model, EntityLogicState OrgState)
		{
			return Task.CompletedTask;
		}
		public virtual async Task SetLogicState(long Id, EntityLogicState State)
		{
			 await DataScope.Use("设置产品逻辑状态", async ctx =>
			 {

				 var e = await ctx.Set<DataModels.DataProduct>().FindAsync(Id);
				 var orgState = e.LogicState;
				 if (orgState == State)
					 return;

				 e.LogicState = State;
				 e.UpdatedTime = Now;
				 if (State == EntityLogicState.Enabled)
					 e.PublishedTime = e.UpdatedTime;
				 ctx.Update(e);
				 await OnObjectStateChanged(e, orgState);


				 if (State == EntityLogicState.Enabled)
					 await UpdateTypeProductCount(ctx, e.TypeId, 1);
				 else if (orgState == EntityLogicState.Enabled)
					 await UpdateTypeProductCount(ctx, e.TypeId, -1);

				 await ctx.SaveChangesAsync();
				 await OnObjectStateChangeSaved(e, orgState);
			 });
		}

		//public override async Task<int> CreateAsync(ProductEditable obj)
		//{
		//	var re=await base.CreateAsync(obj);
		//	if (obj.CategoryIds != null)
		//	{
		//		var notifier = ItemNotifier.Value;
		//		foreach (var cit in obj.CategoryIds)
		//			notifier.NotifyCategoryItemsChanged(cit);
		//	}
		//	return re;
		//}
		//public override async Task<DataModels.DataProduct> DeleteAsync(int Id)
		//{
		//	var re=await base.DeleteAsync(Id);
		//	if (re == null)
		//		return re;

		//	ItemNotifier.Value.NotifyProductChanged(Id);
		//	ItemNotifier.Value.NotifyProductContentChanged(Id);

		//	if(re.Items!=null)
		//		foreach(var item in re.Items)
		//		{
		//			if(item.CategoryItems!=null)
		//				foreach(var cat in item.CategoryItems)
		//					ItemNotifier.Value.NotifyCategoryItemsChanged(cat.CategoryId);
		//			ItemNotifier.Value.NotifyItemChanged(item.Id);
		//		}

		//	return re;
		//}
		//public override async Task<DataModels.DataProduct> UpdateAsync(ProductEditable obj)
		//{
		//	var re=await base.UpdateAsync(obj);
		//	if (re == null) return null;
		//	var notifier = ItemNotifier.Value;
		//	notifier.NotifyProductChanged(obj.Id);
		//	notifier.NotifyProductContentChanged(obj.Id);

		//	if (obj.CategoryIds != null)
		//		foreach (var cid in obj.CategoryIds)
		//			notifier.NotifyCategoryItemsChanged(cid);

		//	if(re.Items!=null)
		//		foreach(var it in re.Items)
		//			if (it.CategoryItems != null)
		//				foreach (var cit in it.CategoryItems)
		//					notifier.NotifyCategoryItemsChanged(cit.CategoryId);
		//	return re;
		//}

		
		//public async Task<QueryResult<ProductInternal>> Query(ProductInternalQueryArgument args,Paging paging)
		//{
		//	var q = OnBuildQuery(Context.ReadOnly<DataModels.DataProduct>(),args);
		//	return await q.ToQueryResultAsync(
		//		MapModelToInternal,
		//		r => r,
		//		pagingBuilder,
		//		paging
		//		);
		//}

        //async Task<IDataObject[]> IDataObjectLoader.Load(string Type, string[][] Keys)
        //{
        //    if (Type == "产品")
        //    {
        //        var re = await DataObjectLoader.Load(
        //            Keys,
        //            id => int.Parse(id[0]),
        //            id => FindByIdAsync(id),
        //            async (ids) =>
        //            {
        //                var tmps = await MapModelToInternal(Context.ReadOnly<DataModels.DataProduct>().Where(a => ids.Contains(a.Id))).ToArrayAsync();
        //                return await OnPrepareInternals(tmps);
        //            }
        //            );
        //        return re;
        //    }
        //    else if (Type == "产品规格")
        //    {
        //        var re = await DataObjectLoader.Load(
        //            Keys,
        //            id => int.Parse(id[0]),
        //            id => GetSpec(id),
        //            async (ids) =>
        //            {
        //                var tmps = await MapSpec(Context.ReadOnly<DataModels.DataProductSpec>().Where(a => ids.Contains(a.Id))).ToArrayAsync();
        //                return tmps;
        //            }
        //            );
        //        return re;
        //    }
        //    else
        //        return Empty.Array<IDataObject>();
        //}
        IQueryable<ProductSpec> MapSpec(IQueryable<DataModels.DataProductSpec> query)
        {
            return query.Select(s => new ProductSpec
            {
                Description = s.Description,
                Id = s.Id,
                Image = s.Image,
                Name = s.Name,
				VIADSpecId=s.VIADSpecId
            });
        }

        public Task<ProductSpec[]> ListSpec(long Id)
        {
			return DataScope.Use("规格清单", ctx =>
				MapSpec(
				ctx.Set<DataModels.DataProductSpec>().AsQueryable()
				.Where(s => s.ProductId == Id && s.LogicState == EntityLogicState.Enabled)
				.OrderBy(s => s.Order)
				).ToArrayAsync()
				);
        }

        public Task<ProductSpec> GetSpec(long Id)
        {
			return DataScope.Use("获取规格", ctx =>
				 MapSpec(
				ctx.Set<DataModels.DataProductSpec>().AsQueryable()
				 .Where(s => s.Id == Id && s.LogicState == EntityLogicState.Enabled)
				 ).SingleOrDefaultAsync()
				);

        }
		
		protected override IQueryable<DataModels.DataProduct> OnBuildQuery(IQueryable<DataModels.DataProduct> Query, ProductInternalQueryArgument Arg)
		{
			Query = Query
				   .Filter(Arg.LogicState, p => p.LogicState)
				   .Filter(Arg.ProductTypeId, p => p.TypeId)
				   .Filter(Arg.UpdatedTime, p => p.UpdatedTime)
				   .Filter(Arg.Price, p => p.Price)
				   .FilterContains(Arg.Name, p => p.Name);
			return Query;
		}
		protected override PagingQueryBuilder<DataModels.DataProduct> PagingQueryBuilder { get; } = new PagingQueryBuilder<DataModels.DataProduct>(
			"time",
			b => b.Add("time", p => p.PublishedTime)
		);
	}
}
