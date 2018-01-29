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
using System.Text;
using System.Threading.Tasks;
using SF.Sys.Annotations;
using SF.Sys.Entities;
using SF.Sys.NetworkService;
using SF.Sys.Reminders;

namespace SF.Sys.Services
{
	public static class ReminderDIHelper
	{
		class RemindableDefination<TRemindable> : IRemindableDefination where TRemindable : class, IRemindable
		{
			public string Name { get; set; }

			public int RetryMaxCount { get; set; }

			public int RetryDelayStart { get; set; }

			public int RetryDelayStep { get; set; }

			public int RetryDelayMax { get; set; }

			public IRemindable CreateRemindable(IServiceProvider ServiceProvider, long? ScopeId)
			{
				return ServiceProvider.Resolve<TRemindable>();
			}
		}

		public static IServiceCollection AddRemindable<TRemindable>(
			this IServiceCollection sc,
			int RetryMaxCount = int.MaxValue,
			int RetryDelayStart = 10,
			int RetryDelayStep = 10,
			int RetryDelayMax = int.MaxValue,
			string Name = null
		) where TRemindable : class, IRemindable
		{
			sc.AddScoped<TRemindable>();
			return sc.AddSingleton<IRemindableDefination>(
				sp =>
				new RemindableDefination<TRemindable>
				{
					Name = Name ?? typeof(TRemindable).FullName,
					RetryDelayMax = RetryDelayMax,
					RetryDelayStart = RetryDelayStart,
					RetryDelayStep = RetryDelayStep,
					RetryMaxCount = RetryMaxCount
				});
		}

	}
}
