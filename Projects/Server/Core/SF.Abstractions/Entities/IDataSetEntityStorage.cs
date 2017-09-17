using System.Threading.Tasks;
using System;
using SF.Core.ServiceManagement;
using System.Linq;
using SF.Data;
using SF.Core.Logging;
using SF.Core.Times;
using SF.Core.Events;

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
		ModifyAction Action { get; set; }
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
		IEventEmitter EventEmitter { get; }
		IServiceInstanceDescriptor ServiceInstanceDescroptor { get; }
		DateTime Now { get; }
	}
	public interface IModifiableEntityManager<TModel>: IEntityManager
		where TModel:class
	{
		void InitCreateContext<TKey, TEditable>(
			IEntityModifyContext<TKey, TEditable, TModel> Context,
			TEditable Editable,
			object ExtraArguments
			)
		where TKey : IEquatable<TKey>
		where TEditable : IEntityWithId<TKey>;

		void InitUpdateContext<TKey, TEditable>(
			IEntityModifyContext<TKey, TEditable, TModel> Context,
			TKey Id,
			TEditable Editable,
			TModel Model,
			object ExtraArguments
			)
		where TKey : IEquatable<TKey>
		where TEditable : IEntityWithId<TKey>;

		void InitRemoveContext<TKey>(
			IEntityModifyContext<TKey, TModel> Context,
			TKey Id,
			TModel Model,
			object ExtraArguments
			)
		where TKey : IEquatable<TKey>;
	}
	public interface IDataSetEntityManager : IEntityManager
	{
		IDataSet DataSet { get; }
	}

	public interface IDataSetEntityManager<TModel> : IDataSetEntityManager,IModifiableEntityManager<TModel>
		where TModel:class
	{
		new IDataSet<TModel> DataSet { get; }

	}
}
