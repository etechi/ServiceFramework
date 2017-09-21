using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Biz.Products.Entity.DataModels;
using System.Linq.Expressions;
using SF.Entities;
using SF.Data;

namespace SF.Biz.Products.Entity
{
	public class CategoryManager<TEditable, TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem,TItem,TProductSpec> :
		EntityManager<long,TEditable, CategoryQueryArgument, TEditable, TCategory>,
		ICategoryManager<TEditable>
		where TEditable : CategoryInternal,  new()
		where TProduct : Product<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TProductDetail : ProductDetail<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TProductType : ProductType<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TCategory : Category<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>,new()
		where TCategoryItem : CategoryItem<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>,new()
		where TPropertyScope : PropertyScope<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TProperty : Property<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TPropertyItem : PropertyItem<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TItem : Item<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
        where TProductSpec:ProductSpec<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem, TProductSpec>
    {
		public IItemNotifier Notifier { get; }
		public CategoryManager(IDataSetEntityManager<TCategory> EntityManager, IItemNotifier Notifier) :
			base(EntityManager)
		{
			this.Notifier = Notifier;
		}
        protected override async Task<TEditable> OnMapModelToEditable(IContextQueryable<TCategory> query)
		{
			return await BatchMapModelToEditable(query).SingleOrDefaultAsync();
		}
		public  IContextQueryable<TEditable> BatchMapModelToEditable(IContextQueryable<TCategory> query)
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
		protected override IContextQueryable<TCategory> OnBuildQuery(IContextQueryable<TCategory> Query, CategoryQueryArgument Arg, Paging paging)
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
		async Task UpdateChildren(TCategory Parent, CategoryInternal[] Items)
		{
			var org_items = await DataSet.LoadTreeChildren(
				Parent,
				false,
				(q, c) => q.Where(i => i.ParentId == c.Id)
				);
			var time = Now;
			var Removed = new List<TCategory>();
			var Updated = new List<long>();
			var re= DataSet.MergeTree(
				org_items,
				Items,
				m => m.Id,
				e => e.Id,
				e => e.ParentId ?? 0,
				e => e.Children?.Cast<TEditable>(),
				(e, p,cs) =>
				{
					var n = new TCategory
					{
						OwnerUserId = Parent.OwnerUserId,
						CreatedTime = time,
						Parent = p,
						Children=cs
					};
					OnUpdateModel(n, e , time);
					return n;
				},
				(m, e) =>
				{
					OnUpdateModel(m, e, time);
					Updated.Add(m.Id);
				},
				m => { Removed.Add(m); }
				);

			foreach(var c in Removed)
				await DataSet.Context.Set< TCategoryItem>().RemoveRangeAsync(i => i.CategoryId == c.Id);
			await DataSet.Context.SaveChangesAsync();

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
			//return BatchMapModelToEditable(re.AsQueryable(DataSet.Context.Provider)).ToArray();
		}
        public async Task<long[]> LoadItems(long CategoryId)
        {
            var items = await (
                from ci in DataContext.Set<TCategoryItem>().AsQueryable()
                where ci.CategoryId == CategoryId
                let item = ci.Item
                orderby ci.Order ascending
                select item.Id
                ).ToArrayAsync();
            return items;
        }
        async Task UpdateItems(TCategory Category, long[] Items)
		{
			Items = Items.Distinct().ToArray();
			var itemSet = DataContext.Set<TCategoryItem>();
			var cur_items = await itemSet
				.AsQueryable(false)
				.Where(ci => ci.CategoryId == Category.Id)
				.ToArrayAsync();
			itemSet.Merge(
				cur_items,
				Items.Select((i, idx) => new { order = idx, id = i }),
				(o, e) => o.ItemId.Equals(e.id),
				n => new TCategoryItem { CategoryId=Category.Id,ItemId=n.id,Order=n.order},
				(o, n) =>
				{
					o.Order = n.order;
				});
			//var c = await DataSet.FindAsync(CategoryId);
			Category.ItemCount = Items.Length;
			//DataSet.Update(c);
			//await DataContext.SaveChangesAsync();
			EntityManager.AddPostAction(
				() =>Notifier.NotifyCategoryItemsChanged(Category.Id),
				PostActionType.AfterCommit
				);
		}
        protected override IContextQueryable<TEditable> OnMapModelToDetail(IContextQueryable<TCategory> Query)
		{
			return BatchMapModelToEditable(Query);
		}
		//public override Task<TEditable> MapModelToEditable(IContextQueryable<TCategory> Query)
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

            EntityManager.AddPostAction(() =>
            {
                foreach (var tag in ((orgTags ?? "") + ";" + (newTags ?? ";")).SplitAndNormalizae(';'))
                    Notifier.NotifyCategoryTag(tag);
            },
			PostActionType.AfterCommit
			);
			if (npid != opid)
			{
				await DataSet.ValidateTreeParent(
					"商品分类", 
					Model.Id,
					npid,
					pnt => DataSet.AsQueryable()
						.Where(c => c.Id == pnt)
						.Select(c => c.ParentId.HasValue?c.ParentId.Value:0)
					);

				EntityManager.AddPostAction(() =>
				{
					if (opid != 0)
						Notifier.NotifyCategoryChildrenChanged(opid);
					if (npid != 0 && npid != opid)
						Notifier.NotifyCategoryChildrenChanged(npid);
				},
				PostActionType.AfterCommit);
			}
			if(Model.Id!=0)
				EntityManager.AddPostAction(
					() =>Notifier.NotifyCategoryChanged(Model.Id),
					PostActionType.AfterCommit
					);
			OnUpdateModel(Model, obj, EntityManager.Now);

			if(obj.Children!=null)
				await UpdateChildren(Model, obj.Children);

			if (obj.Items != null)
				await UpdateItems(Model, obj.Items);



			Model.OwnerUserId = obj.SellerId;
		}
		protected override Task OnNewModel(IModifyContext ctx)
		{
			var Model = ctx.Model;
			Model.CreatedTime = EntityManager.Now;
			
			return Task.CompletedTask;
		}
		protected override Task<TCategory> OnLoadModelForUpdate(long Id, IContextQueryable<TCategory> ctx)
		{
			return DataSet.AsQueryable(false)
				.Where(s => s.Id.Equals(Id))
				.Include(s => s.Items)
				.SingleOrDefaultAsync();
		}
		

        protected override Task OnRemoveModel(IModifyContext ctx)
		{
			var Model = ctx.Model;
			DataContext.Set<TCategoryItem>().RemoveRange(Model.Items);
            var tags = Model.Tag;
			EntityManager.AddPostAction(() =>
			{
				Notifier.NotifyCategoryChanged(Model.Id);
				Notifier.NotifyCategoryChildrenChanged(Model.Id);
				Notifier.NotifyCategoryItemsChanged(Model.Id);
                foreach (var tag in tags.SplitAndNormalizae(';'))
                    Notifier.NotifyCategoryTag(tag);
            },
			PostActionType.AfterCommit
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
