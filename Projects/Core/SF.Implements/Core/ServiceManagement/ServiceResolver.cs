using System;
using System.Collections.Generic;

using System.Linq;
using System.Collections;
using SF.Core.ServiceManagement.Internals;

namespace SF.Core.ServiceManagement
{
	class ServiceResolver : HashSet<Type>, IServiceResolver, IServiceProvider
	{
		IServiceFactoryManager FactoryManager { get; }
		ServiceScopeBase ScopeBase { get; }

		public IServiceProvider Provider => this;

		public ServiceResolver(
			ServiceScopeBase ScopeBase,
			IServiceFactoryManager FactoryManager
			)
		{
			this.ScopeBase = ScopeBase;
			this.FactoryManager = FactoryManager;
		}
		void AddType(Type ServiceType)
		{
			if (!Add(ServiceType))
				throw new InvalidOperationException($"服务{ServiceType}已经在获取中");
		}
		public IServiceInstanceDescriptor ResolveDescriptorByIdent(long ServiceId, Type ServiceType)
		{
			AddType(ServiceType);
			try
			{
				return FactoryManager.GetServiceFactoryByIdent(
					this,
					ServiceId,
					ServiceType
					);
			}
			finally
			{
				Remove(ServiceType);
			}
		}
		public IServiceInstanceDescriptor ResolveDescriptorByType(long? ScopeServiceId, Type ServiceType, string Name)
		{
			AddType(ServiceType);
			try
			{
				return FactoryManager.GetServiceFactoryByType(
					this,
					ScopeServiceId,
					ServiceType,
					Name
					);
			}
			finally
			{
				Remove(ServiceType);
			}
		}

		static Type IServiceInstanceDescriptorType { get; } = typeof(IServiceInstanceDescriptor);

		public object ResolveServiceByType(long? ScopeServiceId, Type ServiceType, string Name)
		{
			AddType(ServiceType);
			try
			{
				var f = FactoryManager.GetServiceFactoryByType(
					this,
					ScopeServiceId,
					ServiceType,
					Name
					);
				return f == null ? null : GetService(f, ServiceType);
			}
			finally
			{
				Remove(ServiceType);
			}
		}
		public object ResolveServiceByIdent(long ServiceId, Type ServiceType)
		{
			AddType(ServiceType);
			try
			{
				var f = ScopeBase.FactoryManager.GetServiceFactoryByIdent(
					this,
					ServiceId,
					ServiceType
					);
				return f == null ? null : GetService(f, ServiceType);
			}
			finally
			{
				Remove(ServiceType);
			}
		}

		public IServiceProvider CreateInternalServiceProvider(IServiceInstanceDescriptor Descriptor)
		{
			return new ScopedServiceProvider
			{
				ServiceResolver = this,
				ScopeServiceInstanceDescriptor = Descriptor
			};
		}

		public IEnumerable<IServiceInstanceDescriptor> ResolveServiceDescriptors(
			long? ScopeServiceId,
			Type ChildServiceType,
			string Name
			)
		{
			AddType(ChildServiceType);
			try
			{
				return FactoryManager.GetServiceFactoriesByType(
					this,
					ScopeServiceId,
					ChildServiceType,
					Name
					);
			}
			finally
			{
				Remove(ChildServiceType);
			}
		}

		public IEnumerable<object> ResolveServices(long? ScopeServiceId, Type ChildServiceType, string Name)
		{
			AddType(ChildServiceType);
			try
			{
				foreach (var factory in FactoryManager.GetServiceFactoriesByType(
					this,
					ScopeServiceId,
					ChildServiceType,
					Name
					))
				{
					yield return GetService(factory, ChildServiceType);
				}
			}
			finally
			{
				Remove(ChildServiceType);
			}
		}
		object GetService(IServiceFactory factory, Type ServiceType) =>
			ScopeBase.GetService(factory, ServiceType, this);

		public object GetService(Type serviceType)
		{
			if (serviceType == Types.ServiceResolverType)
				return this;
			return ResolveServiceByType(null, serviceType, null);
		}
	}

}
