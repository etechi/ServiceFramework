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


using SF.Sys.Events;
using SF.Sys.Services;
using SF.Sys.Services.Internals;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Sys.Settings
{
	public class SettingChangedTrackerService : ISettingChangedTrackerService
	{
		ConcurrentDictionary<Type, List<Func<IServiceProvider, Task>>> Callbacks { get; }
			= new ConcurrentDictionary<Type, List<Func<IServiceProvider, Task>>>();
		IServiceScopeFactory ScopeFactory { get; }
		public SettingChangedTrackerService(
			IEventSubscriber<ServiceInstanceChanged> Subscriber, 
			IServiceScopeFactory ScopeFactory,
			IServiceProvider ServiceProvider
			)
		{
			this.ScopeFactory = ScopeFactory;
			var resolver = ServiceProvider.Resolver();
			Subscriber.Wait((ei) =>
			{
				SF.Sys.Threading.Debouncer.Start(ei.Id, (cancelled) =>
				{
					if (cancelled) return;
					Task.Run(async () =>
					{
						var desc = resolver.ResolveDescriptorByIdent(ei.Event.Id);
						if (!Callbacks.TryGetValue(desc.ServiceDeclaration.ServiceType, out var cbs))
							return;

						Func<IServiceProvider, Task>[] cba;
						lock (cbs)
							cba = cbs.ToArray();
						using (var sc = ScopeFactory.CreateServiceScope())
							foreach (var cb in cba)
								await cb(sc.ServiceProvider);
					});

				}, 100, 1000);
				return Task.CompletedTask;			
			});
		}
		public IDisposable OnSettingChanged<T>(Func<IServiceProvider, Task> Callback)
		{
			var type = typeof(ISettingService<T>);
			if (!Callbacks.TryGetValue(type, out var list))
				list = Callbacks.GetOrAdd(type, new List<Func<IServiceProvider, Task>>());
			lock (list)
			{
				list.Add(Callback);
			}
			return Disposable.FromAction(() =>
			{
				lock (list)
				{
					list.Remove(Callback);
				}
			});
		}
	}
}
