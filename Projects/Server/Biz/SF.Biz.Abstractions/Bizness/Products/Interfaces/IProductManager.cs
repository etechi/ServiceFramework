using SF.Entities;
using System.Threading.Tasks;

namespace SF.Biz.Products
{
	public interface IProductManager: IProductManager<ProductInternal,ProductEditable>
	{
	}
	public interface IProductManager<TInternal, TEditable> :
		IEntityManager<ObjectKey<long>, TEditable>,
		IEntitySource<ObjectKey<long>, TInternal , ProductInternalQueryArgument>
		where TInternal : ProductInternal
		where TEditable : ProductEditable
	{
		Task SetLogicState(long Id, EntityLogicState State);

        Task<ProductSpec> GetSpec(long Id);
        Task<ProductSpec[]> ListSpec(long Id);

    }
}
