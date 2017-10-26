﻿#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

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
using SF.Clients;
using SF.Auth;
using SF.Auth.Permissions;
using SF.Entities.AutoEntityProvider;

namespace SF.Entities
{
	class DataSetEntityManager<TEditable, TDataModel> : IDataSetEntityManager<TEditable, TDataModel>
		where TDataModel : class, new()
		where TEditable : class, new()
	{
		public IServiceProvider ServiceProvider { get; }
		IScoped<IDataContext> _ScopedDataContext;

		IDataSet<TDataModel> _DataSet;
		ITimeService _TimeService;
		IEntityReferenceResolver _DataEntityResolver;
		ILogger<TDataModel> _Logger;
		IIdentGenerator<TDataModel> _IdentGenerator;
		IEventEmitter _EventEmitter;
		IClientService _ClientService;
		IEntityMetadata _EntityMetadata;
		IAccessToken _AccessToken;
		IQueryResultBuildHelperCache _QueryResultBuildHelperCache;
		IQueryFilterCache _QueryFilterCache;
		IPagingQueryBuilderCache _PagingQueryBuilderCache;
		IEntityModifierCache _EntityModifierCache;
		public IServiceInstanceDescriptor ServiceInstanceDescroptor { get; }

		DateTime _Now;

		public DataSetEntityManager(IServiceProvider ServiceProvider)
		{
			this.ServiceProvider = ServiceProvider;
			var resolver = ServiceProvider.Resolver();
			if (resolver.CurrentServiceId.HasValue)
				this.ServiceInstanceDescroptor = resolver.ResolveDescriptorByIdent(resolver.CurrentServiceId.Value);
		}

		I Resolve<I>(ref I value)
			where I : class
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
		public IDataContext DataContext => DataSet.Context;
		public IDataSet<TDataModel> DataSet => Resolve(ref _DataSet);
		public IIdentGenerator<TDataModel> IdentGenerator => Resolve(ref _IdentGenerator);
		IIdentGenerator IEntityManager.IdentGenerator => Resolve(ref _IdentGenerator);
		public IEntityReferenceResolver DataEntityResolver => Resolve(ref _DataEntityResolver);
		public ITimeService TimeService => Resolve(ref _TimeService);
		public ILogger Logger => Resolve(ref _Logger);
		public IEventEmitter EventEmitter => Resolve(ref _EventEmitter);
		public IClientService ClientService => Resolve(ref _ClientService);
		public IAccessToken AccessToken => Resolve(ref _AccessToken);
		public IScoped<IDataContext> ScopedDataContext => Resolve(ref _ScopedDataContext);
		public IQueryFilterCache QueryFilterCache=> Resolve(ref _QueryFilterCache);
		public IQueryResultBuildHelperCache QueryResultBuildHelperCache => Resolve(ref _QueryResultBuildHelperCache);
		public IEntityModifierCache EntityModifierCache => Resolve(ref _EntityModifierCache);
		public IPagingQueryBuilderCache PagingQueryBuilderCache => Resolve(ref _PagingQueryBuilderCache);

		public IEntityMetadata EntityMetadata
		{
			get
			{
				if (_EntityMetadata == null)
				{
					var metas = ServiceProvider.Resolve<IEntityMetadataCollection>();
					_EntityMetadata = metas.FindByManagerType(GetType());
				}
				return _EntityMetadata;
			}
		}
		IDataSet IDataSetEntityManager.DataSet => DataSet;

		
		public void InitCreateContext(IEntityModifyContext<TEditable, TDataModel> Context,TEditable Editable, object ExtraArguments)
		{

			Context.Model = new TDataModel();
			Context.Action = ModifyAction.Create;
			Context.Editable = Editable;
			Context.ExtraArgument = ExtraArguments;
		}

		public void InitRemoveContext(IEntityModifyContext<TEditable, TDataModel> Context, TEditable Editable, TDataModel Model, object ExtraArguments)
		{

			Context.Action = ModifyAction.Delete;
			Context.Editable=Editable;
			Context.Model = Model;
			Context.ExtraArgument = ExtraArguments;
		}

		public void InitUpdateContext(IEntityModifyContext<TEditable, TDataModel> Context,TEditable Editable, TDataModel Model, object ExtraArguments)
		{

			Context.Action = ModifyAction.Update;
			Context.Editable = Editable;
			Context.Model = Model;
			Context.ExtraArgument = ExtraArguments;
		}
	}
}
