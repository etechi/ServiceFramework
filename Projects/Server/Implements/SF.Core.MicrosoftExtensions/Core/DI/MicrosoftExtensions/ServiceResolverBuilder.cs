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

using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using SF.Core.ServiceManagement.Internals;
using System.Linq;
namespace SF.Core.ServiceManagement
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
		public static IServiceCollection AddServices(this IServiceCollection Services, Microsoft.Extensions.DependencyInjection.IServiceCollection MSServices)
		{
			Services.Remove(typeof(Microsoft.Extensions.DependencyInjection.IServiceScopeFactory));
			Services.AddTransient< Microsoft.Extensions.DependencyInjection.IServiceScopeFactory, MSScopeFactory>();
			return Services.AddRange(
				MSServices.Select(s =>
				{
					if (s.ImplementationFactory != null)
						return new ServiceDescriptor(s.ServiceType, s.ImplementationFactory, MapLifetime(s.Lifetime));
					else if (s.ImplementationInstance != null)
						return new ServiceDescriptor(s.ServiceType, s.ImplementationInstance);
					else if (s.ImplementationType != null)
						return new ServiceDescriptor(s.ServiceType, s.ImplementationType, MapLifetime(s.Lifetime));
					else
						throw new NotSupportedException();
				}));
		}
	}
}
