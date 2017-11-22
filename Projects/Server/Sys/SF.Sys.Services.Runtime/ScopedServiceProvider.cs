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
using System.Collections;
using System.Reflection;
using SF.Sys.Services.Internals;
using SF.Sys.Services;

namespace SF.Sys.Services
{

	//class ScopedServiceProvider : IServiceProvider, IServiceResolver
	//{
	//	public IServiceInstanceDescriptor ScopeServiceInstanceDescriptor { get; set; }
	//	public ServiceResolver ServiceResolver { get; set; }
	//	public IServiceProvider Provider => this;
	//	public IServiceProvider CreateInternalServiceProvider(IServiceInstanceDescriptor Descriptor)
	//	{
	//		return new ScopedServiceProvider
	//		{
	//			ScopeServiceInstanceDescriptor = Descriptor,
	//			ServiceResolver = ServiceResolver
	//		};
	//	}

	//	public object GetService(Type serviceType)
	//	{
	//		if (serviceType == Types.ServiceResolverType)
	//			return this;
	//		if (serviceType == Types.ServiceProviderType)
	//			return this;
	//		return ResolveServiceByType(null, serviceType, null);
	//	}

	//	public IServiceInstanceDescriptor ResolveDescriptorByIdent(long ServiceId, Type ServiceType)
	//	{
	//		return ServiceResolver.ResolveDescriptorByIdent(ServiceId, ServiceType);
	//	}

	//	public IServiceInstanceDescriptor ResolveDescriptorByType(long? ScopeServiceId, Type ChildServiceType, string Name)
	//	{
	//		if (!ScopeServiceId.HasValue)
	//		{
	//			if (ChildServiceType == Types. ServiceInstanceDescriptorType)
	//				return ScopeServiceInstanceDescriptor;
	//		}
	//		//if (ChildServiceType == ServiceResolverType)
	//		//	return ServiceResolver;
	//		return ServiceResolver.ResolveDescriptorByType(ScopeServiceInstanceDescriptor.ParentInstanceId, ChildServiceType, null);
	//	}

	//	public object ResolveServiceByIdent(long ServiceId, Type ServiceType)
	//	{
	//		if (ServiceType == Types.ServiceInstanceDescriptorType && ServiceId == ScopeServiceInstanceDescriptor.InstanceId)
	//			return ScopeServiceInstanceDescriptor;
	//		return ServiceResolver.ResolveServiceByIdent(ServiceId, ServiceType);
	//	}

	//	public object ResolveServiceByType(long? ScopeServiceId, Type ChildServiceType, string Name)
	//	{
	//		if (!ScopeServiceId.HasValue)
	//		{
	//			if (ChildServiceType == Types.ServiceInstanceDescriptorType)
	//				return ScopeServiceInstanceDescriptor;
	//		}

	//		return ServiceResolver.ResolveServiceByType(ScopeServiceId ?? ScopeServiceInstanceDescriptor.InstanceId, ChildServiceType, Name);
	//	}

	//	static IEnumerable<IServiceInstanceDescriptor> SelfDescriptor(IServiceInstanceDescriptor desc)
	//	{
	//		yield return desc;
	//	}
	//	public IEnumerable<IServiceInstanceDescriptor> ResolveServiceDescriptors(long? ScopeServiceId, Type ChildServiceType, string Name)
	//	{
	//		if (!ScopeServiceId.HasValue)
	//		{
	//			if (ChildServiceType == Types.ServiceInstanceDescriptorType)
	//				return SelfDescriptor(ScopeServiceInstanceDescriptor);
	//		}
	//		return ServiceResolver.ResolveServiceDescriptors(ScopeServiceId ?? ScopeServiceInstanceDescriptor.InstanceId, ChildServiceType, Name);
	//	}

	//	public IEnumerable<object> ResolveServices(long? ScopeServiceId, Type ChildServiceType, string Name)
	//	{
	//		if (!ScopeServiceId.HasValue)
	//		{
	//			if (ChildServiceType == Types.ServiceInstanceDescriptorType)
	//				return SelfDescriptor(ScopeServiceInstanceDescriptor);
	//		}
	//		return ServiceResolver.ResolveServices(ScopeServiceId ?? ScopeServiceInstanceDescriptor.InstanceId, ChildServiceType, Name);
	//	}
	//}

}
