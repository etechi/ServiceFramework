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
using System.Collections.Concurrent;
using System.Threading;
using SF.Sys.Entities;
using SF.Sys.Threading;
using SF.Biz.Trades;
using SF.Sys;

namespace SF.Biz.Products
{
	public class CacheItemService :
		 IItemService,
		 IItemNotifier,
         ITradableItemResolver
    {
		ConcurrentDictionary<long, IItemCached> items { get; } = new ConcurrentDictionary<long, IItemCached>();
		ConcurrentDictionary<long, IProductCached> products { get; } = new ConcurrentDictionary<long, IProductCached>();
		ConcurrentDictionary<long, ICategoryCached> categories { get; } = new ConcurrentDictionary<long, ICategoryCached>();
		ConcurrentDictionary<string, long[]> taggedCategories { get; } = new ConcurrentDictionary<string, long[]>();
		ConcurrentDictionary<long, long[]> category_children { get; } = new ConcurrentDictionary<long, long[]>();
		ConcurrentDictionary<long, long[]> category_items { get; } = new ConcurrentDictionary<long, long[]>();
		ObjectSyncQueue<long> CategorySyncQueue { get; } = new ObjectSyncQueue<long>();
		ObjectSyncQueue<long> ItemContentSyncQueue { get; } = new ObjectSyncQueue<long>();

		public Func<IItemSource> ItemSource { get; }

		public long MainCategoryId => ItemSource().MainCategoryId;
		public CacheItemService(Func<IItemSource> ItemSource)
		{
			this.ItemSource = ItemSource;
		}


		static PagingQueryBuilder<IItem> itemPagingBuilder = new PagingQueryBuilder<IItem>(
			"hot",
			i =>
			  i.Add("new", p => p.PublishedTime, true)
			  .Add("hot", p => p.Visited, true)
			  .Add("price", p => p.Price)
			  .Add("order", (q, a) => q)
		);
		public virtual PagingQueryBuilder<IItem> ItemPagingBuilder { get { return itemPagingBuilder; } }


		static PagingQueryBuilder<ICategoryCached> catPagingBuilder = new PagingQueryBuilder<ICategoryCached>(
			"order",
			i =>
			  i.Add("order", p => p.Order)
		);
		public virtual PagingQueryBuilder<ICategoryCached> CategoryPagingBuilder { get { return catPagingBuilder; } }

		public async Task<QueryResult<ICategoryCached>> ListCategories(long ParentCategoryCachedId, Paging paging)
		{
			var cids = await GetRelationIds(ParentCategoryCachedId, category_children, ItemSource().CategoryChildrenLoader);
			var cats = await GetCategories(cids);
			var re = cats.AsQueryable<ICategoryCached>().ToQueryResult(
				pq => pq,
				r => r,
				CategoryPagingBuilder,
				paging
				);
			return re;
		}
		public async Task<ICategoryCached[]> GetCategories(long[] CategoryIds)
		{
			return await GetObjects<ICategoryCached, ICategoryCached>(CategoryIds, categories, ItemSource().CategoryLoader);
		}
		public async Task<long[]> GeICategoryCachedIds(string Tag)
		{
			long[] ids;
			if (!taggedCategories.TryGetValue(Tag, out ids))
			{
				ids = await ItemSource().TaggedCategoryLoader.Load(Tag);
				taggedCategories.GetOrAdd(Tag, ids);
			}
			return ids;
		}
		public async Task<ICategoryCached[]> GetTaggedCategories(string Tag)
		{
			return await GetCategories(await GeICategoryCachedIds(Tag));
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
		protected virtual IItem CreateItemResult(IItemCached Item, IProductCached Product)
        {
            return new Item(Item, Product);
        }
		public async Task<IItem[]> GetItems(long[] ItemIds)
		{
			var itemSource = ItemSource();
			var its=await GetObjects<IItemCached>(ItemIds, items, itemSource.ItemLoader);
			var ps = (await GetObjects<IProductCached>(its.Select(i => i.ProductId).ToArray(), products, itemSource.ProductLoader)).ToDictionary(p => p.Id);
			return its.Select(it =>
			{
				IProductCached p;
				if (ps.TryGetValue(it.ProductId, out p))
					return CreateItemResult(it, p);
				else
					return default(IItem);
			}).Where(i => i != null).ToArray();
		}
		public async Task<IItem[]> GetProducts(long[] ProductIds)
		{
			var ps = await GetObjects<IProductCached>(ProductIds, products, ItemSource().ProductLoader);
			return ps.Select(p => CreateItemResult(null, p)).Where(p=>p!=null).ToArray();
		}

		async Task ItemCollect(long cid,HashSet<long> aids)
		{
			var itemSource = ItemSource();
			var cids = await GetRelationIds(cid, category_items, itemSource.CategoryItemsLoader);
			foreach (var id in cids)
				aids.Add(id);
			var ccats = await GetRelationIds(cid, category_children, itemSource.CategoryChildrenLoader);
			foreach (var cat in ccats)
				await ItemCollect(cat, aids);
		}
      

        public async Task<QueryResult<IItem>> ListTaggedItems(string Tag, bool WithChildCategoryItems, string filter, Paging paging)
        {
            var cids = await GeICategoryCachedIds(Tag);
            if (cids.Length == 0)
                return QueryResult<IItem>.Empty;
            if (cids.Length == 1)
                return await ListCategoryItems(cids[0], WithChildCategoryItems, filter, paging);

            var aids = new HashSet<long>();
            foreach(var cid in cids)
                await ItemCollect(cid, aids);
            return await NewItemResult(aids.ToArray(), filter, paging);
        }
        public async Task<QueryResult<IItem>> QueryAsync(ItemQueryArgument Args)
        {
            if (!Args.ProductId.HasValue)
            {
                if (Args.CategoryId != null)
                    return await ListCategoryItems(Args.CategoryId.Value, true, Args.Title,Args.Paging);
                if (Args.CategoryTag != null)
                    return await ListTaggedItems(Args.CategoryTag, true, Args.Title, Args.Paging);
                return await ListCategoryItems(MainCategoryId, true, Args.Title, Args.Paging);
            }
            var pi = await GetProducts(new[] { Args.ProductId.Value });
            if (pi.Length == 0)
                return QueryResult<IItem>.Empty;
            var ii = await GetItems(new[] { pi[0].MainItemId });
			if (Args.SellerId.HasValue)
				ii = ii.Where(i => i.SellerId == Args.SellerId.Value).ToArray();

            if(ii==null || ii.Length==0)
                 return QueryResult<IItem>.Empty;
            return new QueryResult<IItem>
            {
                Items = ii,
                Total=1
            };
		}
		public async Task<QueryResult<long>> QueryIdentsAsync(ItemQueryArgument Args)
		{
			var re = await QueryAsync(Args);
			return new QueryResult<long>
			{
				Items = re.Items.Select(i => i.ItemId).ToArray(),
				Summary = re.Summary,
				Total = re.Total
			};
		}

		async Task<QueryResult<IItem>> NewItemResult(long[] cids,string filter, Paging paging)
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
				ids = await GetRelationIds(CategoryId, category_items, ItemSource().CategoryItemsLoader);
			}
			return ids;
		}
		public async Task<QueryResult<IItem>> ListCategoryItems(long CategoryId, bool WithChildCategoryItems, string filter, Paging paging)
		{
			var ids = await ListItemIdents(CategoryId, WithChildCategoryItems, filter, paging);
			return await NewItemResult(ids,filter,paging);
		}


