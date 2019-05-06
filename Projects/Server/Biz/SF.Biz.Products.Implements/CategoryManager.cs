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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Sys.Entities;
using SF.Sys.Data;
using SF.Sys.ADT;
using SF.Sys;
using SF.Sys.Linq;

namespace SF.Biz.Products.Entity
{
	public class CategoryManager :
		CategoryManager<CategoryInternal, DataModels.Product, DataModels.ProductDetail, DataModels.ProductType, DataModels.Category, DataModels.CategoryItem, DataModels.PropertyScope, DataModels.Property, DataModels.PropertyItem, DataModels.Item, DataModels.ProductSpec>,
		IProductCategoryManager
	{
		public CategoryManager(IEntityServiceContext ServiceContext, IItemNotifier Notifier) : base(ServiceContext, Notifier)
		{
		}
	}
	public class CategoryManager<TEditable, TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem,TItem,TProductSpec> :
		ModidifiableEntityManager<ObjectKey<long>,TEditable, CategoryQueryArgument, TEditable, TCategory>,
		IProductCategoryManager<TEditable>
		where TEditable : CategoryInternal,  new()
		where TProduct : DataModels.Product<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TProductDetail : DataModels.ProductDetail<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TProductType : DataModels.ProductType<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TCategory : DataModels.Category<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>,new()
		where TCategoryItem : DataModels.CategoryItem<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>,new()
		where TPropertyScope : DataModels.PropertyScope<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TProperty : DataModels.Property<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TPropertyItem : DataModels.PropertyItem<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TItem : DataModels.Item<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
        where TProductSpec: DataModels.ProductSpec<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem, TProductSpec>
    {
		public IItemNotifier Notifier { get; }
		public CategoryManager(IEntityServiceContext ServiceContext, IItemNotifier Notifier) :
			base(ServiceContext)
		{
			this.Notifier = Notifier;
		}
        protected override async Task<TEditable> OnMapModelToEditable(IDataContext ctx, IQueryable<TCategory> query)
		{
			return await BatchMapModelToEditable(query).SingleOrDefaultAsync();
		}
		public  IQueryable<TEditable> BatchMapModelToEditable(IQueryable<TCategory> query)
		{
			return from c in query
				select new TEditable
				{
					Id = c.Id,
					Tag = c.Tag,
					ParentId = c.ParentId,
					Order = c.Order,
					Name = c.Name,
					Title = c.Title,
					Icon = c.Icon,
                    BannerUrl=c.BannerUrl,
                    BannerImage = c.BannerImage,
                    MobileBannerUrl =c.MobileBannerUrl,
                    MobileBannerImage=c.MobileBannerImage,
					SellerId=c.OwnerUserId,
					Image = c.Image,
					ObjectState = c.ObjectState,
					Description = c.Description,
					ParentName=c.ParentId.HasValue?c.Parent.Name:null
				};
		}
		protected override IQueryable<TCategory> OnBuildQuery(IQueryable<TCategory> Query, CategoryQueryArgument Arg)
		{
			return Query.Filter(Arg.SellerId, c => c.OwnerUserId)
				.Filter(Arg.ParentId,c=>c.ParentId)
				.Filter(Arg.ObjectState, c => c.ObjectState)
				.Filter(Arg.Name,c=>c.Name);
		}
		

		protected virtual IEnumerable<TCategory> MapEditableToModel(IEnumerable<TEditable> items)
		{
			return items.Select(c => new TCategory
			{
				Id = c.Id,
				Tag = c.Tag,
				ParentId = c.ParentId,
				Order = c.Order,
				Name = c.Name,
				Title = c.Title,
				Description = c.Description,
                BannerUrl=c.BannerUrl,
                BannerImage = c.BannerImage,
                MobileBannerUrl =c.MobileBannerUrl,
                MobileBannerImage = c.MobileBannerImage,
                Icon = c.Icon,
				Image = c.Image,
				ObjectState = c.ObjectState,
				Children=MapEditableToModel(c.Children.Cast<TEditable>()).ToArray()
			});
		}
		protected virtual void OnUpdateModel(TCategory Model,CategoryInternal Editable,DateTime  time)
		{
			
			Model.Tag = Editable.Tag;
			Model.ParentId = Editable.ParentId;
			Model.Order = Editable.Order;
			Model.Name = Editable.Name;
			Model.Title = Editable.Title;
			Model.Description = Editable.Description;
			Model.Icon = Editable.Icon;
            Model.BannerUrl = Editable.BannerUrl;
            Model.BannerImage = Editable.BannerImage;
            Model.MobileBannerUrl = Editable.MobileBannerUrl;
            Model.MobileBannerImage = Editable.MobileBannerImage;
            Model.Image = Editable.Image;
			Model.UpdatedTime = time;
			Model.ObjectState = Editable.ObjectState;
			
		}
		async Task UpdateChildren(IDataContext ctx,TCategory Parent, CategoryInternal[] Items)
		{
			var dataSet = ctx.Set<TCategory>();
			var org_items = await dataSet.LoadTreeChildren(
				Parent,
				false,
				(q, c) => q.Where(i => i.ParentId == c.Id)
				);
			var time = Now;
			var Removed = new List<TCategory>();
			var Updated = new List<long>();

			foreach (var n in Tree.AsEnumerable(Items, ii => ii.Children).Where(n => n.Id == 0))
				n.Id = await IdentGenerator.GenerateAsync<TCategory>();

			var re= dataSet.MergeTree(
				Parent,
				org_items,
				Items,
				m => m.Id,
				e => e.Id,
				e => e.ParentId ?? 0,
				e => e.Children?.Cast<TEditable>(),
				(e, p) =>
				{
					var n = new TCategory
					{
						Id=e.Id,
						OwnerUserId = Parent.OwnerUserId,
						CreatedTime = time,
					};
					e.ParentId = p?.Id;
					OnUpdateModel(n, e , time);
					return n;
				},
				(m, e, np) =>
				{
					e.ParentId = np.Id;
					OnUpdateModel(m, e, time);
					Updated.Add(m.Id);
				},
				m => { Removed.Add(m); }
				);

			foreach(var c in Removed)
				await ctx.Set< TCategoryItem>().RemoveRangeAsync(i => i.CategoryId == c.Id);
			ctx.AddCommitTracker(
				TransactionCommitNotifyType.AfterCommit,
				(t, ex) =>
				{
					foreach (var c in Removed)
					{
						Notifier.NotifyCategoryChanged(c.Id);
						Notifier.NotifyCategoryChildrenChanged(c.Id);
						Notifier.NotifyCategoryItemsChanged(c.Id);
					}
					foreach (var id in Updated)
					{
						Notifier.NotifyCategoryChanged(id);
						Notifier.NotifyCategoryChildrenChanged(id);
					}
				}
			);
			//return BatchMapModelToEditable(re.AsQueryable(DataSet.Context.Provider)).ToArray();
		}
        public Task<long[]> LoadItems(long CategoryId)
        {
			return DataScope.Use("载入项目", ctx =>
				 (from ci in ctx.Set<TCategoryItem>().AsQueryable()
				  where ci.CategoryId == CategoryId
				  let item = ci.Item
				  orderby ci.Order ascending
				  select item.Id
				 ).ToArrayAsync()
				 );
        }
        async Task UpdateItems(IDataContext ctx,TCategory Category, long[] Items)
		{
			Items = Items.Distinct().ToArray();
			var itemSet = ctx.Set<TCategoryItem>();
			var cur_items = await itemSet
				.AsQueryable(false)
				.Where(ci => ci.CategoryId == Category.Id)
				.ToArrayAsync();
			itemSet.Merge(
				cur_items,
				Items.Select((i, idx) => new { order = idx, id = i }),
				(o, e) => o.ItemId.Equals(e.id),
				n => new TCategoryItem {
					
					CategoryId =Category.Id,
					ItemId =n.id,
					Order =n.order
				},
				(o, n) =>
				{
					o.Order = n.order;
				});
			//var c = await DataSet.FindAsync(CategoryId);
			Category.ItemCount = Items.Length;
			//DataSet.Update(c);
			//await DataContext.SaveChangesAsync();
			ctx.AddCommitTracker(
				TransactionCommitNotifyType.AfterCommit,
				(t, ex) =>
				{
					Notifier.NotifyCategoryItemsChanged(Category.Id);
				});
		}
        protected override IQueryable<TEditable> OnMapModelToDetail(IQueryable<TCategory> Query)
		{
			return BatchMapModelToEditable(Query);
		}
		//public override Task<TEditable> MapModelToEditable(IQueryable<TCategory> Query)
		//{
		//	return (from c in Query
		//			select new TEditable
		//			{
		//				Id = c.Id,
		//				Tag = c.Tag,
		//				ParentId = c.ParentId,
		//				Order = c.Order,
		//				Name = c.Name,
		//				Title = c.Title,
		//				Icon = c.Icon,
		//				Image = c.Image,
		//				ObjectState = c.ObjectState,
		//				Description = c.Description
		//			}
		//		}).SingleOrDefaultAsync();
		//}
		protected virtual void OnUpdateCollectionItem(TCategoryItem Model, TEditable obj, TCategoryItem cur)
		{
			Model.CategoryId = cur.CategoryId;
			Model.ItemId = cur.ItemId;
			Model.Order = cur.Order;
		}
		protected override async Task OnUpdateModel(IModifyContext ctx)
		{
			var Model = ctx.Model;
			var obj = ctx.Editable;
			var npid = obj.ParentId ?? 0;
			var opid = Model.ParentId ??0;

            obj.Tag = obj.Tag.SplitAndNormalizae(';').Join(";");

            var orgTags = Model.Tag;
            var newTags = ctx.Editable.Tag;

			ctx.DataContext.AddCommitTracker(
				TransactionCommitNotifyType.AfterCommit,
				(t, ex) =>
				{
					foreach (var tag in ((orgTags ?? "") + ";" + (newTags ?? ";")).SplitAndNormalizae(';'))
						Notifier.NotifyCategoryTag(tag);
				});
			if (npid != opid)
			{
				var cat = ctx.DataContext.Set<TCategory>();
				await cat.ValidateTreeParent(
					"商品分类", 
					Model.Id,
					npid,
					pnt => cat.AsQueryable()
						.Where(c => c.Id == pnt)
						.Select(c => c.ParentId.HasValue?c.ParentId.Value:0)
					);

				ctx.DataContext.AddCommitTracker(
					TransactionCommitNotifyType.AfterCommit,
					(t, ex) =>
					{
						if (opid != 0)
							Notifier.NotifyCategoryChildrenChanged(opid);
						if (npid != 0 && npid != opid)
							Notifier.NotifyCategoryChildrenChanged(npid);
					}
					);
			}
			if(Model.Id!=0)
				ctx.DataContext.AddCommitTracker(
					TransactionCommitNotifyType.AfterCommit,
					(t, ex) =>
					{
						Notifier.NotifyCategoryChanged(Model.Id);
					}
					);
			OnUpdateModel(Model, obj, ServiceContext.Now);

			if(obj.Children!=null)
				await UpdateChildren(ctx.DataContext, Model, obj.Children);

			if (obj.Items != null)
				await UpdateItems(ctx.DataContext, Model, obj.Items);



			Model.OwnerUserId = obj.SellerId;
		}
		protected override async Task OnNewModel(IModifyContext ctx)
		{
			var Model = ctx.Model;
			Model.CreatedTime = Now;
			Model.Id = await IdentGenerator.GenerateAsync<TCategory>();
		}
		
		protected override Task<TCategory> OnLoadModelForUpdate(ObjectKey<long> Id, IQueryable<TCategory> ctx)
		{
			//DataSet.AsQueryable(false)
			//	.Where(s => s.Id.Equals(Id.Id))
			return ctx
				.Include(s => s.Items)
				.SingleOrDefaultAsync();
		}
		

        protected override Task OnRemoveModel(IModifyContext ctx)
		{
			var Model = ctx.Model;
			ctx.DataContext.Set<TCategoryItem>().RemoveRange(Model.Items);
            var tags = Model.Tag;
			ctx.DataContext.AddCommitTracker(
				TransactionCommitNotifyType.AfterCommit,
				(t, ex) =>
				{
					Notifier.NotifyCategoryChanged(Model.Id);
					Notifier.NotifyCategoryChildrenChanged(Model.Id);
					Notifier.NotifyCategoryItemsChanged(Model.Id);
					foreach (var tag in tags.SplitAndNormalizae(';'))
						Notifier.NotifyCategoryTag(tag);
			}
			);
			return base.OnRemoveModel(ctx);
		}

		//async Task<IDataObject[]> IDataObjectLoader.Load(string Type, string[][] Keys)
		//{
		//    return await DataObjectLoader.Load(
		//       Keys,
		//       id => int.Parse(id[0]),
		//       id => FindByIdAsync(id),
		//       async (ids) => {
		//           var tmps = await MapModelToInternal(
		//               Context.ReadOnly<TCategory>().Where(a => ids.Contains(a.Id))
		//               ).ToArrayAsync();
		//           return await OnPrepareInternals(tmps);
		//       }
		//       );
		//}
		//public override async Task<int> CreateAsync(TEditable obj)
		//{
		//	var re=await base.CreateAsync(obj);
		//	if (obj.ParentId.HasValue)
		//		Notifier.NotifyCategoryChildrenChanged(obj.ParentId.Value);
		//	return re;
		//}
		//public override async Task<TCategory> UpdateAsync(TEditable obj)
		//{
		//	var cat = await Context.Editable<TCategory>().FindAsync(obj.Id);
		//	var orgParentId = cat.ParentId ?? 0;
		//	var newParentId = obj.ParentId ?? 0;
		//	var pntChildChanged = cat.ObjectState != obj.ObjectState || orgParentId!=newParentId;
		//	var re=await base.UpdateAsync(obj);
		//	if(pntChildChanged)
		//	{
		//		if(newParentId!=0)
		//			Notifier.NotifyCategoryChildrenChanged(newParentId);
		//		if(orgParentId!=0 && orgParentId != newParentId)
		//			Notifier.NotifyCategoryChildrenChanged(orgParentId);
		//	}
		//	Notifier.NotifyCategoryChanged(obj.Id);
		//	return re;
		//}
		//public override async Task<TCategory> DeleteAsync(int Id)
		//{
		//	var re=await base.DeleteAsync(Id);
		//	if (re == null)
		//		return re;
		//	if(re.ParentId.HasValue)
		//		Notifier.NotifyCategoryChildrenChanged(re.ParentId.Value);
		//	Notifier.NotifyCategoryChanged(Id);
		//	return re;
		//}
		
		protected override PagingQueryBuilder<TCategory> PagingQueryBuilder => new PagingQueryBuilder<TCategory>(
			"updated",
			i => i
			.Add("name", c => c.Name)
			.Add("updated", c => c.UpdatedTime, true)
			.Add("created", c => c.CreatedTime, true)
			);
		

		//public async Task<string[]> Categories()
		//{
		//	return await Set.Select(c => c.Category).Distinct().ToArrayAsync();
		//}

	}
}
