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

using SF.Sys.Services.Internals;
using System.Collections.Generic;
using System;
using System.Linq;
using SF.Sys.Collections.Generic;
using SF.Sys.Events;
using SF.Sys.Reflection;

namespace SF.Sys.Services.Storages
{
	public class MemoryServiceSource : IServiceConfigLoader, IServiceInstanceLister
	{
        public class Meta : IServiceInstanceMeta
        {
            public string Name { get; set; }

            public string Title { get; set; }

            public string Image { get; set; }

            public string Icon { get; set; }

            public string Description { get; set; }
        }
        public class Config : IServiceConfig
		{
			public long Id { get; set; }
			public long? ContainerId { get; set; }

			public string ServiceType { get; set; }
            public string ServiceIdent { get; set; }
			public string ImplementType { get; set; }

			public string Setting { get; set; }
			public int Priority { get; set; } = -1;
            public Meta Meta { get; set; }
            IServiceInstanceMeta IServiceConfig.Meta => Meta;

        }
		Dictionary<string, SortedList<int,Config> > ServiceList { get; } = new Dictionary<string, SortedList<int, Config>>();
		Dictionary<long, Config> Configs { get; } = new Dictionary<long, Config>();

		public IEventEmitService EventEmitter { get; }

		public MemoryServiceSource(IEventEmitService EventEmitter)
		{
			this.EventEmitter = EventEmitter;
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
			SetConfig(cfg.ServiceType, cfg.ImplementType, cfg.Id, cfg.Setting, 0, cfg.ContainerId,cfg.ServiceIdent);
		}
		Config SetConfig(
			string ServiceType,
			string ImplementType,
			long Id, 
			object Settings, 
			int Priority = -1, 
			long? ParentId = null,
			string ServiceIdent = null,
            string Name=null,
            string Title=null
			)
		{
			var svcs = GetServices(ServiceType, ParentId);
			var cfg = Configs.Get(Id);

            if (cfg == null)
            {
                cfg = new Config();
                cfg.Meta = new Meta();
            }
            else if (cfg.ContainerId != ParentId)
                throw new ArgumentException();

			cfg.ContainerId = ParentId;
			cfg.ImplementType = ImplementType;
			cfg.ServiceType = ServiceType;
			if(Priority!=-1)
				cfg.Priority = Priority;
			cfg.Setting = Settings is string ? (string)Settings : Json.Stringify(Settings);
			cfg.Id = Id;
            cfg.ServiceIdent= ServiceIdent;
            cfg.Meta.Name = Name ?? ImplementType;
            cfg.Meta.Title = Title ?? cfg.Meta.Name;
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
			EventEmitter.Emit(
				new InternalServiceChanged
				{
					ScopeId = ParentId,
					ServiceType = ServiceType
				}).Wait();
			EventEmitter.Emit(
				new ServiceInstanceChanged
				{
					Id = Id
				}).Wait();
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
				.Select(c =>new ServiceReference { Id = c.Id, ServiceIdent = c.ServiceIdent })
				.ToArray();
		}
	}

}
