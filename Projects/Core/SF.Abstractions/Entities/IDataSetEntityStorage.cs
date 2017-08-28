using System.Threading.Tasks;
using System;
using SF.Core.ServiceManagement;
using System.Linq;
using SF.Data;

namespace SF.Entities
{
	public enum ModifyAction
	{
		Create,
		Update,
		Delete
	}
	public interface IEntityModifyContext<TKey, TEditable, TModel>
		where TKey:IEquatable<TKey>
		where TModel:class
	{
		ModifyAction Action { get; }
		TKey Id { get; set; }
		TEditable Editable { get; set; }
		TModel Model { get; set; }
		object OwnerId { get; set; }
		object UserData { get; set; }
		object ExtraArgument { get; set; }
		void AddPostAction(Action action,bool CallOnSaved = true);
		void AddPostAction(Func<Task> action,bool CallOnSaved = true);
	}
	public interface IEntityStorage<TModel>
		where TModel:class
	{
	}
	public interface IDataSetEntityStorage<TModel> : IEntityStorage<TModel>
		where TModel:class
	{
		IDataSet<TModel> DataSet { get; }
		IEntityModifyContext<TKey, TEditable, TModel> NewContext<TKey, TEditable>(
			ModifyAction Action,
			TEditable Editable,
			object ExtraArguments
			)
		where TKey : IEquatable<TKey>
		where TEditable : IEntityWithId<TKey>;
	}
}
