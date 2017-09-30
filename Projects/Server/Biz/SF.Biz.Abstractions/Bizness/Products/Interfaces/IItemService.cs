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


	public interface IItemService<TItem, TCategory>:
		IEntityQueryable<TItem,ItemQueryArgument>
		where TItem: IItem
		where TCategory: ICategoryCached
	{
		Task<TItem> GetItemDetail(long ItemId);
		Task<TItem> GetProductDetail(long ProductId);
		Task<TItem[]> GetProducts(long[] ProductIds);
		Task<TItem[]> GetItems(long[] ItemIds);
        Task<QueryResult<TItem>> ListItems(string Tag, bool WithChildCategoryItems, string filter, Paging paging);
        Task<QueryResult<TItem>> ListItems(long CategoryId, bool WithChildCategoryItems, string filter, Paging paging);
        Task<TCategory[]> GetCategories(string Tag);
		Task<TCategory[]> GetCategories(long[] CategoryIds);
		Task<QueryResult<TCategory>> ListCategories(long ParentCategoryId, Paging paging);
	}
	
}
