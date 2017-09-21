﻿using System;
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
		IEntityReferenceResolver _DataEntityResolver;
		ILogger<T> _Logger;
		IIdentGenerator _IdentGenerator;
		IEventEmitter _EventEmitter;
		IServiceInstanceDescriptor _ServiceInstanceDescroptor;
		DateTime _Now;

		I Resolve<I>(ref I value)
			where I:class
		{
			if (value == null)
				value = ServiceProvider.Resolve<I>();
			return value;
		}
		public DateTime Now
		{
			get
			{
				if (_Now == default(DateTime))
					_Now = TimeService.Now;
				return _Now;
			}
		}
		public IDataSet<T> DataSet => Resolve(ref _DataSet);
		public IIdentGenerator IdentGenerator => Resolve(ref _IdentGenerator);
		public IEntityReferenceResolver DataEntityResolver => Resolve(ref _DataEntityResolver);
		public ITimeService TimeService => Resolve(ref _TimeService);
		public ILogger Logger => Resolve(ref _Logger);
		public IEventEmitter EventEmitter => Resolve(ref _EventEmitter);
		public IServiceInstanceDescriptor ServiceInstanceDescroptor=> Resolve(ref _ServiceInstanceDescroptor);
		IDataSet IDataSetEntityManager.DataSet => DataSet;

		

		public void InitCreateContext<TKey, TEditable>(IEntityModifyContext<TKey, TEditable, T> Context,TEditable Editable, object ExtraArguments)
			where TKey : IEquatable<TKey>
			where TEditable : IEntityWithId<TKey>
		{
			Context.Action = ModifyAction.Create;
			Context.Editable = Editable;
			Context.ExtraArgument = ExtraArguments;
		}

		public void InitRemoveContext<TKey>(IEntityModifyContext<TKey, T> Context, TKey Id, T Model, object ExtraArguments) where TKey : IEquatable<TKey>
		{
			Context.Action = ModifyAction.Delete;
			Context.Id = Id;
			Context.Model = Model;
			Context.ExtraArgument = ExtraArguments;
		}

		public void InitUpdateContext<TKey, TEditable>(IEntityModifyContext<TKey, TEditable, T> Context,TKey Id, TEditable Editable, T Model, object ExtraArguments)
			where TKey : IEquatable<TKey>
			where TEditable : IEntityWithId<TKey>
		{
			Context.Action = ModifyAction.Update;
			Context.Id = Id;
			Context.Editable = Editable;
			Context.Model = Model;
			Context.ExtraArgument = ExtraArguments;
		}
	}
}
