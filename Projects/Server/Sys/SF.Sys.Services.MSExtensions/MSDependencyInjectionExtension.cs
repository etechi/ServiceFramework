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
using Microsoft.Extensions.DependencyInjection;
using SF.Sys.Linq;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SF.Sys.Services
{
	public static class MSDependencyInjectionExtension
	{
		class MSScope : Microsoft.Extensions.DependencyInjection.IServiceScope
		{
			IServiceScope ServiceScope { get;}
			public MSScope(IServiceScope ServiceScope)
			{
				this.ServiceScope = ServiceScope;
			}
			public IServiceProvider ServiceProvider => ServiceScope.ServiceProvider;

			public void Dispose()
			{
				ServiceScope.Dispose();
			}
		}
		class MSScopeFactory :Microsoft.Extensions.DependencyInjection.IServiceScopeFactory
		{

			IServiceScopeFactory ScopeFactory { get; }
			
			public MSScopeFactory(IServiceScopeFactory ScopeFactory)
			{
				this.ScopeFactory = ScopeFactory;
			}

			public Microsoft.Extensions.DependencyInjection.IServiceScope CreateScope()
			{
				return new MSScope(ScopeFactory.CreateServiceScope());
			}
		}
		static ServiceLifetime MapLifetime(ServiceImplementLifetime lifetime)
		{
			switch (lifetime)
			{
				case ServiceImplementLifetime.Scoped:
					return ServiceLifetime.Scoped;
				case ServiceImplementLifetime.Singleton:
					return ServiceLifetime.Singleton;
					case ServiceImplementLifetime.Transient:
					return ServiceLifetime.Transient;
				default:
					throw new NotSupportedException();
			}
		}
		static ServiceImplementLifetime MapLifetime(ServiceLifetime lifetime)
		{
			switch (lifetime)
			{
				case ServiceLifetime.Scoped:
					return ServiceImplementLifetime.Scoped;
				case ServiceLifetime.Singleton:
					return ServiceImplementLifetime.Singleton;
				case ServiceLifetime.Transient:
					return ServiceImplementLifetime.Transient;
				default:
					throw new NotSupportedException();
			}
		}
		static ServiceDescriptor MapServiceDescriptor(Microsoft.Extensions.DependencyInjection.ServiceDescriptor s)
		{
			if (s.ImplementationFactory != null)
				return new ServiceDescriptor(s.ServiceType, s.ImplementationFactory, MapLifetime(s.Lifetime));
			else if (s.ImplementationInstance != null)
				return new ServiceDescriptor(s.ServiceType, s.ImplementationInstance);
			else if (s.ImplementationType != null)
				return new ServiceDescriptor(s.ServiceType, s.ImplementationType, MapLifetime(s.Lifetime));
			else
				throw new NotSupportedException();
		}
		static Microsoft.Extensions.DependencyInjection.ServiceDescriptor MapServiceDescriptor(ServiceDescriptor s)
		{
			if (s.IsManagedService)
				throw new NotSupportedException();
			switch (s.ServiceImplementType)
			{
				case ServiceImplementType.Creator:
					return new Microsoft.Extensions.DependencyInjection.ServiceDescriptor(
						s.InterfaceType,
						s.ImplementCreator,
						MapLifetime(s.Lifetime)
						);
				case ServiceImplementType.Instance:
					return new Microsoft.Extensions.DependencyInjection.ServiceDescriptor(
						s.InterfaceType,
						s.ImplementInstance
						);
				case ServiceImplementType.Type:
					return new Microsoft.Extensions.DependencyInjection.ServiceDescriptor(
						s.InterfaceType,
						s.ImplementType,
						MapLifetime(s.Lifetime)
					);
				default:
					throw new NotSupportedException();
			}
		}
		public static IServiceCollection AddMSServices(this IServiceCollection Services, Microsoft.Extensions.DependencyInjection.IServiceCollection MSServices)
		{
			if (MSServices == null)
				return Services;
			Services.Remove(typeof(Microsoft.Extensions.DependencyInjection.IServiceScopeFactory));
			Services.AddTransient< Microsoft.Extensions.DependencyInjection.IServiceScopeFactory, MSScopeFactory>();
			return Services.AddRange(MSServices.Select(MapServiceDescriptor));
		}

		class MicrosoftServiceCollectionAdapter : Microsoft.Extensions.DependencyInjection.IServiceCollection
		{
			public IServiceCollection services { get; }
			public MicrosoftServiceCollectionAdapter(IServiceCollection services)
			{
				this.services = services;
			}
			public Microsoft.Extensions.DependencyInjection.ServiceDescriptor this[int index] {
				get => MapServiceDescriptor(services.Where(s=>!s.IsManagedService).GetByIndex(index));
				set => services[index] = MapServiceDescriptor(value);
			}

			public int Count => services.Where(s=>!s.IsManagedService).Count();

			public bool IsReadOnly => services.IsReadOnly;

			public void Add(Microsoft.Extensions.DependencyInjection.ServiceDescriptor item)
			{
				services.Add(MapServiceDescriptor(item));
			}

			public void Clear()
			{
				services.Clear();
			}
			static bool EqualDescriptor(ServiceDescriptor s, ServiceDescriptor d)
			{
				return s.InterfaceType == d.InterfaceType &&
					s.IsManagedService == d.IsManagedService &&
					s.ServiceImplementType == d.ServiceImplementType &&
					s.ImplementCreator == d.ImplementCreator &&
					s.ImplementInstance == d.ImplementInstance &&
					s.ImplementType == d.ImplementType;
			}
			public bool Contains(Microsoft.Extensions.DependencyInjection.ServiceDescriptor item)
			{
				var d = MapServiceDescriptor(item);
				return services.Any(s => EqualDescriptor(s, d));
			}

			public void CopyTo(Microsoft.Extensions.DependencyInjection.ServiceDescriptor[] array, int arrayIndex)
			{
				throw new NotSupportedException();
			}

			public IEnumerator<Microsoft.Extensions.DependencyInjection.ServiceDescriptor> GetEnumerator()
			{
				return services.Where(d=>!d.IsManagedService).Select(MapServiceDescriptor).GetEnumerator();
			}

			public int IndexOf(Microsoft.Extensions.DependencyInjection.ServiceDescriptor item)
			{
				var d = MapServiceDescriptor(item);
				return services.Where(s => !s.IsManagedService).IndexOf(s => EqualDescriptor(s, d));
			}

			public void Insert(int index, Microsoft.Extensions.DependencyInjection.ServiceDescriptor item)
			{
				var d = MapServiceDescriptor(item);
				var r = services.Where(s => !s.IsManagedService).Skip(index).FirstOrDefault();
				if (r == null)
					services.Add(d);
				else
					services.Insert(services.IndexOf(r), d);
			}

			public bool Remove(Microsoft.Extensions.DependencyInjection.ServiceDescriptor item)
			{
				var d = MapServiceDescriptor(item);
				var idx = services.IndexOf(s => EqualDescriptor(s, d));
				if (idx == -1)
					return false;
				services.RemoveAt(idx);
				return true;
			}

			public void RemoveAt(int index)
			{
				services.Remove(services.Where(s => !s.IsManagedService).GetByIndex(index));
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}
		public static Microsoft.Extensions.DependencyInjection.IServiceCollection AsMicrosoftServiceCollection(this IServiceCollection sc)
		{
			return new MicrosoftServiceCollectionAdapter(sc);
		}

		
	}
}
