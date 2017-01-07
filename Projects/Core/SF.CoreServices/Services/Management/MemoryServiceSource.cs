using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;
using SF.Reflection;

namespace SF.Services.Management
{
	public class MemoryServiceSource : IServiceConfigLoader, IDefaultServiceLocator
	{
		public class Config : IServiceConfig
		{
			public string CreateArguments { get; }

			public string ImplementType { get; }

			public string ServiceType { get; }
			public Config(string ServiceType, string ImplementType, string CreateArguments)
			{
				this.ServiceType = ServiceType;
				this.ImplementType = ImplementType;
				this.CreateArguments = CreateArguments;
			}
		}
		static Dictionary<string, string> ServiceMap { get; } = new Dictionary<string, string>();
		static Dictionary<string, Config> Configs { get; } = new Dictionary<string, Config>();

		public IManagedServiceConfigChangedNotifier ConfigChangedNotifier { get; }

		public MemoryServiceSource(IManagedServiceConfigChangedNotifier ConfigChangedNotifier)
		{
			this.ConfigChangedNotifier = ConfigChangedNotifier;
		}
		public void SetDefaultService<I>(string Id)
		{
			ServiceMap[typeof(I).FullName] = Id;
			ConfigChangedNotifier.NotifyDefaultChanged(typeof(I).FullName);
		}
		public void SetConfig<I, T>(string Id, object config)
		{
			Configs[Id] = new Config(
				typeof(I).FullName, 
				typeof(T).FullName + "," + typeof(T).GetAssembly().GetName().Name, 
				JsonConvert.SerializeObject(config)
				);
			ConfigChangedNotifier.NotifyChanged(Id);
		}
		public IServiceConfig GetConfig(string Id)
		{
			return Configs[Id];
		}
		public string Locate(string Type)
		{
			return ServiceMap[Type];
		}
	}

}
