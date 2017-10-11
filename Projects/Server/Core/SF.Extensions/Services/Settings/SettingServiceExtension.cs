using System;

using System.Collections.Generic;
using SF.Core.ServiceManagement.Internals;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using SF.Core.ServiceManagement.Management;
using SF.Core.ServiceManagement;
using SF.Entities;
using SF.Core;
using SF.Core.ServiceManagement.Models;
using SF.Metadata;

namespace SF.Services.Settings
{
	[Comment("设置服务")]
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
					if (cfg.Value == null)
						cfg.Value = new T();
					Edit(cfg.Value);
					e.Setting = Json.Stringify(cfg);
				}
				);
		}
	}
}
