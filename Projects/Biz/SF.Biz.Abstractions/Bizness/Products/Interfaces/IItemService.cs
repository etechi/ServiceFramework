using SF.Entities;
using SF.Metadata;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SF.Biz.Products
{
	public class ItemQueryArgument : IQueryArgument<long>
    {
		public Option<long> Id { get; set; }
        [Display(Name = "产品ID")]
        public long? ProductId { get; set; }

        [Display(Name = "产品标题")]
        public string Title { get; set; }

        [EntityIdent(typeof(ICategoryManager<>))]
        [Display(Name = "产品目录")]
        public long? CategoryId { get; set; }

        [Display(Name = "目录标签")]
        public string CategoryTag { get; set; }

    }
    public interface IItemService<TItem, TCategory>:
		IEntityQueryable<long,TItem,ItemQueryArgument>
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
