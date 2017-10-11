using SF.Entities;
using System.Threading.Tasks;

namespace SF.Biz.Products
{
	public static class ItemServiceExtension
	{
		public static async Task<TCategory> GetCategory<TItem, TCategory>(
			this IItemService<TItem, TCategory> service,
			long id
			)
			where TItem : IItem
			where TCategory : ICategoryCached
		{
			var re = await service.GetCategories(new[] { id });
			if (re == null || re.Length == 0) return default(TCategory);
			return re[0];
		}
		public static async Task<TItem> GetItem<TItem, TCategory>(
			this IItemService<TItem, TCategory> service,
			long id
			)
			where TItem : IItem
			where TCategory : ICategoryCached
		{
			var re = await service.GetItems(new[] { id });
			if (re == null || re.Length == 0) return default(TItem);
			return re[0];
		}

		public static Task<QueryResult<TCategory>> ListCategories<TItem, TCategory>(
			this IItemService<TItem, TCategory> service,
			long ParentCategoryId,
			int count=0
			)
			where TItem : IItem
			where TCategory : ICategoryCached
		{
			return service.ListCategories(
				ParentCategoryId, 
				count == 0 ? null : new Paging { Count = count }
				);
		}

		public static Task<QueryResult<TItem>> ListItems<TItem, TCategory>(
			this IItemService<TItem, TCategory> service,
			long CategoryId, 
			bool WithChildCategoryItems, 
			Paging paging
			)
			where TItem : IItem
			where TCategory : ICategoryCached
		{
			return service.ListItems(
				CategoryId,
				WithChildCategoryItems,
				paging
				);
		}
		public static Task<QueryResult<TItem>> ListItems<TItem, TCategory>(
			this IItemService<TItem, TCategory> service,
			long CategoryId,
			bool WithChildCategoryItems,
			int count=0
			)
			where TItem : IItem
			where TCategory : ICategoryCached
		{
			return service.ListItems(
				CategoryId,
				WithChildCategoryItems,
				count == 0 ? null : new Paging { Count = count }
				);
		}
		public static Task<QueryResult<TItem>> Search<TItem, TCategory>(
			this IItemService<TItem, TCategory> service,
			long CategoryId,
			bool WithChildCategoryItems,
			string keyword,
			int count = 0
			)
			where TItem : IItem
			where TCategory : ICategoryCached
		{
			return service.ListCategoryItems(
				CategoryId,
				WithChildCategoryItems,
				keyword,
				count == 0 ? null : new Paging { Count = count }
				);
		}
	}
}
