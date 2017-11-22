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

namespace SF.Sys.Services.Internals
{
	
	
	class ServiceCreatorCache
	{
		
		//服务实例创建缓存
		ConcurrentDictionary<(Type, Type), (ServiceCreator,ServiceConfigLoader, ConstructorInfo)> ServiceCreatorDict { get; }
			= new ConcurrentDictionary<(Type, Type), (ServiceCreator, ServiceConfigLoader, ConstructorInfo)>();

		IServiceMetadata ServiceMetadata { get; }
		public ServiceCreatorCache(IServiceMetadata ServiceMetadata)
		{
			this.ServiceMetadata = ServiceMetadata;
		}

		public static (ServiceCreator,ServiceConfigLoader, ConstructorInfo) CreateServiceCreator(
			Type ServiceType, 
			Type ImplementType,
			IServiceMetadata ServiceMetadata,
			bool IsManagedService
			)
		{
			var ci = ServiceCreatorBuilder
				.FindBestConstructorInfo(ImplementType, ServiceMetadata)
				.IsNotNull(
					() => $"找不到服务实现类型{ImplementType}的构造函数"
					);
			var result = ServiceCreatorBuilder.Build(
				ServiceMetadata,
				ServiceType,
				ImplementType,
				ci,
				IsManagedService
				);
			return (result.creator,result.loader, ci);
		}
		public static (ServiceCreator,ServiceConfigLoader, IServiceCreateParameterTemplate) CreateServiceInstanceCreator(
			Type ServiceType,
			Type ImplementType,
			string Setting,
			IServiceMetadata ServiceMetadata,
			Func<Type,Type,bool,(ServiceCreator, ServiceConfigLoader, ConstructorInfo)> ServiceCreator,
			bool IsManagedService
			)
		{
			var realImplType = ImplementType.IsGenericTypeDefinition ?
							ImplementType.MakeGenericType(ServiceType.GetGenericArguments()) :
							ImplementType;

			var (Creator, cfgLoader, ConstructorInfo) = ServiceCreator(ServiceType, realImplType, IsManagedService);
			var CreateParameterTemplate = ServiceCreateParameterTemplate.Load(
				ConstructorInfo,
				Setting,
				ServiceMetadata
				);
			return (Creator, cfgLoader,CreateParameterTemplate);
		}

		public static (ServiceCreator,ServiceConfigLoader, IServiceCreateParameterTemplate) CreateServiceInstanceCreator(
			Type ServiceType,
			Type ImplementType,
			string Setting,
			IServiceMetadata ServiceMetadata,
			bool IsManagedService
			)
			=> CreateServiceInstanceCreator(
				ServiceType,
				ImplementType,
				Setting,
				ServiceMetadata,
				(st,it, ims) =>CreateServiceCreator(st,it, ServiceMetadata, ims),
				IsManagedService
				);

		public (ServiceCreator,ServiceConfigLoader,ConstructorInfo) GetServiceCreator(
			Type ServiceType, Type ImplementType,bool IsManagedService)
			=> ServiceCreatorDict.GetOrAdd(
				(ServiceType, ImplementType), 
				key => CreateServiceCreator(key.Item1,key.Item2,ServiceMetadata,IsManagedService)
				);


		public (ServiceCreator, ServiceConfigLoader,IServiceCreateParameterTemplate) GetServiceInstanceCreator(
			Type ServiceType,
			Type ImplementType,
			string Setting,
			bool IsManagedService
			)
			=> CreateServiceInstanceCreator(
				ServiceType,
				ImplementType,
				Setting,
				ServiceMetadata,
				GetServiceCreator,
				IsManagedService
				);

	}

}
