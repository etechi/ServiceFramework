using System;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SF.Core.ServiceManagement.Internals
{
	
	
	class ServiceCreatorCache
	{
		
		//服务实例创建缓存
		ConcurrentDictionary<(Type, Type), (ServiceCreator, ConstructorInfo)> ServiceCreatorDict { get; }
			= new ConcurrentDictionary<(Type, Type), (ServiceCreator, ConstructorInfo)>();

		IServiceMetadata ServiceMetadata { get; }
		public ServiceCreatorCache(IServiceMetadata ServiceMetadata)
		{
			this.ServiceMetadata = ServiceMetadata;
		}

		public static (ServiceCreator, ConstructorInfo) CreateServiceCreator(
			Type ServiceType, 
			Type ImplementType,
			IServiceMetadata ServiceMetadata,
			bool IsManagedService
			)
		{
			var ci = ServiceCreatorBuilder
				.FindBestConstructorInfo(ImplementType)
				.AssertNotNull(
					() => $"找不到服务实现类型{ImplementType}的构造函数"
					);

			var creator = ServiceCreatorBuilder.Build(
				ServiceMetadata,
				ServiceType,
				ImplementType,
				ci,
				IsManagedService
				);
			return (creator, ci);
		}
		public static (ServiceCreator, IServiceCreateParameterTemplate) CreateServiceInstanceCreator(
			Type ServiceType,
			Type ImplementType,
			string Setting,
			IServiceMetadata ServiceMetadata,
			Func<Type,Type,bool,(ServiceCreator, ConstructorInfo)> ServiceCreator,
			bool IsManagedService
			)
		{
			var realImplType = ImplementType.IsGenericTypeDefinition ?
							ImplementType.MakeGenericType(ServiceType.GetGenericArguments()) :
							ImplementType;

			var (Creator, ConstructorInfo) = ServiceCreator(ServiceType, realImplType, IsManagedService);
			var CreateParameterTemplate = ServiceCreateParameterTemplate.Load(
				ConstructorInfo,
				Setting,
				ServiceMetadata
				);
			return (Creator, CreateParameterTemplate);
		}

		public static (ServiceCreator, IServiceCreateParameterTemplate) CreateServiceInstanceCreator(
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

		public (ServiceCreator,ConstructorInfo) GetServiceCreator(Type ServiceType, Type ImplementType,bool IsManagedService)
			=> ServiceCreatorDict.GetOrAdd(
				(ServiceType, ImplementType), 
				key => CreateServiceCreator(key.Item1,key.Item2,ServiceMetadata,IsManagedService)
				);


		public (ServiceCreator, IServiceCreateParameterTemplate) GetServiceInstanceCreator(
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
