using Newtonsoft.Json;
using SF.Core.ManagedServices.Runtime;
using SF.Core.ServiceManagement.Internals;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

namespace SF.Core.ServiceManagement.Storages
{
	public class MemoryServiceSource : IServiceConfigLoader, IServiceInstanceLister
	{

		public class Config : IServiceConfig
		{
			public long Id { get; set; }

			public long? ParentId { get; set; }

			public string ServiceType { get; set; }

			public string ImplementType { get; set; }

			public string Setting { get; set; }
			public int Priority { get; set; } = -1;
			public string Name { get; set; }
		}
		Dictionary<string, SortedList<int,Config> > ServiceList { get; } = new Dictionary<string, SortedList<int, Config>>();
		Dictionary<long, Config> Configs { get; } = new Dictionary<long, Config>();

		public IServiceInstanceConfigChangedNotifier ConfigChangedNotifier { get; }

		public MemoryServiceSource(IServiceInstanceConfigChangedNotifier ConfigChangedNotifier)
		{
			this.ConfigChangedNotifier = ConfigChangedNotifier;
		}
		
		SortedList<int,Config> GetServices(string type,long? ParentId)
		{
			var key = type + "-" + ParentId;
			var svcs = ServiceList.Get(key);
			if (svcs == null)
				ServiceList[key] = svcs = new SortedList<int, Config>();
			return svcs;
		}
		public void SetDefaultService<I>(long Id)
		{
			var cfg = GetConfig(Id);
			SetConfig(cfg.ServiceType, cfg.ImplementType, cfg.Id, cfg.Setting, 0, cfg.ParentId,cfg.Name);
		}
		Config SetConfig(
			string ServiceType,
			string ImplementType,
			long Id, 
			object Settings, 
			int Priority = -1, 
			long? ParentId = null,
			string Name=null
			)
		{
			var svcs = GetServices(ServiceType, ParentId);
			var cfg = Configs.Get(Id);

			if (cfg == null)
				cfg = new Config();
			else if (cfg.ParentId != ParentId)
				throw new ArgumentException();

			cfg.ParentId = ParentId;
			cfg.ImplementType = ImplementType;
			cfg.ServiceType = ServiceType;
			if(Priority!=-1)
				cfg.Priority = Priority;
			cfg.Setting = Settings is string ? (string)Settings : Json.Stringify(Settings);
			cfg.Id = Id;
			cfg.Name = Name;
			Configs[Id] = cfg;

			var idx = svcs.IndexOfValue(cfg);
			if (idx != -1)
				svcs.RemoveAt(idx);
			if (cfg.Priority == -1)
				cfg.Priority = svcs.Count == 0 ? 1 : svcs.Keys.Max() + 1;
			var re = svcs.Values.ToList().OrderBy(i => i.Priority).ToList();
			re.ModifyPosition(
				cfg,
				PositionModifyAction.Insert,
				c => c == cfg,
				c => c.Priority,
				(c, i) => c.Priority = i
				);
			svcs.Clear();
			re.ForEach(i => svcs.Add(i.Priority, i));
			ConfigChangedNotifier.NotifyInternalServiceChanged(ParentId, ServiceType);
			ConfigChangedNotifier.NotifyChanged(Id);
			return cfg;
		}
		public Config SetConfig<I,T>(long Id, object Settings,int Priority=-1,long? ParentId=null,string Name=null)
			where T:I
		{
			return SetConfig(typeof(I).GetFullName(), typeof(T).GetFullName(), Id, Settings, Priority, ParentId,Name);
		}
		public IServiceConfig GetConfig(long Id)
		{
			if (!Configs.TryGetValue(Id, out var cfg))
				throw new ArgumentException("找不到服务配置:" + Id);
			return cfg;
		}
		public ServiceReference[] List(long? ScopeId,string Type,int Limit)
		{
			var svcs = GetServices(Type, ScopeId);
			return svcs.Values
				.Take(Limit)
				.Select(c =>new ServiceReference { Id = c.Id, Name = c.Name })
				.ToArray();
		}
	}

}
