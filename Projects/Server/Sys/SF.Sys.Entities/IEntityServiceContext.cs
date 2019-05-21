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

using System;
using SF.Sys.Data;
using SF.Sys.Services;
using SF.Sys.Entities.AutoEntityProvider;
using SF.Sys.Events;
using SF.Sys.Clients;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace SF.Sys.Entities
{
	public enum ModifyAction
	{
		Create,
		Update,
		Delete
	}
	
	public interface IEntityChildMerger<TChildEditable, TChildModel> where TChildModel : class
	{
		IEntityServiceContext ServiceContext { get; }
		IEntityModifyContext ModifyContext { get; }
		IEnumerable<TChildModel> ChildModels { get; }
		IEnumerable<TChildEditable> ChildEditables { get; }
			
		Task<ICollection<TChildModel>> Merge(
			Func<IEntityModifyContext<TChildEditable, TChildModel>,ModifyAction,Task> Handler
			);
	}
	public interface IEntityChildMergeHandler<TChildEditable, TChildModel> where TChildModel : class
	{
		Task<ICollection<TChildModel>> Merge(
			IEntityChildMerger<TChildEditable, TChildModel> Merger
			);
	}
	public interface IEntityModifyHandlerProvider {
		IEntityChildMergeHandler<TEditable, TModel> FindMergeHandler<TEditable, TModel>() where TModel : class;
	}
	public interface IEntityModifyContext
	{
		IDataContext DataContext { get; set; }
		ModifyAction Action { get; set; }
		object OwnerId { get; set; }
		object UserData { get; set; }
		object ExtraArgument { get; set; }

		IEntityModifyHandlerProvider ModifyHandlerProvider { get; }
		IEntityModifyContext<TEditale, TModel> CreateChildModifyContext<TEditale, TModel>() where TModel:class;
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

	public interface IEntityServiceContext
	{
		IIdentGenerator IdentGenerator { get; }
		//IEntityReferenceResolver DataEntityResolver { get; }
		//ITimeService TimeService { get; }
		//ILogger GetLogger(Type Type);
		IEntityPropertyFiller EntityPropertyFiller { get; }
		IEventEmitService EventEmitService { get; }
		IServiceProvider ServiceProvider { get; }
		IServiceInstanceDescriptor ServiceInstanceDescroptor { get; }
		IClientService ClientService { get; }
		IEntityMetadata EntityMetadata { get; }
		IEntityMetadataCollection EntityMetadataCollection { get; }
		IQueryResultBuildHelperCache QueryResultBuildHelperCache { get; }
		IPagingQueryBuilderCache PagingQueryBuilderCache { get; }
		IQueryFilterCache QueryFilterCache { get; }
		IEntityModifierCache EntityModifierCache { get; }
        IAccessToken AccessToken { get; }
        //IDataContext DataContext { get; }
        IDataScope DataScope { get;  }
		IUserAgent UserAgent { get; }
		
		DateTime Now { get; }
	}

	
}
