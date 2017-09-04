using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Core.Logging;
using SF.Core.Times;
using SF.Data;
using SF.Core.ServiceManagement;
using SF.Core.Events;

namespace SF.Entities
{
	class DataSetEntityManager<T> : IDataSetEntityManager<T>
		where T : class
	{
		IServiceProvider ServiceProvider { get; }
		IDataSet<T> _DataSet;
		ITimeService _TimeService;
		IDataEntityResolver _DataEntityResolver;
		ILogger<T> _Logger;
		IIdentGenerator _IdentGenerator;
		IEventEmitter _EventEmitter;

		I Resolve<I>(ref I value)
			where I:class
		{
			if (value == null)
				value = ServiceProvider.Resolve<I>();
			return value;
		}

		public IDataSet<T> DataSet => Resolve(ref _DataSet);
		public IIdentGenerator IdentGenerator => Resolve(ref _IdentGenerator);
		public IDataEntityResolver DataEntityResolver => Resolve(ref _DataEntityResolver);
		public ITimeService TimeService => Resolve(ref _TimeService);
		public ILogger Logger => Resolve(ref _Logger);
		public IEventEmitter EventEmitter => Resolve(ref _EventEmitter);

		IDataSet IDataSetEntityManager.DataSet => DataSet;

		class ModifyContext<TKey, TModel> : IEntityModifyContext<TKey, TModel>
			where TKey:IEquatable<TKey>
			where TModel:class
		{
			public ModifyContext(ModifyAction Action)
			{
				this.Action = Action;
			}
			public TKey Id { get; set ; }
			public TModel Model { get; set ; }

			public ModifyAction Action { get; }
			public object OwnerId { get ; set ; }
			public object UserData { get; set ; }
			public object ExtraArgument { get ; set ; }
		}

		class ModifyContext<TKey, TEditable, TModel> :
			ModifyContext<TKey, TModel>,
			IEntityModifyContext<TKey, TEditable, TModel>
			where TKey : IEquatable<TKey>
			where TModel:class
		{
			public ModifyContext(ModifyAction Action) : base(Action)
			{
			}

			public TEditable Editable { get; set; }
		}

		public IEntityModifyContext<TKey, TEditable, T> NewCreateContext<TKey, TEditable>(TEditable Editable, object ExtraArguments)
			where TKey : IEquatable<TKey>
			where TEditable : IEntityWithId<TKey>
		{
			return new ModifyContext<TKey, TEditable, T>(ModifyAction.Create)
			{
				Editable = Editable,
				ExtraArgument = ExtraArguments
			};
		}

		public IEntityModifyContext<TKey, T> NewRemoveContext<TKey>(TKey Id, T Model, object ExtraArguments) where TKey : IEquatable<TKey>
		{
			return new ModifyContext<TKey, T>(ModifyAction.Delete)
			{
				Id= Id,
				Model = Model,
				ExtraArgument = ExtraArguments
			};
		}

		public IEntityModifyContext<TKey, TEditable, T> NewUpdateContext<TKey, TEditable>(TKey Id, TEditable Editable, T Model, object ExtraArguments)
			where TKey : IEquatable<TKey>
			where TEditable : IEntityWithId<TKey>
		{
			return new ModifyContext<TKey, TEditable, T>(ModifyAction.Update)
			{
				Id= Id,
				Editable = Editable,
				Model = Model,
				ExtraArgument = ExtraArguments
			};
		}
	}
}
