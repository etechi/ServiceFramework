using SF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Biz.Products
{
	public interface IItemManager : IItemManager<ItemInternal, ItemEditable>
	{ }
	public interface IItemManager<TInternal, TEditable> :
		IEntityManager<ObjectKey<long>, TEditable>,
		IEntitySource<ObjectKey<long>, TInternal,ItemQueryArgument>
		where TInternal : ItemInternal
		where TEditable : ItemEditable
	{
	}
}
