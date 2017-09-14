using SF.Entities;
using System.Threading.Tasks;

namespace SF.Biz.Products
{
	public interface IProductManager<TInternal, TEditable> :
		IEntityManager<long, TEditable>,
		IEntitySource<long, TInternal , ProductInternalQueryArgument>
		where TInternal : ProductInternal
		where TEditable : ProductEditable
	{
		Task SetObjectState(long Id, EntityLogicState State);

        Task<ProductSpec> GetSpec(long Id);
        Task<ProductSpec[]> ListSpec(long Id);

    }
}
