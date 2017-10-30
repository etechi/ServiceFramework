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

namespace SF.Core.ServiceManagement
{
	public class ServiceCollection : IServiceCollection
	{
		List<ServiceDescriptor> Descriptors { get; } = new List<ServiceDescriptor>();
		HashSet<Type> ServiceTypeHash { get; } = new HashSet<Type>();
		public IEnumerable<Type> ServiceTypes=> ServiceTypeHash;

		public int Count => Descriptors.Count;

		public bool IsReadOnly => false;

		public ServiceDescriptor this[int index] { get => Descriptors[index]; set => Descriptors[index]=value; }

		

		public void Add(ServiceDescriptor Descriptor)
		{
			Descriptors.Add(Descriptor);
			//if (Descriptor.Lifetime != ServiceInterfaceImplementLifetime.Scoped && Descriptor.Lifetime != ServiceInterfaceImplementLifetime.Transient)
			//	throw new NotSupportedException();
			//if (Descriptor.ImplementType == null || Descriptor.ImplementInstance != null || Descriptor.ImplementCreator != null)
			//	throw new NotSupportedException();
			//if (ServiceTypeHash.Add(Descriptor.ServiceType))
			//	NormalServiceCollection.Add(
			//		new ServiceDescriptor(
			//			Descriptor.ServiceType,
			//			isp => isp.Resolve<Runtime.IManagedServiceScope>().Resolve(
			//				Descriptor.ServiceType, 
			//				null
			//				),
			//			Descriptor.Lifetime
			//		));
		}

		public IEnumerator<ServiceDescriptor> GetEnumerator()
		{
			return Descriptors.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public int IndexOf(ServiceDescriptor item)
		{
			return Descriptors.IndexOf(item);
		}

		public void Insert(int index, ServiceDescriptor item)
		{
			Descriptors.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			Descriptors.RemoveAt(index);
		}

		public void Clear()
		{
			Descriptors.Clear();
		}

		public bool Contains(ServiceDescriptor item)
		{
			return Descriptors.Contains(item);
		}

		public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
		{
			Descriptors.CopyTo(array, arrayIndex);
		}

		public bool Remove(ServiceDescriptor item)
		{
			return Descriptors.Remove(item);
		}
	}

}
