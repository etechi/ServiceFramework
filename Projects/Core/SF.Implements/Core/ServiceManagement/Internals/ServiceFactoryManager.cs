using System;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SF.Core.ServiceManagement.Internals
{
	struct ServiceKey
	{
		public string Type;
		public long Id;
	}
	class ServiceKeyComparer : IEqualityComparer<ServiceKey>
	{
		public static ServiceKeyComparer Instance { get; } = new ServiceKeyComparer();

		public bool Equals(ServiceKey x, ServiceKey y)
		{
			if (x.Type != y.Type || x.Id != y.Id)
				return false;
			return true;
		}

		public int GetHashCode(ServiceKey obj)
		{
			var c = obj.Type.GetHashCode() ^ obj.Id.GetHashCode();
			return c;
		}
	}
	class ServiceFactoryManager : IServiceFactoryManager,IServiceInstanceConfigChangedNotifier
	{

		ConcurrentDictionary<string, HashSet<Type>> GenericTypes { get; } = new ConcurrentDictionary<string, HashSet<Type>>();
		ConcurrentDictionary<string, long> DefaultServiceImplementMap { get; } = new ConcurrentDictionary<string, long>();
		ConcurrentDictionary<ServiceKey, Lazy<ServiceFactory>> FactoryMap { get; } = new ConcurrentDictionary<ServiceKey, Lazy<ServiceFactory>>(ServiceKeyComparer.Instance);

		IServiceMetadata ServiceMetadata { get; }
		ConcurrentDictionary<string, HashSet<string>> GenericTypeMap { get; } = new ConcurrentDictionary<string, HashSet<string>>();

		public ServiceFactoryManager(
			IServiceMetadata ServiceMetadata
			)
		{
			this.ServiceMetadata = ServiceMetadata;
		}
		long ResolveDefaultService(IServiceProvider ServiceProvider, Type ServiceType, long Id)
		{
			
			if (Id > 0)
				return Id;
			if (DefaultServiceImplementMap.TryGetValue(ServiceType.FullName, out Id))
				return Id;

			if (Id < 0)
			{
				var svc = ServiceMetadata.Services
					.Get(ServiceType)
					.AssertNotNull(() => $"找不到服务{ServiceType}");
				if (Id + svc.Implements.Count < 0)
					throw new InvalidOperationException($"服务索引错误，必须大于等于{-svc.Implements.Count}");
			}
			else if (ServiceType == typeof(IDefaultServiceLocator) ||
				ServiceType==typeof(IServiceConfigLoader))
				Id=0;
			else
			{ 
				var DefaultServiceLocator = ServiceProvider
					.TryResolve<IDefaultServiceLocator>();
				if (DefaultServiceLocator == null)
					Id = 0;
				else
				{
					//.AssertNotNull(
					//	() => $"找不到服务{typeof(IDefaultServiceLocator)}"
					//	);

					var LocateResult = DefaultServiceLocator.Locate(ServiceType.FullName);
					if (LocateResult.HasValue)
						Id = LocateResult.Value;
					else
					{
						//ServiceMetadata.Services.Get(ServiceType)
						//	.Implements
						//	.Error(
						//		v => v.Count == 0,
						//		v => $"没有服务实现"
						//	)
						//	.Error(
						//		v => v.Count > 1,
						//		v => $"找不到默认服务"
						//		);
						Id = 0;
					}
				}
			}
			Id = DefaultServiceImplementMap.GetOrAdd(ServiceType.FullName, Id);
			return Id;
		}
		public IServiceFactory GetServiceFactory(IServiceResolver ServiceResolver, Type ServiceType, long ServiceInstanceId)
		{
			if (ServiceType == null)
				throw new ArgumentNullException();
			if (ServiceType.IsGenericTypeDefinition)
				throw new NotSupportedException();

			Lazy<ServiceFactory> entry;

			var key = new ServiceKey
			{
				Type = ServiceType.FullName,
				Id = ServiceInstanceId,
			};
			if (FactoryMap.TryGetValue(key, out entry))
				return entry.Value;
			var ServiceDeclare = ServiceMetadata.Services.Get(ServiceType);
			if (ServiceDeclare != null)
			{
				ServiceInstanceId = ResolveDefaultService(ServiceResolver, ServiceType, ServiceInstanceId);
				return FactoryMap.GetOrAdd(
				   key,
				   new Lazy<ServiceFactory>(
					   () => new ServiceFactory(
						   ServiceType,
						   ServiceDeclare,
						   ServiceInstanceId,
						   ServiceResolver,
						   ServiceMetadata
						   )
					   )
				   ).Value;
			}


			if (!ServiceType.IsGenericType)
				return null;

			var TypeArgs = ServiceType.GetGenericArguments();
			var ServiceTypeDef = ServiceType.GetGenericTypeDefinition();

			ServiceDeclare = ServiceMetadata.Services.Get(ServiceTypeDef);
			if (ServiceDeclare == null)
				return null;

			key = new ServiceKey
			{
				Type = ServiceType.FullName,
				Id = ServiceInstanceId
			};
			if (!FactoryMap.TryGetValue(key, out entry))
				entry = FactoryMap.GetOrAdd(
					key, 
					new Lazy<ServiceFactory>(
						()=>
						{
							var types = GenericTypeMap.GetOrAdd(ServiceTypeDef.FullName, k => new HashSet<string>());
							lock (types)
								types.Add(ServiceType.FullName);

							return new ServiceFactory(
								ServiceType,
								ServiceDeclare,
								ServiceInstanceId,
								ServiceResolver,
								ServiceMetadata
								);
						})
					);
			return entry.Value;
		}


		void IServiceInstanceConfigChangedNotifier.NotifyChanged(string ServiceType, long Id)
		{
			Lazy<ServiceFactory> v;
			FactoryMap.TryRemove(new ServiceKey { Type = ServiceType, Id = Id }, out v);

			//清理泛型实例类的服务
			var types = GenericTypes.Get(ServiceType);
			if(types!=null)
				lock (types)
				{
					types.ForEach(type =>
						FactoryMap.TryRemove(
							new ServiceKey { Type = ServiceType, Id = Id }, 
							out v
							)
						);
					types.Clear();
				}

		}

		void IServiceInstanceConfigChangedNotifier.NotifyDefaultChanged(string Type)
		{
			long v;
			DefaultServiceImplementMap.TryRemove(Type, out v);
		}

		
	}

}
