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

using SF.Core.CallPlans;
using SF.Core.CallPlans.Runtime;
using SF.Core.CallPlans.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Data;
using SF.Core.ServiceManagement;

namespace SF.Core.ServiceManagement
{
	public static class CallGuarantorDIServiceCollectionExtension
	{

		public static IServiceCollection AddCallPlans(this IServiceCollection sc,int Interval=5*1000,int ExecCountPerInterval=100)
		{
			sc.AddSingleton(sp =>
				new CallableFactory(sp.Resolve<IEnumerable<ICallableDefination>>())
				);
			sc.AddScoped<ICallDispatcher, CallDispatcher>();
			sc.AddScoped<ICallPlanProvider, CallPlanProvider>();

			sc.AddTimerService(
				"调用计划",
				Interval,
				async sp =>
				{
					await CallScheduler.Execute(sp, ExecCountPerInterval);
					return Interval;
				},
				async sp =>
					await CallScheduler.Startup(sp)
				);
			return sc;
		}

		public static IServiceCollection AddDefaultCallPlanStorage(this IServiceCollection sc,string TablePrefix=null)
		{
			sc.AddDataModules<SF.Core.CallPlans.Storage.DataModels.CallExpired, SF.Core.CallPlans.Storage.DataModels.CallInstance>(TablePrefix);
			sc.AddScoped<ICallPlanStorage, CallPlanStorage>();
			return sc;
		}
	}
}
