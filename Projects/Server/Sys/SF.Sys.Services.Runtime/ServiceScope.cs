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

using SF.Sys.Services.Internals;
using System;
using System.Collections.Generic;
namespace SF.Sys.Services
{
	public class ServiceScope : ServiceScopeBase,
		IServiceScope
	{

		IServiceProvider RootServiceProvider { get; }
		public ServiceScope(IServiceProvider ServiceProvider, IServiceFactoryManager ServiceFactoryManager) :
			base(ServiceFactoryManager)
		{
			this.RootServiceProvider = ServiceProvider;
		}
		internal override object GetService(IServiceFactory factory,Type ServiceType,IServiceResolver ServiceResolver)
		{
			if (factory.ServiceImplement.LifeTime == ServiceImplementLifetime.Singleton)
				return ((ServiceScopeBase)RootServiceProvider).GetService(factory, ServiceType, ServiceResolver);
			//.GetService(factory.ServiceDeclaration.ServiceType);
			return base.GetService(factory, ServiceType, ServiceResolver);
		}

		protected override CacheType GetCacheType(IServiceFactory Factory)
		{
			switch (Factory.ServiceImplement.LifeTime)
			{
				case ServiceImplementLifetime.Scoped:
					return CacheType.CacheScoped;
				case ServiceImplementLifetime.Singleton:
					return CacheType.NoCache;
				case ServiceImplementLifetime.Transient:
					return CacheType.CacheDisposable;
				default:
					throw new NotSupportedException();
			}
		}
	}

}
