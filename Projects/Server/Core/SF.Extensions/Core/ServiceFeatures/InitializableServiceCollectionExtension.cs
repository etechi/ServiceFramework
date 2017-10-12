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
	public static class InitializableDIServiceCollectionExtension
	{
		class InitHelper : IServiceInitializable
		{
			public Func<IServiceProvider,Task> Callback { get; set; }
			public string Group { get; set; }
			public string Title { get; set; }
			public int Priority { get; set; }
			public Task Init(IServiceProvider ServiceProvider)
			{
				return Callback(ServiceProvider);
			}
		}
		public static IServiceCollection AddInitializer(
			this IServiceCollection sc, 
			string Group,
			string Title,
			Func<IServiceProvider,Task> Callback,
			int Priority=0
			)
			{
				sc.AddSingleton<IServiceInitializable>(sp =>
					new InitHelper
					{
						Group=Group,
						Title=Title,
						Callback = isp => Callback(isp),
						Priority=Priority
					});
				return sc;
			}
		
		public static IServiceCollection AddInitializer(
			this IServiceCollection sc,
			string Group,
			string Title,
			Action<IServiceProvider> Callback,
			int Priority = 0)
		{
			sc.AddInitializer(
				Group,
				Title,
				sp =>
				{
					Callback(sp);
					return Task.CompletedTask;
				},
				Priority
				);
			return sc;
		}
		
	}
}
