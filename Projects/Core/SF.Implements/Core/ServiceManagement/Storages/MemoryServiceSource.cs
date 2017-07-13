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
			public long Id { get; set; }

			public long? ParentId { get; set; }

			public string ServiceType { get; set; }

			public string ImplementType { get; set; }

			public string Setting { get; set; }
		}
		static Dictionary<string, long> ServiceMap { get; } = new Dictionary<string, long>();
		static Dictionary<long, Config> Configs { get; } = new Dictionary<long, Config>();

		public IServiceInstanceConfigChangedNotifier ConfigChangedNotifier { get; }

		public MemoryServiceSource(IServiceInstanceConfigChangedNotifier ConfigChangedNotifier)
		{
			this.ConfigChangedNotifier = ConfigChangedNotifier;
		}
		public void SetDefaultService<I>(long ServiceInstanceId, long? ScopeId=null)
		{
			ServiceMap[typeof(I).FullName+"-"+ ScopeId] = ServiceInstanceId;
			ConfigChangedNotifier.NotifyDefaultChanged(ScopeId, typeof(I).FullName);
		}
		public Config SetConfig<I,T>(long Id, object Settings,long? ParentId=null)
			where T:I
		{
			var cfg= new Config
			{
				ServiceType = typeof(I).FullName,
				Id = Id,
				ParentId=ParentId,
				ImplementType=typeof(T).FullName,
				Setting=Json.Stringify(Settings)
			};
			
			Configs[Id] = cfg;
			ConfigChangedNotifier.NotifyChanged( Id);
			return cfg;
		}
		public IServiceConfig GetConfig(long Id)
		{
			return Configs[Id];
		}
		public long? Locate(long? ScopeId,string Type)
		{
			return ServiceMap.Get(Type + "-" + ScopeId);
		}
	}

}
