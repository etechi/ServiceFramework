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
	class EntityServiceContext : IEntityServiceContext
	{
		public IServiceProvider ServiceProvider { get; }
		IScoped<IDataContext> _ScopedDataContext;
		IDataContext _DataContext;
		ITimeService _TimeService;
		IEntityReferenceResolver _DataEntityResolver;
		IIdentGenerator _IdentGenerator;
		IEventEmitter _EventEmitter;
		IClientService _ClientService;
		IEntityMetadata _EntityMetadata;
		IAccessToken _AccessToken;
		IQueryResultBuildHelperCache _QueryResultBuildHelperCache;
		IQueryFilterCache _QueryFilterCache;
		IPagingQueryBuilderCache _PagingQueryBuilderCache;
		IEntityModifierCache _EntityModifierCache;
		IEntityMetadataCollection _EntityMetadataCollection;
		public IServiceInstanceDescriptor ServiceInstanceDescroptor { get; }

		DateTime _Now;

		public EntityServiceContext(IServiceProvider ServiceProvider)
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
		public IDataContext DataContext => Resolve(ref _DataContext);
		public IIdentGenerator IdentGenerator => Resolve(ref _IdentGenerator);
		IIdentGenerator IEntityServiceContext.IdentGenerator => Resolve(ref _IdentGenerator);
		public IEntityReferenceResolver DataEntityResolver => Resolve(ref _DataEntityResolver);
		public ITimeService TimeService => Resolve(ref _TimeService);
		public ILogger GetLogger(Type Type) => (ILogger )ServiceProvider.GetService(typeof(ILogger<>).MakeGenericType(Type));
		public IEventEmitter EventEmitter => Resolve(ref _EventEmitter);
		public IClientService ClientService => Resolve(ref _ClientService);
		public IAccessToken AccessToken => Resolve(ref _AccessToken);
		public IScoped<IDataContext> ScopedDataContext => Resolve(ref _ScopedDataContext);
		public IQueryFilterCache QueryFilterCache=> Resolve(ref _QueryFilterCache);
		public IQueryResultBuildHelperCache QueryResultBuildHelperCache => Resolve(ref _QueryResultBuildHelperCache);
		public IEntityModifierCache EntityModifierCache => Resolve(ref _EntityModifierCache);
		public IPagingQueryBuilderCache PagingQueryBuilderCache => Resolve(ref _PagingQueryBuilderCache);
		public IEntityMetadataCollection EntityMetadataCollection => Resolve(ref _EntityMetadataCollection);

		public IEntityMetadata EntityMetadata
		{
			get
			{
				if (_EntityMetadata == null)
				{
					_EntityMetadata = EntityMetadataCollection.FindByManagerType(GetType());
				}
				return _EntityMetadata;
			}
		}

	}
}
