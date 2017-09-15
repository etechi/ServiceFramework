using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using SF.Entities;

namespace SF.Biz.Products
{
	public class CacheItemService<TCategory,TItemSource>:
		CacheItemService<Item, TCategory, TItemSource>
		 where TCategory : ICategoryCached
		 where TItemSource : IItemSource
	{
		public CacheItemService(TItemSource ItemSource) : base(ItemSource)
		{
		}

		protected override Item CreateItemResult(IItemCached Item, IProductCached Product, IProductContentCached Content)
		{
			return new Item(Item, Product, Content);
		}
	}
	public abstract class CacheItemService<TItem, TCategory, TItemSource> :
		 IItemService<TItem, TCategory>,
		 IItemNotifier
		 where TItem : IItem
		 where TCategory : ICategoryCached
		 where TItemSource : IItemSource
	{
		ConcurrentDictionary<long, IItemCached> items { get; } = new ConcurrentDictionary<long, IItemCached>();
		ConcurrentDictionary<long, IProductCached> products { get; } = new ConcurrentDictionary<long, IProductCached>();
		ConcurrentDictionary<long, IProductContentCached> productContents { get; } = new ConcurrentDictionary<long, IProductContentCached>();
		ConcurrentDictionary<long, TCategory> categories { get; } = new ConcurrentDictionary<long, TCategory>();
		ConcurrentDictionary<string, long[]> taggedCategories { get; } = new ConcurrentDictionary<string, long[]>();
		ConcurrentDictionary<long, long[]> category_children { get; } = new ConcurrentDictionary<long, long[]>();
		ConcurrentDictionary<long, long[]> category_items { get; } = new ConcurrentDictionary<long, long[]>();
		ObjectSyncQueue<long> CategorySyncQueue { get; } = new ObjectSyncQueue<long>();
		ObjectSyncQueue<long> ItemContentSyncQueue { get; } = new ObjectSyncQueue<long>();

		public TItemSource ItemSource { get; }

		public int MainCategoryId => ItemSource.MainCategoryId;
		public CacheItemService(TItemSource ItemSource)
		{
			this.ItemSource = ItemSource;
		}


		static PagingQueryBuilder<TItem> itemPagingBuilder = new PagingQueryBuilder<TItem>(
			"hot",
			i =>
			  i.Add("new", p => p.PublishedTime, true)
			  .Add("hot", p => p.Visited, true)
			  .Add("price", p => p.Price)
			  .Add("order", (q, a) => q)
		);
		public virtual PagingQueryBuilder<TItem> ItemPagingBuilder { get { return itemPagingBuilder; } }


		static PagingQueryBuilder<TCategory> catPagingBuilder = new PagingQueryBuilder<TCategory>(
			"order",
			i =>
			  i.Add("order", p => p.Order)
		);
		public virtual PagingQueryBuilder<TCategory> CategoryPagingBuilder { get { return catPagingBuilder; } }

		public async Task<QueryResult<TCategory>> ListCategories(long ParentCategoryId, Paging paging)
		{
			var cids = await GetRelationIds(ParentCategoryId, category_children, ItemSource.CategoryChildrenLoader);
			var cats = await GetCategories(cids);
			var re = cats.AsQueryable<TCategory>().ToQueryResult(
				pq => pq,
				r => r,
				CategoryPagingBuilder,
				paging
				);
			return re;
		}
		public async Task<TCategory[]> GetCategories(long[] CategoryIds)
		{
			return await GetObjects<ICategoryCached, TCategory>(CategoryIds, categories, ItemSource.CategoryLoader);
		}
		public async Task<long[]> GetCategoryIds(string Tag)
		{
			long[] ids;
			if (!taggedCategories.TryGetValue(Tag, out ids))
			{
				ids = await ItemSource.TaggedCategoryLoader.Load(Tag);
				taggedCategories.GetOrAdd(Tag, ids);
			}
			return ids;
		}
		public async Task<TCategory[]> GetCategories(string Tag)
		{
			return await GetCategories(await GetCategoryIds(Tag));
		}

		async Task<long[]> GetRelationIds(long id, ConcurrentDictionary<long, long[]> dic, IRelationLoader loader)
		{
			long[] result;
			if (dic.TryGetValue(id, out result))
				return result;
			return await CategorySyncQueue.Queue(id, async () =>
			{
				if (dic.TryGetValue(id, out result))
					return result;
				result = await loader.Load(id);
				return dic.GetOrAdd(id, result);
			});
		}
		Task<T[]> GetObjects<T>(long[] ids, ConcurrentDictionary<long, T> dic, IBatchLoader<T> loader)
			where T : IEntityWithId<long>
		{
			return GetObjects<T, T>(ids, dic, loader);
		}
		async Task<T[]> GetObjects<C, T>(long[] ids, ConcurrentDictionary<long, T> dic, IBatchLoader<C> loader)
			where C : IEntityWithId<long>
		{
			var count = ids.Length;
			var result = new List<T>(count);
			var missing = new List<long>(count);
			for (var i = 0; i < count; i++)
			{
				var id = ids[i];
				T item;
				if (!dic.TryGetValue(id, out item))
					missing.Add(id);
				result.Add(item);
			}

			if (missing.Count == 0)
				return result.ToArray();

			var loaded = (await loader.Load(missing.ToArray())).ToDictionary(i => i.Id, i => (T)(object)i);
			for (var i = 0; i < count; i++)
			{
				if (result[i] != null)
					continue;

				T loadedItem;
				if (!loaded.TryGetValue(ids[i], out loadedItem))
					continue;

				result[i] = dic.GetOrAdd(ids[i], loadedItem);
			}
			return result.Where(r => r != null).ToArray();
		}
		protected abstract TItem CreateItemResult(IItemCached Item, IProductCached Product, IProductContentCached Content);
		public async Task<TItem[]> GetItems(long[] ItemIds)
		{
			var its=await GetObjects<IItemCached>(ItemIds, items, ItemSource.ItemLoader);
			var ps = (await GetObjects<IProductCached>(its.Select(i => i.ProductId).ToArray(), products, ItemSource.ProductLoader)).ToDictionary(p => p.Id);
			return its.Select(it =>
			{
				IProductCached p;
				if (ps.TryGetValue(it.ProductId, out p))
					return CreateItemResult(it, p, null);
				else
					return default(TItem);
			}).Where(i => i != null).ToArray();
		}
		public async Task<TItem[]> GetProducts(long[] ProductIds)
		{
			var ps = await GetObjects<IProductCached>(ProductIds, products, ItemSource.ProductLoader);
			return ps.Select(p => CreateItemResult(null, p, null)).Where(p=>p!=null).ToArray();
		}

		async Task ItemCollect(long cid,HashSet<long> aids)
		{
			var cids = await GetRelationIds(cid, category_items, ItemSource.CategoryItemsLoader);
			foreach (var id in cids)
				aids.Add(id);
			var ccats = await GetRelationIds(cid, category_children, ItemSource.CategoryChildrenLoader);
			foreach (var cat in ccats)
				await ItemCollect(cat, aids);
		}
      

        public async Task<QueryResult<TItem>> ListItems(string Tag, bool WithChildCategoryItems, string filter, Paging paging)
        {
            var cids = await GetCategoryIds(Tag);
            if (cids.Length == 0)
                return QueryResult<TItem>.Empty;
            if (cids.Length == 1)
                return await ListItems(cids[0], WithChildCategoryItems, filter, paging);

            var aids = new HashSet<long>();
            foreach(var cid in cids)
                await ItemCollect(cid, aids);
            return await NewItemResult(aids.ToArray(), filter, paging);
        }
        public async Task<QueryResult<TItem>> QueryAsync(ItemQueryArgument Args, Paging paging)
        {
            if (!Args.ProductId.HasValue)
            {
                if (Args.CategoryId != null)
                    return await ListItems(Args.CategoryId.Value, true, Args.Title, paging);
                if (Args.CategoryTag != null)
                    return await ListItems(Args.CategoryTag, true, Args.Title, paging);
                return await ListItems(MainCategoryId, true, Args.Title, paging);
            }
            var pi = await GetProducts(new[] { Args.ProductId.Value });
            if (pi.Length == 0)
                return QueryResult<TItem>.Empty;
            var ii = await GetItems(new[] { pi[0].MainItemId });
            if(ii==null || ii.Length==0)
                 return QueryResult<TItem>.Empty;
            return new QueryResult<TItem>
            {
                Items = ii,
                Total=1
            };
		}
		public async Task<QueryResult<long>> QueryIdentsAsync(ItemQueryArgument Args, Paging paging)
		{
			var re = await QueryAsync(Args, paging);
			return new QueryResult<long>
			{
				Items = re.Items.Select(i => i.ItemId).ToArray(),
				Summary = re.Summary,
				Total = re.Total
			};
		}

		async Task<QueryResult<TItem>> NewItemResult(long[] cids,string filter, Paging paging)
        {
            var items = await GetItems(cids);

            var q = items.AsQueryable();
            q = q.Where(i => i.OnSale);
            if (filter != null)
                q = q.Where(i => 
                i.Title.IndexOf(
                    filter,
                    StringComparison.CurrentCultureIgnoreCase
                    )!=-1
                );

            var re = q.ToQueryResult(
                pq => pq,
                r => r,
                ItemPagingBuilder,
                paging
                );
            return re;
        }
		public async Task<long[]> ListItemIdents(long CategoryId, bool WithChildCategoryItems, string filter, Paging paging)
		{
			long[] ids = null;
			if (WithChildCategoryItems)
			{
				var aids = new HashSet<long>();
				await ItemCollect(CategoryId, aids);
				ids = aids.ToArray();
			}
			else
			{
				ids = await GetRelationIds(CategoryId, category_items, ItemSource.CategoryItemsLoader);
			}
			return ids;
		}
		public async Task<QueryResult<TItem>> ListItems(long CategoryId, bool WithChildCategoryItems, string filter, Paging paging)
		{
			var ids = await ListItemIdents(CategoryId, WithChildCategoryItems, filter, paging);
			return await NewItemResult(ids,filter,paging);
		}
		

		public void NotifyItemChanged(long ItemId)
		{
			IItemCached item;
			items.TryRemove(ItemId, out item);
		}
		public void NotifyProductContentChanged(long ItemId)
		{
			IProductContentCached item;
			productContents.TryRemove(ItemId, out item);
		}
		public void NotifyProductChanged(long ItemId)
		{
			IProductCached item;
			products.TryRemove(ItemId, out item);
		}

		public void NotifyCategoryChanged(long CategoryId)
		{
			TCategory cat;
			categories.TryRemove(CategoryId, out cat);
		}

		public void NotifyCategoryChildrenChanged(long CategoryId)
		{
			long[] item;
			category_children.TryRemove(CategoryId, out item);
		}

		public void NotifyCategoryItemsChanged(long CategoryId)
		{
			long[] items;
			category_items.TryRemove(CategoryId, out items);
		}

		async Task<IProductContentCached> GetProductContent(long Id)
		{
			IProductContentCached detail;
			if (productContents.TryGetValue(Id, out detail))
				return detail;
			return await ItemContentSyncQueue.Queue<IProductContentCached>(Id, async () =>
			{
				if (productContents.TryGetValue(Id, out detail))
					return detail;
				var re=await ItemSource.ProductContentLoader.Load(new long[] { Id });
				if (re.Length == 0)
					return null;
				detail = re[0];
				return productContents.GetOrAdd(Id, detail);
			});
			
		}
		public async Task<TItem> GetItemDetail(long Id)
		{
			var its = await GetObjects<IItemCached>(new long[]{ Id}, items, ItemSource.ItemLoader);
			if (its.Length == 0) return default(TItem);

			var ps = await GetObjects<IProductCached>(new long[] { its[0].ProductId }, products, ItemSource.ProductLoader);
			if (ps.Length == 0) return default(TItem);

			var content = await GetProductContent(ps[0].Id);
			if (content == null)
				return default(TItem);
			return CreateItemResult(its[0], ps[0], content);
		}
		public async Task<TItem> GetProductDetail(long Id)
		{
			var ps = await GetObjects<IProductCached>(new long[] { Id }, products, ItemSource.ProductLoader);
			if (ps.Length == 0) return default(TItem);

			var content = await GetProductContent(Id);
			if (content == null)
				return default(TItem);
			return CreateItemResult(null, ps[0], content);

		}

        public void NotifyCategoryTag(string CategoryTag)
        {
			long[] ids;
            taggedCategories.TryRemove(CategoryTag, out ids);
        }
    }
	
}
