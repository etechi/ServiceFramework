using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Data;

namespace SF.Entities
{

	public abstract class CategoryEntityManager<
		TCategoryKey,
		TCategoryPublic,
		TCategoryTemp,
		TQueryArgument,
		TCategoryEditable,
		TCategoryModel,

		TItemKey,
		TItemModel
		> :
		EntityManager<TCategoryKey, TCategoryPublic, TCategoryTemp, TQueryArgument, TCategoryEditable, TCategoryModel>
		where TCategoryPublic : class, IEntityWithId<TCategoryKey>
		where TCategoryKey : IEquatable<TCategoryKey>
		where TCategoryModel : class, IEntityWithId<TCategoryKey>, new()
		where TQueryArgument : class, IQueryArgument<TCategoryKey>, new()
		where TCategoryEditable : class, IEntityWithId<TCategoryKey>
		where TItemKey : IEquatable<TItemKey>
		where TItemModel : class, IEntityWithId<TItemKey>, new()
	{
		public CategoryEntityManager(
			IDataSet<TCategoryModel> DataSet
			) : base(DataSet)
		{
		}
	}


}