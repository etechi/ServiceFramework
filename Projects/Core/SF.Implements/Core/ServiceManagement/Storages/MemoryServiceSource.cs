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
		public class InterfaceConfig : IServiceInterfaceConfig
		{
			public string ImplementType { get; set; }

			public string Setting { get; set; }
		}
		public class Config : IServiceConfig
		{
			public string ServiceType { get; set; }

			public string Id { get; set; }
			public int AppId { get; set; }
			public IReadOnlyDictionary<string, IServiceInterfaceConfig> Settings { get; set; } = new Dictionary<string, IServiceInterfaceConfig>();
			public Config Interface<I,T>(object Setting)
			{
				((Dictionary < string, IServiceInterfaceConfig > )Settings)[typeof(I).FullName] = new InterfaceConfig
				{
					ImplementType = typeof(T).FullName,
					Setting = JsonConvert.SerializeObject(Settings)
				};
				return this;
			}
		}
		static Dictionary<string, string> ServiceMap { get; } = new Dictionary<string, string>();
		static Dictionary<string, Config> Configs { get; } = new Dictionary<string, Config>();

		public IServiceInstanceConfigChangedNotifier ConfigChangedNotifier { get; }

		public MemoryServiceSource(IServiceInstanceConfigChangedNotifier ConfigChangedNotifier)
		{
			this.ConfigChangedNotifier = ConfigChangedNotifier;
		}
		public void SetDefaultService<I>(string Id,int AppId=1)
		{
			ServiceMap[typeof(I).FullName+"-"+AppId] = Id;
			ConfigChangedNotifier.NotifyDefaultChanged(typeof(I).FullName,AppId);
		}
		public Config SetConfig<I>(string Id,Func<Config,Config> Init,int AppId=1)
		{
			var key = typeof(I).FullName + "-" + Id+"-"+AppId;
			var cfg= new Config
			{
				ServiceType = typeof(I).FullName,
				Id = Id,
				AppId=AppId
			};
			
			Configs[key] = Init(cfg);
			ConfigChangedNotifier.NotifyChanged(typeof(I).FullName,AppId, Id);
			return cfg;
		}
		public IServiceConfig GetConfig(string Type, int AppId,string Id)
		{
			return Configs[Type+"-"+Id+"-"+AppId];
		}
		public string Locate(string Type,int AppId)
		{
			return ServiceMap.Get(Type+"-"+AppId);
		}
	}

}
