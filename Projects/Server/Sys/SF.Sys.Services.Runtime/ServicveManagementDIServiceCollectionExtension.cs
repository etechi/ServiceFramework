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
using SF.Sys.Services.Internals;
using SF.Sys.Hosting;

namespace SF.Sys.Services
{
	public static class RuntimeDIServiceCollectionExtension
	{
		
		public static IServiceCollection AddManagedService(
			this IServiceCollection sc
			)
		{
			//sc.AddSingleton<IServiceFactoryManager, ServiceFactoryManager>();
			//sc.AddScoped<IServiceInstanceScope,ServiceInstanceScope>();
			sc.AddSingleton<IServiceDeclarationTypeResolver, DefaultServiceDeclarationTypeResolver>();
			sc.AddSingleton<IServiceImplementTypeResolver, DefaultServiceImplementTypeResolver>();
			sc.AddSingleton<NetworkService.IExtraServiceTypeSource, ImplementConfigTypeSource>();
			sc.AddSingleton<IServiceInvokerProvider, ServiceInvokerProvider>();
			sc.AddSingleton<IServiceConfigService, ServiceSetupService>();
			return sc;
		}
		
		public static IServiceProvider BuildServiceResolver(
			this IServiceCollection sc,
			Caching.ILocalCache<IServiceEntry> AppServiceCache=null
			)
		{
			return new ServiceProviderBuilder().Build(
				sc, 
				AppServiceCache
				);
		}

		public static IAppInstance Build(this IAppInstanceBuilder Builder)
		{
			return Builder.Build(sc => sc.BuildServiceResolver());
		}
	}

}
