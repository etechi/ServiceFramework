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

using SF.Sys;
using SF.Sys.Entities;
using SF.Sys.Services;
using SF.Sys.Services.Management;
using SF.Sys.Services.Management.Models;
using System;
using System.Threading.Tasks;

namespace SF.Sys.Settings
{
	/// <summary>
	/// 设置服务
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class SettingService<T> : ISettingService<T>
	{
		public T Value { get; }
		public SettingService(T Value)
		{
			this.Value = Value;
		}
	}
	
	public static class SettingServiceExtension
	{
		public static IServiceCollection AddSetting<T>(
			   this IServiceCollection sc,
			   T DefaultSetting = null
			   ) where T : class
		{
			sc.AddManaged(
				typeof(ISettingService<T>),
				typeof(SettingService<T>),
				ServiceImplementLifetime.Scoped
				);
			sc.InitServices($"初始化配置:{typeof(T)}", async (sp, sim, ParentId) =>
			{
				await sim.EnsureDefaultService(
					ParentId,
					typeof(ISettingService<T>),
					typeof(SettingService<T>),
					(object)DefaultSetting ?? new { }
					);
			});
			return sc;
		}
		class Data<T>
		{
			public T Value { get; set; }
		}
		public static T Setting<T>(this IServiceProvider sp)
		{
			return sp.WithScope(isp => isp.Resolve<ISettingService<T>>().Value);
		}
		public static async Task UpdateSetting<T>(
			this IServiceInstanceManager Manager,
			long? ParentId,
			Action<T> Edit
			) where T:new()
		{
			var si = await Manager.TryGetDefaultService<ISettingService<T>>(ParentId);
			if (si == null)
			{
				var setting = new Data<T> { Value = new T() };
				Edit(setting.Value);

				await Manager.EnsureDefaultService(
					ParentId,
					typeof(ISettingService<T>),
					typeof(SettingService<T>),
					setting
					);
			}
			else
				await Manager.UpdateEntity(
				ObjectKey.From(si.Id),
				(ServiceInstanceEditable e) => {
					var cfg = e.Setting.IsNullOrEmpty() ? new Data<T> {  } : Json.Parse<Data<T>>(e.Setting);
					if (cfg.Value.IsDefault())
						cfg.Value = new T();
					Edit(cfg.Value);
					e.Setting = Json.Stringify(cfg);
				}
				);
		}
	}
}
