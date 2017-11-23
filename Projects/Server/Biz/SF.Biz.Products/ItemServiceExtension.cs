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

using SF.Sys.Entities;
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
