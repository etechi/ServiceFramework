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
	public interface IEntityModifyContext<TModel>:
		IEntityModifyContext
	{
		TModel Model { get; set; }
	}
	public interface IEntityModifyContext<TEditable, TModel> :
		IEntityModifyContext<TModel>
		where TModel:class
	{
		TEditable Editable { get; set; }
	}

	public interface IEntityManager
	{
		IIdentGenerator IdentGenerator { get; }
		IEntityReferenceResolver DataEntityResolver { get; }
		ITimeService TimeService { get; }
		ILogger Logger { get; }
		IEventEmitter EventEmitter { get; }
		IServiceInstanceDescriptor ServiceInstanceDescroptor { get; }
		DateTime Now { get; }
	}
	public interface IModifiableEntityManager<TEditable,TModel>: IEntityManager
		where TModel:class
	{
		void InitCreateContext(
			IEntityModifyContext<TEditable, TModel> Context,
			TEditable Editable,
			object ExtraArguments
			);

		void InitUpdateContext(
			IEntityModifyContext<TEditable, TModel> Context,
			TEditable Editable,
			TModel Model,
			object ExtraArguments
			);

		void InitRemoveContext(
			IEntityModifyContext<TEditable, TModel> Context,
			TEditable Editable,
			TModel Model,
			object ExtraArguments
			);
	}
	public interface IDataSetEntityManager : IEntityManager
	{
		IDataSet DataSet { get; }
	}
	public interface IReadOnlyDataSetEntityManager<TModel> : IDataSetEntityManager
		where TModel : class
	{
		new IIdentGenerator<TModel> IdentGenerator { get; }
		new IDataSet<TModel> DataSet { get; }
	}
	public interface IDataSetEntityManager<TEditable,TModel> : 
		IReadOnlyDataSetEntityManager<TModel>, 
		IModifiableEntityManager<TEditable, TModel>
		where TModel:class
	{
	}
}
