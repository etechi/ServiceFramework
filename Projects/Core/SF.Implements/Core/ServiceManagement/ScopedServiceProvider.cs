using System;
using System.Collections.Generic;

using System.Linq;
using System.Collections;
using System.Reflection;
using SF.Core.ServiceManagement.Internals;
using SF.Core.ServiceManagement;

namespace SF.Core.ServiceManagement
{

	class ScopedServiceProvider : IServiceProvider, IServiceResolver
	{
		public IServiceInstanceDescriptor ScopeServiceInstanceDescriptor { get; set; }
		public ServiceResolver ServiceResolver { get; set; }
		public IServiceProvider Provider => this;
		public IServiceProvider CreateInternalServiceProvider(IServiceInstanceDescriptor Descriptor)
		{
			return new ScopedServiceProvider
			{
				ScopeServiceInstanceDescriptor = Descriptor,
				ServiceResolver = ServiceResolver
			};
		}

		public object GetService(Type serviceType)
		{
			if (serviceType == Types.ServiceResolverType)
				return this;
			if (serviceType == Types.ServiceProviderType)
				return this;
			return ResolveServiceByType(null, serviceType, null);
		}

		public IServiceInstanceDescriptor ResolveDescriptorByIdent(long ServiceId, Type ServiceType)
		{
			return ServiceResolver.ResolveDescriptorByIdent(ServiceId, ServiceType);
		}

		public IServiceInstanceDescriptor ResolveDescriptorByType(long? ScopeServiceId, Type ChildServiceType, string Name)
		{
			if (!ScopeServiceId.HasValue)
			{
				if (ChildServiceType == Types. ServiceInstanceDescriptorType)
					return ScopeServiceInstanceDescriptor;
			}
			//if (ChildServiceType == ServiceResolverType)
			//	return ServiceResolver;
			return ServiceResolver.ResolveDescriptorByType(ScopeServiceInstanceDescriptor.ParentInstanceId, ChildServiceType, null);
		}

		public object ResolveServiceByIdent(long ServiceId, Type ServiceType)
		{
			if (ServiceType == Types.ServiceInstanceDescriptorType && ServiceId == ScopeServiceInstanceDescriptor.InstanceId)
				return ScopeServiceInstanceDescriptor;
			return ServiceResolver.ResolveServiceByIdent(ServiceId, ServiceType);
		}

		public object ResolveServiceByType(long? ScopeServiceId, Type ChildServiceType, string Name)
		{
			if (!ScopeServiceId.HasValue)
			{
				if (ChildServiceType == Types.ServiceInstanceDescriptorType)
					return ScopeServiceInstanceDescriptor;
			}

			return ServiceResolver.ResolveServiceByType(ScopeServiceId ?? ScopeServiceInstanceDescriptor.InstanceId, ChildServiceType, Name);
		}

		static IEnumerable<IServiceInstanceDescriptor> SelfDescriptor(IServiceInstanceDescriptor desc)
		{
			yield return desc;
		}
		public IEnumerable<IServiceInstanceDescriptor> ResolveServiceDescriptors(long? ScopeServiceId, Type ChildServiceType, string Name)
		{
			if (!ScopeServiceId.HasValue)
			{
				if (ChildServiceType == Types.ServiceInstanceDescriptorType)
					return SelfDescriptor(ScopeServiceInstanceDescriptor);
			}
			return ServiceResolver.ResolveServiceDescriptors(ScopeServiceId ?? ScopeServiceInstanceDescriptor.InstanceId, ChildServiceType, Name);
		}

		public IEnumerable<object> ResolveServices(long? ScopeServiceId, Type ChildServiceType, string Name)
		{
			if (!ScopeServiceId.HasValue)
			{
				if (ChildServiceType == Types.ServiceInstanceDescriptorType)
					return SelfDescriptor(ScopeServiceInstanceDescriptor);
			}
			return ServiceResolver.ResolveServices(ScopeServiceId ?? ScopeServiceInstanceDescriptor.InstanceId, ChildServiceType, Name);
		}
	}

}
