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
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.ServiceManagement
{
	//public interface IServiceContainer
	//{
	//	long Id { get; }
	//	IServiceDeclaration Declaration { get; }
	//	IServiceImplement Implement { get; }
	//	object ServiceInstance { get; }
	//}
	public delegate T TypedInstanceResolver<T>(long Id);
	public delegate T NamedServiceResolver<T>(string Name);

	

	public interface IServiceResolver 
	{
		IServiceProvider Provider { get; }
		long? CurrentServiceId { get; }
		IDisposable WithScopeService(long? ServiceId);
		object ResolveServiceByIdent(long ServiceId, Type ServiceType);
		object ResolveServiceByIdent(long ServiceId);
		IServiceInstanceDescriptor ResolveDescriptorByIdent(long ServiceId, Type ServiceType);
		IServiceInstanceDescriptor ResolveDescriptorByIdent(long ServiceId);

		object ResolveServiceByType(long? ScopeServiceId, Type ChildServiceType,string Name);
		IServiceInstanceDescriptor ResolveDescriptorByType(long? ScopeServiceId, Type ChildServiceType, string Name);

		//IServiceProvider CreateInternalServiceProvider(IServiceInstanceDescriptor Descriptor);

		IEnumerable<IServiceInstanceDescriptor> ResolveServiceDescriptors(
			long? ScopeServiceId, 
			Type ChildServiceType,
			string Name
			);

		IEnumerable<object> ResolveServices(
			long? ScopeServiceId,
			Type ChildServiceType,
			string Name
			);

	}
}
