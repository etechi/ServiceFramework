using System.Threading.Tasks;
using System;
using SF.Core.ServiceManagement;
using System.Linq;
using SF.Data;
using SF.Core.Logging;
using SF.Core.Times;

namespace SF.Entities
{
	public enum ModifyAction
	{
		Create,
		Update,
		Delete
	}
	public interface IEntityModifyContext
	{
		ModifyAction Action { get; }
		object OwnerId { get; set; }
		object UserData { get; set; }
		object ExtraArgument { get; set; }
	}
	public interface IEntityModifyContext<TKey, TModel>:
		IEntityModifyContext
	{
		TKey Id { get; set; }
		TModel Model { get; set; }
	}
	public interface IEntityModifyContext<TKey, TEditable, TModel> :
		IEntityModifyContext<TKey, TModel>
		where TKey:IEquatable<TKey>
		where TModel:class
	{
		TEditable Editable { get; set; }
	}

	public interface IEntityManager
	{
		IIdentGenerator IdentGenerator { get; }
		IDataEntityResolver DataEntityResolver { get; }
		ITimeService TimeService { get; }
		ILogger Logger { get; }
	}
	public interface IEntityManager<TModel>: IEntityManager
		where TModel:class
	{
		IEntityModifyContext<TKey, TEditable, TModel> NewCreateContext<TKey, TEditable>(
			TEditable Editable,
			object ExtraArguments
			)
		where TKey : IEquatable<TKey>
		where TEditable : IEntityWithId<TKey>;

		IEntityModifyContext<TKey, TEditable, TModel> NewUpdateContext<TKey, TEditable>(
			TEditable Editable,
			TModel Model,
			object ExtraArguments
			)
		where TKey : IEquatable<TKey>
		where TEditable : IEntityWithId<TKey>;

		IEntityModifyContext<TKey, TModel> NewRemoveContext<TKey>(
			TModel Model,
			object ExtraArguments
			)
		where TKey : IEquatable<TKey>;
	}
	public interface IDataSetEntityManager : IEntityManager
	{
		IDataSet DataSet { get; }
	}

	public interface IDataSetEntityManager<TModel> : IDataSetEntityManager,IEntityManager<TModel>
		where TModel:class
	{
		new IDataSet<TModel> DataSet { get; }

	}
}
