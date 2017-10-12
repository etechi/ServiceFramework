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

using SF.Core.ServiceFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Core.ServiceManagement
{
	public static class BootableDIServiceCollectionExtension
	{
		class BootableHelper : IServiceBootable
		{
			public Func<Task<IDisposable>> Callback { get; set; }
			public Task<IDisposable> Boot()
			{
				return Callback();
			}
		}
		public static IServiceCollection AddBootstrap(
			this IServiceCollection sc, 
			Func<IServiceProvider,Task<IDisposable>> Callback)
			{
				sc.AddSingleton<IServiceBootable>(sp =>
					new BootableHelper
					{
						Callback = () => Callback(sp)
					});
				return sc;
			}
		public static IServiceCollection AddBootstrap(this IServiceCollection sc, Func<IServiceProvider,Task> Callback)
			=> sc.AddBootstrap(
				async sp => {
					await Callback(sp);
					return Disposable.Empty;
				}
				);
		public static IServiceCollection AddBootstrap(
			this IServiceCollection sc,
			Func<IServiceProvider, IDisposable> Callback)
			=> sc.AddBootstrap(
				sp =>
				Task.FromResult(Callback(sp))
				);
		
		public static IServiceCollection AddBootstrap(this IServiceCollection sc, Action<IServiceProvider> Callback)
			=> sc.AddBootstrap(
				sp => {
					Callback(sp);
					return Disposable.Empty;
				}
				);
	}
}
