using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Data.Storage;

namespace SF.Data.Entity
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
		where TCategoryPublic : class, IObjectWithId<TCategoryKey>
		where TCategoryKey : IEquatable<TCategoryKey>
		where TCategoryModel : class, IObjectWithId<TCategoryKey>, new()
		where TQueryArgument : class, IQueryArgument<TCategoryKey>, new()
		where TCategoryEditable : class, IObjectWithId<TCategoryKey>
		where TItemKey : IEquatable<TItemKey>
		where TItemModel : class, IObjectWithId<TItemKey>, new()
	{
		public CategoryEntityManager(
			IDataSet<TCategoryModel> DataSet
			) : base(DataSet)
		{
		}
	}


}