        public void NotifyItemChanged(long ItemId)
		{
			IItemCached item;
			items.TryRemove(ItemId, out item);
		}
		
		public void NotifyProductChanged(long ItemId)
		{
			IProductCached item;
			products.TryRemove(ItemId, out item);
		}

		public void NotifyCategoryChanged(long CategoryId)
		{
			ICategoryCached cat;
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
        
		public async Task<IItem> GetItemDetail(long Id)
		{
			var itemSource = ItemSource();
			var its = await GetObjects<IItemCached>(new long[]{ Id}, items, itemSource.ItemLoader);
			if (its.Length == 0) return default(IItem);

			var ps = await GetObjects<IProductCached>(new long[] { its[0].ProductId }, products, itemSource.ProductLoader);
			if (ps.Length == 0) return default(IItem);

			return CreateItemResult(its[0], ps[0]);
		}
		public async Task<IItem> GetProductDetail(long Id)
		{
			var ps = await GetObjects<IProductCached>(new long[] { Id }, products, ItemSource().ProductLoader);
			if (ps.Length == 0) return default(IItem);

			return CreateItemResult(null, ps[0]);

		}

        public void NotifyCategoryTag(string CategoryTag)
        {
			long[] ids;
            taggedCategories.TryRemove(CategoryTag, out ids);
        }

        async Task<ITradableItem[]> ITradableItemResolver.Resolve(string[] Idents)
        {
            var ids = Idents.Select(i => i.RightAfter('-').ToInt64()).ToArray();
            var results = new ITradableItem[ids.Length];
            var re=await GetItems(ids);
            for (var i = 0; i < re.Length; i++)
                results[i] = (ITradableItem)re[i];
            return results;
        }
    }
	
}
