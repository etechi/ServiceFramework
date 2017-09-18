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

namespace SF.Services.Settings
{
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
		class Data<T>
		{
			public T Value { get; set; }
		}
		public static async Task UpdateSetting<T>(
			this IServiceInstanceManager Manager,
			long? ParentId,
			Action<T> Edit
			) where T:new()
		{
			var id = await Manager.TryGetDefaultService<ISettingService<T>>(ParentId);
			if (id == 0)
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
				id,
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
