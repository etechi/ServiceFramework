using Newtonsoft.Json;
using SF.Core.ManagedServices.Runtime;
using SF.Core.ServiceManagement.Internals;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace SF.Core.ServiceManagement.Storages
{
	public class MemoryServiceSource : IServiceConfigLoader, IDefaultServiceLocator
	{
		public class Config : IServiceConfig
		{

			public string ImplementType { get; }

			public string ServiceType { get; }

			public long Id { get; }

			public string Settings { get; }

			public Config(string ServiceType, string ImplementType, string Settings,long Id)
			{
				this.ServiceType = ServiceType;
				this.ImplementType = ImplementType;
				this.Settings = Settings;
				this.Id = Id;
			}
		}
		static Dictionary<string, long?> ServiceMap { get; } = new Dictionary<string, long?>();
		static Dictionary<string, Config> Configs { get; } = new Dictionary<string, Config>();

		public IServiceInstanceConfigChangedNotifier ConfigChangedNotifier { get; }

		public MemoryServiceSource(IServiceInstanceConfigChangedNotifier ConfigChangedNotifier)
		{
			this.ConfigChangedNotifier = ConfigChangedNotifier;
		}
		public void SetDefaultService<I>(long Id)
		{
			ServiceMap[typeof(I).FullName] = Id;
			ConfigChangedNotifier.NotifyDefaultChanged(typeof(I).FullName);
		}
		public void SetConfig<I, T>(long Id, object config)
		{
			var key = typeof(I).FullName + "-" + Id;
			Configs[key] = new Config(
				typeof(I).FullName, 
				typeof(T).FullName + "," + typeof(T).GetAssembly().GetName().Name, 
				JsonConvert.SerializeObject(config),
				Id
				);
			ConfigChangedNotifier.NotifyChanged(typeof(T).FullName, Id);
		}
		public IServiceConfig GetConfig(string Type,long Id)
		{
			return Configs[Type+"-"+Id];
		}
		public long? Locate(string Type)
		{
			return ServiceMap.Get(Type);
		}
	}

}
