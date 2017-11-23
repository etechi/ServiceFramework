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
	public class ProductManager :
		ProductManager<ProductInternal, ProductEditable, DataModels.Product, DataModels.ProductDetail, DataModels.ProductType, DataModels.Category, DataModels.CategoryItem, DataModels.PropertyScope, DataModels.Property, DataModels.PropertyItem, DataModels.Item, DataModels.ProductSpec>,
		IProductManager
	{
		public ProductManager(IEntityServiceContext ServiceContext, Lazy<IItemNotifier> ItemNotifier) : base(ServiceContext, ItemNotifier)
		{
		}
	}
	public class ProductManager<TInternal, TEditable, TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec> :
		ModidifiableEntityManager<ObjectKey<long>, TInternal, ProductInternalQueryArgument, TEditable, TProduct>,
		IProductManager<TInternal, TEditable>
        where TInternal : ProductInternal, new()
		where TEditable : ProductEditable, new()
		where TProduct : DataModels.Product<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>,new()
		where TProductDetail : DataModels.ProductDetail<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>,new()
		where TProductType : DataModels.ProductType<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TCategory : DataModels.Category<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TCategoryItem : DataModels.CategoryItem<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>,new()
		where TPropertyScope : DataModels.PropertyScope<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TProperty : DataModels.Property<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TPropertyItem : DataModels.PropertyItem<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TItem : DataModels.Item<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>,new()
        where TProductSpec : DataModels.ProductSpec<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem, TProductSpec>,new()
    {
		public Lazy<IItemNotifier> ItemNotifier { get; }
        public ProductManager(
			IEntityServiceContext ServiceContext, 
			Lazy<IItemNotifier> ItemNotifier
            ) : base(ServiceContext)
		{
			this.ItemNotifier = ItemNotifier;
		}
        protected override IContextQueryable<TInternal> OnMapModelToDetail(IContextQueryable<TProduct> Query)
		{
			return from c in Query
				   select new TInternal
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
					   ObjectState = c.ObjectState,
					   ProductTypeId=c.TypeId
					   
				   };
		}

		protected virtual Task OnInitEditable(TEditable editable,TProduct product)
		{
			return Task.CompletedTask;
		}
        protected override async Task<TEditable> OnMapModelToEditable(IContextQueryable<TProduct> Query)
		{
			var q = from m in Query
					select new {
						product = m,
  						detail = m.Detail,
						cats= from it in m.Items
							  where it.SellerId==m.OwnerUserId
							  from cit in it.CategoryItems
							  select cit.CategoryId,
                              
                        Specs = m.Specs.Select(i =>
                              new ProductSpecDetail
                              {
                                  Id = i.Id,
                                  Name = i.Name,
                                  CreatedTime = i.CreatedTime,
                                  Desc = i.Desc,
                                  Image = i.Image,
                                  ObjectState = i.ObjectState,
                                  UpdatedTime = i.UpdatedTime,
								  VIADSpecId=i.VIADSpecId
                              })
                    };

			var data=await q.SingleOrDefaultAsync();
			if (data == null) return null;
			var e= new TEditable
			{
				Id= data.product.Id,
				Title= data.product.Title,
				MarketPrice= data.product.MarketPrice,
				Price= data.product.Price,
                IsVirtual=data.product.IsVirtual,
                CouponDisabled=data.product.CouponDisabled,
				Image= data.product.Image,
				Content=new ProductContent
				{
					Descs= Json.Parse<ProductDescItem[]>(data.detail?.Detail),
					Images= Json.Parse<ProductImage[]>(data.detail?.Images)
				},
				Name= data.product.Name,
				TypeId= data.product.TypeId,
				OwnerUserId= data.product.OwnerUserId,
				PublishedTime= data.product.PublishedTime,
				ObjectState= data.product.ObjectState,
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
			Model.OwnerUserId = obj.OwnerUserId;
			Model.Detail.Detail = Json.Stringify(obj.Content.Descs);
			Model.Detail.Images = Json.Stringify(obj.Content.Images);
			Model.UpdatedTime = Now;
			Model.ObjectState = obj.ObjectState;
			Model.VIADSpecId = obj.VIADSpecId;
			if (Model.TypeId != obj.TypeId)
			{
				if (Model.ObjectState == EntityLogicState.Enabled)
				{
					if(Model.TypeId!=0)
						await UpdateTypeProductCount(Model.TypeId, -1);
					await UpdateTypeProductCount(obj.TypeId, 1);

				}
				Model.TypeId = obj.TypeId;
			}
			DataContext.Set<TProductDetail>().Update(Model.Detail);

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

            DataContext.Set<TProductSpec>().Merge(
                Model.Specs,
                obj.Specs,
                (m, e) => m.Id == e.Id,
                e => new TProductSpec
                {
                    ProductId = Model.Id,
                    CreatedTime = Now,
                    Name = e.Name,
                    Desc = e.Desc,
                    Image = e.Image,
                    ObjectState = e.ObjectState,
                    UpdatedTime = Now,
					VIADSpecId=e.VIADSpecId
                },
                (m, e) =>
                {
                    m.Id = e.Id;
                    m.Name = e.Name;
                    m.Desc = e.Desc;
                    m.Image = e.Image;
                    m.ObjectState = e.ObjectState;
                    m.UpdatedTime = Now;
					m.VIADSpecId = e.VIADSpecId;

				}
                );

            if (Model.Id!=0)
			{
				var notifier = this.ItemNotifier.Value;
				ServiceContext.DataContext.TransactionScopeManager.AddCommitTracker(
					TransactionCommitNotifyType.AfterCommit,
					(t, ex) =>
					{ 
						notifier.NotifyProductChanged(Model.Id);
						notifier.NotifyProductContentChanged(Model.Id);
					}
					);

				var defaultItemId = await DataContext
					.Set<TItem>().AsQueryable()
					.Where(i => i.ProductId == Model.Id && i.SellerId == Model.OwnerUserId)
					.Select(i => i.Id)
					.SingleOrDefaultAsync();
				if (defaultItemId != 0)
				{
					var cats = await DataContext
						.Set<TCategoryItem>().AsQueryable(false)
						.Where(ci => ci.ItemId == defaultItemId)
						.ToArrayAsync();

					var Removed = new List<long>();
					var Added = new List<long>();
					DataContext.Set<TCategoryItem>().Merge(
						cats,
						obj.CategoryIds,
						(m, e) => m.CategoryId == e,
						e =>
						{
							Added.Add(e);
							return new TCategoryItem { CategoryId = e, ItemId = defaultItemId };
						},
						null,
						(e) =>
						{
							Removed.Add(e.CategoryId);
                            DataContext.Remove(e);
						}
						);
					ServiceContext.DataContext.TransactionScopeManager.AddCommitTracker(
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
			Model.Id = await IdentGenerator.GenerateAsync<TProduct>();
			Model.Detail = new TProductDetail();
			Model.ObjectState = EntityLogicState.Disabled;
			Model.CreatedTime = Now;
			var itemId = await IdentGenerator.GenerateAsync<TItem>();
			Model.Items = new[]
			{
				new TItem
				{
					Id= itemId,
					SellerId =obj.OwnerUserId,
					CreatedTime=Model.CreatedTime,
					UpdatedTime=Model.CreatedTime,
					CategoryItems=obj.CategoryIds==null?
						null:
						obj.CategoryIds.Select(
							cid=>new TCategoryItem { CategoryId =cid,ItemId=itemId}
							)
						.ToArray()
				}
			};
            var notifier = this.ItemNotifier.Value;
            if(obj.CategoryIds!=null)
				ServiceContext.DataContext.TransactionScopeManager.AddCommitTracker(
					TransactionCommitNotifyType.AfterCommit,
					(t, ex) =>
					{ 
						foreach (var cid in obj.CategoryIds)
								notifier.NotifyCategoryItemsChanged(cid);
					});
		}
		protected override IContextQueryable<TProduct> OnLoadChildObjectsForUpdate(ObjectKey<long> Id, IContextQueryable<TProduct> query)
		{
			return query.Include(p => p.Detail).Include(p => p.Specs);
		}
		

		async Task UpdateTypeProductCount(long type,int diff)
		{
			var t = await DataContext.Set<TProductType>().FindAsync(type);
			if (t == null)
				throw new ArgumentException("产品类型无效:" + type);
			t.ProductCount = Math.Max(0, t.ProductCount + diff);
			DataContext.Set<TProductType>().Update(t);
		}
		protected override async Task OnRemoveModel(IModifyContext ctx)
		{
			var Id = ctx.Editable.Id;
			var prd = await DataSet.AsQueryable(false).Where(p => p.Id.Equals(Id))
				.Include(p => p.Detail)
				.Include(p => p.PropertyItems)
				.Include(p => p.Specs)
				.Include(p => p.Items.Select(i => i.CategoryItems))
				.SingleOrDefaultAsync();
			if (prd == null)
				return;

			var cids = prd.Items.SelectMany(i => i.CategoryItems).Select(ci => ci.CategoryId).ToArray();


			if (prd.ObjectState == EntityLogicState.Enabled)
				await UpdateTypeProductCount(prd.TypeId, -1);

			foreach (var item in prd.Items)
				DataContext.RemoveRange(item.CategoryItems);


			DataContext.RemoveRange(prd.Items);
			DataContext.RemoveRange(prd.PropertyItems);
			DataContext.RemoveRange(prd.Specs);
			DataContext.Remove(prd.Detail);

			var notifier = this.ItemNotifier.Value;
			ServiceContext.DataContext.TransactionScopeManager.AddCommitTracker(
				TransactionCommitNotifyType.AfterCommit,
				(t, ex) =>
				{
					foreach (var ci in cids)
						notifier.NotifyCategoryItemsChanged(ci);
				});
			await base.OnRemoveModel(ctx);
		}
		

        static PagingQueryBuilder<TProduct> pagingBuilder = new PagingQueryBuilder<TProduct>(
			"updated",
			i => i
			.Add("visited", c => c.Visited,true)
			.Add("sells", c => c.SellCount,true)
			.Add("updated", c => c.UpdatedTime, true)
			.Add("created", c => c.CreatedTime, true)
			);
		protected virtual Task OnObjectStateChanged(TProduct Model, EntityLogicState OrgState)
		{
			return Task.CompletedTask;
		}
		protected virtual Task OnObjectStateChangeSaved(TProduct Model, EntityLogicState OrgState)
		{
			return Task.CompletedTask;
		}
		public virtual async Task SetLogicState(long Id, EntityLogicState State)
		{
			var e = await DataSet.FindAsync(Id);
			var orgState = e.ObjectState;
			if (orgState == State)
				return;
			
			e.ObjectState = State;
			e.UpdatedTime = Now;
			if (State == EntityLogicState.Enabled)
				e.PublishedTime = e.UpdatedTime;
			DataSet.Update(e);
			await OnObjectStateChanged(e, orgState);


			if (State == EntityLogicState.Enabled)
				await UpdateTypeProductCount(e.TypeId, 1);
			else if (orgState == EntityLogicState.Enabled)
				await UpdateTypeProductCount(e.TypeId, -1);

			await DataContext.SaveChangesAsync();
			await OnObjectStateChangeSaved(e, orgState);
		}

		//public override async Task<int> CreateAsync(TEditable obj)
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
		//public override async Task<TProduct> DeleteAsync(int Id)
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
		//public override async Task<TProduct> UpdateAsync(TEditable obj)
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

		
		//public async Task<QueryResult<TInternal>> Query(ProductInternalQueryArgument args,Paging paging)
		//{
		//	var q = OnBuildQuery(Context.ReadOnly<TProduct>(),args);
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
        //                var tmps = await MapModelToInternal(Context.ReadOnly<TProduct>().Where(a => ids.Contains(a.Id))).ToArrayAsync();
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
        //                var tmps = await MapSpec(Context.ReadOnly<TProductSpec>().Where(a => ids.Contains(a.Id))).ToArrayAsync();
        //                return tmps;
        //            }
        //            );
        //        return re;
        //    }
        //    else
        //        return Empty.Array<IDataObject>();
        //}
        IContextQueryable<ProductSpec> MapSpec(IContextQueryable<TProductSpec> query)
        {
            return query.Select(s => new ProductSpec
            {
                Desc = s.Desc,
                Id = s.Id,
                Image = s.Image,
                Name = s.Name,
				VIADSpecId=s.VIADSpecId
            });
        }

        public async Task<ProductSpec[]> ListSpec(long Id)
        {
            return await MapSpec(
                DataContext.Set<TProductSpec>().AsQueryable()
                .Where(s => s.ProductId == Id && s.ObjectState == EntityLogicState.Enabled)
                .OrderBy(s => s.Order)
                ).ToArrayAsync();
        }

        public async Task<ProductSpec> GetSpec(long Id)
        {
            return await MapSpec(
			   DataContext.Set<TProductSpec>().AsQueryable()
				.Where(s => s.Id == Id && s.ObjectState == EntityLogicState.Enabled)
                ).SingleOrDefaultAsync();
        }
		
		protected override IContextQueryable<TProduct> OnBuildQuery(IContextQueryable<TProduct> Query, ProductInternalQueryArgument Arg, Paging paging)
		{
			Query = Query
				   .Filter(Arg.State, p => p.ObjectState)
				   .Filter(Arg.ProductTypeId, p => p.TypeId)
				   .Filter(Arg.UpdateTime, p => p.UpdatedTime)
				   .Filter(Arg.Price, p => p.Price)
				   .FilterContains(Arg.Name, p => p.Name);
			return Query;
		}
		protected override PagingQueryBuilder<TProduct> PagingQueryBuilder { get; } = new PagingQueryBuilder<TProduct>(
			"time",
			b => b.Add("time", p => p.PublishedTime)
		);
	}
}
