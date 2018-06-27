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
using System.Threading.Tasks;
using SF.Sys.Services;
using SF.Sys.Logging;

namespace SF.Sys.ServiceFeatures
{
	public static class InitializableServiceProviderExtension
	{
		class ServiceInitializer
		{
			public static ServiceInitializer Instance { get; } = new ServiceInitializer();
			public async Task<bool> InitServices(IServiceProvider sp,string Group, IReadOnlyDictionary<string, string> Args)
			{
				var logger = sp.Resolve<ILogger<ServiceInitializer>>();
				var bss = sp.Resolve<IEnumerable<IServiceInitializable>>()
					.Where(i=>i.Group== Group)
					.Reverse() //需要反向，返回顺序和注册顺序相反
					.OrderBy(i=>i.Priority)
					.ToArray();
				foreach (var bs in bss)
				{
					logger.Info("初始化开始:{0} {1}",Group, bs.Title);
					await bs.Init(sp, Args ?? new Dictionary<string,string>());
					logger.Info("初始化结束:{0} {1}", Group, bs.Title);
				}
				return true;
			}
		}
		public static async Task<bool> InitServices(this IServiceProvider sp,string Group, IReadOnlyDictionary<string, string> Args=null)
		{
			return await ServiceInitializer.Instance.InitServices(sp,Group, Args);
		}

	}
}
