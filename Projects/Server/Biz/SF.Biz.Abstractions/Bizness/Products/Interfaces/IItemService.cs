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

using SF.Entities;
using SF.Metadata;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SF.Biz.Products
{
	public class ItemQueryArgument : IQueryArgument<ObjectKey<long>>
    {
		public ObjectKey<long> Id { get; set; }

        [Display(Name = "产品")]
		[EntityIdent(typeof(ProductInternal))]
		public long? ProductId { get; set; }

		[Display(Name = "卖家")]
		[EntityIdent(typeof(SF.Users.Members.Models.MemberInternal))]
		public long? SellerId { get; set; }

		[Display(Name = "产品标题")]
        public string Title { get; set; }

        [EntityIdent(typeof(CategoryInternal))]
        [Display(Name = "产品目录")]
        public long? CategoryId { get; set; }

        [Display(Name = "目录标签")]
        public string CategoryTag { get; set; }

		[Display(Name = "产品类型")]
		public long? TypeId { get; set; }
    }


	public interface IItemService :
		IItemService<IItem, ICategoryCached>
	{ }

	[NetworkService]
	public interface IItemService<TItem, TCategory>:
		IEntityQueryable<TItem,ItemQueryArgument>
		where TItem: IItem
		where TCategory: ICategoryCached
	{
		Task<TItem> GetItemDetail(long ItemId);
		Task<TItem> GetProductDetail(long ProductId);
		Task<TItem[]> GetProducts(long[] ProductIds);
		Task<TItem[]> GetItems(long[] ItemIds);
        Task<QueryResult<TItem>> ListTaggedItems(string Tag, bool WithChildCategoryItems, string Filter, Paging Paging);
        Task<QueryResult<TItem>> ListCategoryItems(long CategoryId, bool WithChildCategoryItems, string Filter, Paging Paging);
        Task<TCategory[]> GetTaggedCategories(string Tag);
		Task<TCategory[]> GetCategories(long[] CategoryIds);
		Task<QueryResult<TCategory>> ListCategories(long ParentCategoryId, Paging Paging);
	}
	
}
