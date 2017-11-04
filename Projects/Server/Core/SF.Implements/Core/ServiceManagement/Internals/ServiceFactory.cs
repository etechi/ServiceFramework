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

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace SF.Core.ServiceManagement.Internals
{
	class ServiceFactory : IServiceFactory
	{
		public long InstanceId { get; }
		public long? ParentInstanceId { get; }
		Func<long?> LazyDataScopeId { get; }
		public long? DataScopeId => 
			LazyDataScopeId();
		public bool IsManaged => InstanceId > 0;
		public IServiceDeclaration ServiceDeclaration { get; }
		public IServiceImplement ServiceImplement { get; }

		public IServiceCreateParameterTemplate CreateParameterTemplate { get; }
		public ServiceCreator Creator { get; }
		public ServiceConfigLoader ConfigLoader { get; }
		public ServiceFactory(
			long Id,
			long? ParentServiceId,
			Func<long?> LazyDataScopeId,
			IServiceDeclaration ServiceDeclaration,
			IServiceImplement ServiceImplement,
			IServiceCreateParameterTemplate CreateParameterTemplate,
			ServiceCreator Creator,
			ServiceConfigLoader ConfigLoader
			)
		{
			this.InstanceId = Id;
			this.ParentInstanceId = ParentServiceId;
			this.ServiceDeclaration = ServiceDeclaration;
			this.ServiceImplement = ServiceImplement;
			this.CreateParameterTemplate = CreateParameterTemplate;
			this.Creator = Creator;
			this.ConfigLoader = ConfigLoader;
			this.LazyDataScopeId = LazyDataScopeId;
		}

		public object Create(
			IServiceResolver ServiceResolver
			)
		{
			if (IsManaged)
				using (ServiceResolver.WithScopeService(InstanceId))
					return Creator(ServiceResolver, this, CreateParameterTemplate);
			return Creator(ServiceResolver, this, CreateParameterTemplate);
		}

		public static (IServiceDeclaration,IServiceImplement) ResolveMetadata(
			IServiceResolver ServiceResolver,
			long Id,
			string svcTypeName,
			string implTypeName,
			Type ServiceType
			)
		{
			var ServiceMetadata = ServiceResolver.Resolve<IServiceMetadata>();

			var declType = ServiceResolver.Resolve<IServiceDeclarationTypeResolver>()
				.Resolve(svcTypeName)
				.IsNotNull(
					() => $"找不到服务类型({svcTypeName}),服务ID:{Id}"
					)
				.Assert(
					type =>ServiceType==null || type == ServiceType,
					type => $"服务实例({Id})的服务类型({type})和指定服务类型({ServiceType})不一致"
					);

			var decl = ServiceMetadata.Services
				.Get(declType)
				.IsNotNull(
					() => $"找不到服务描述({declType}),服务:{Id}"
					);

			var implType = ServiceResolver.Resolve<IServiceImplementTypeResolver>()
				.Resolve(implTypeName)
				.IsNotNull(
					() => $"找不到服务配置({Id})指定的服务实现类型({implTypeName}),服务:{declType}"
					);

			var impl = decl.Implements
				.Last(i => i.ServiceImplementType == ServiceImplementType.Type && i.ImplementType == implType)
				.IsNotNull(
					() => $"找不到服务配置({Id})指定的服务实现类型({implType}),服务:{declType}"
					);
			return (decl, impl);
		}
		public static IServiceFactory Create(
			long Id,
			long? ParentId,
			Func<long?> LazyDataScopeId,
			IServiceDeclaration decl,
			IServiceImplement impl,
			Type ServiceType, 
			ServiceCreatorCache CreatorCache,
			IServiceMetadata ServiceMetadata,
			string Setting
			)
		{
			ServiceCreator Creator;
			ServiceConfigLoader ConfigLoader = (sp, si, ctr) => Array.Empty<KeyValuePair<string, object>>();
			IServiceCreateParameterTemplate CreateParameterTemplate = null;
			switch (impl.ServiceImplementType)
			{
				case ServiceImplementType.Creator:
					var func = impl.ImplementCreator;
					Creator = (sp, si, ctr) => func(sp.Provider);
					break;
				case ServiceImplementType.Instance:
					var ins = impl.ImplementInstance;
					Creator = (sp, si, ctr) => ins;
					break;
				case ServiceImplementType.Method:
					var method = impl.ImplementMethod;
					var fn = method.IsGenericMethodDefinition ?
						method
							.MakeGenericMethod(ServiceType.GetGenericArguments())
							.CreateDelegate<Func<IServiceProvider, object>>() :
						method
						.CreateDelegate<Func<IServiceProvider, object>>();
					Creator = (sp, si, ctr) => fn(sp.Provider);
					break;
				case ServiceImplementType.Type:
					{
						(Creator,ConfigLoader, CreateParameterTemplate) =
							CreatorCache == null ?
							ServiceCreatorCache.CreateServiceInstanceCreator(
								ServiceType,
								impl.ImplementType,
								Setting,
								ServiceMetadata,
								impl.IsManagedService
								) :
							CreatorCache.GetServiceInstanceCreator(
								ServiceType,
								impl.ImplementType,
								Setting,
								impl.IsManagedService
								);
						break;
					}
				default:
					throw new NotSupportedException();
			}

			return new ServiceFactory(
				Id,
				ParentId,
				LazyDataScopeId,
				decl,
				impl,
				CreateParameterTemplate,
				Creator,
				ConfigLoader
				);
		}

		Dictionary<Delegate,Lazy<Action<object>>> _SettingChangedCallbacks;

		public void NotifySettingChanged(object Setting)
		{
			lock (this)
				foreach (var cb in _SettingChangedCallbacks.Values)
					cb.Value(Setting);

		}
		public IDisposable OnSettingChanged<T>(Action<T> Callback)
		{
			if (ServiceImplement.LifeTime != ServiceImplementLifetime.Singleton)
				throw new NotSupportedException("只有单例服务支持设置更改通知");
			lock (this)
			{
				if (_SettingChangedCallbacks == null)
					_SettingChangedCallbacks = new Dictionary<Delegate, Lazy<Action<object>>>();
				_SettingChangedCallbacks.Add(Callback,new Lazy<Action<object>>(()=>{
					return null;
				}));
				return Disposable.FromAction(() =>
				{
					lock(this)
						_SettingChangedCallbacks.Remove(Callback);
				});
			}

		}
	}

}
