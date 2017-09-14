using System.Threading.Tasks;

namespace SF.Biz.Products
{
	public interface IBatchLoader<T>
	{
		Task<T[]> Load(long[] Ids);
	}
	public interface IRelationLoader<T>
	{
		Task<long[]> Load(T Id);
	}
    public interface IRelationLoader : IRelationLoader<long>
    {
        
    }

    public interface IItemNotifier
	{
		void NotifyProductChanged(long ProductId);
		void NotifyItemChanged(long ItemId);
		void NotifyProductContentChanged(long ProductId);
		void NotifyCategoryChanged(long CategoryId);
		void NotifyCategoryChildrenChanged(long CategoryId);
		void NotifyCategoryItemsChanged(long CategoryId);
        void NotifyCategoryTag(string CategoryTag);
	}
	public interface IItemSource
	{
        int MainCategoryId { get;}
		IBatchLoader<IItemCached> ItemLoader { get; }
		IBatchLoader<ICategoryCached> CategoryLoader { get; }
		IBatchLoader<IProductCached> ProductLoader { get; }
		IBatchLoader<IProductContentCached> ProductContentLoader { get; }
		IRelationLoader CategoryChildrenLoader { get; }
		IRelationLoader CategoryItemsLoader { get; }
        IRelationLoader<string> TaggedCategoryLoader { get; }
    }
	
}
