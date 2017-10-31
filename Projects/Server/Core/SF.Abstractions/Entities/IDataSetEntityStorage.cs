#region Apache License Version 2.0
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

using System.Threading.Tasks;
using System;
using SF.Core.ServiceManagement;
using System.Linq;
using SF.Data;
using SF.Core.Logging;
using SF.Core.Times;
using SF.Core.Events;
using SF.Clients;
using SF.Entities.AutoEntityProvider;

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
		IServiceProvider ServiceProvider { get; }
		IServiceInstanceDescriptor ServiceInstanceDescroptor { get; }
		IClientService ClientService { get; }
		IAccessToken AccessToken { get; }
		IEntityMetadata EntityMetadata { get; }
		IScoped<IDataContext> ScopedDataContext { get; }
		IQueryResultBuildHelperCache QueryResultBuildHelperCache { get; }
		IPagingQueryBuilderCache PagingQueryBuilderCache { get; }
		IQueryFilterCache QueryFilterCache { get; }
		IEntityModifierCache EntityModifierCache { get; }
		IDataContext DataContext { get; }
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
