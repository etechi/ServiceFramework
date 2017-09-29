using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Data;

namespace SF.Entities
{

	public abstract class CategoryEntityManager<
		TCategoryPublic,
		TCategoryTemp,
		TQueryArgument,
		TCategoryEditable,
		TCategoryModel,

		TItemKey,
		TItemModel
		> :
		ModidifiableEntityManager< TCategoryPublic, TCategoryTemp, TQueryArgument, TCategoryEditable, TCategoryModel>
		where TCategoryPublic : class
		where TCategoryModel : class, new()
		where TQueryArgument : class,new()
		where TCategoryEditable : class
		where TItemKey : IEquatable<TItemKey>
		where TItemModel : class, IEntityWithId<TItemKey>, new()
	{
		public CategoryEntityManager(
			IDataSetEntityManager<TCategoryEditable, TCategoryModel> EntityManager
			) : base(EntityManager)
		{
		}
	}


}