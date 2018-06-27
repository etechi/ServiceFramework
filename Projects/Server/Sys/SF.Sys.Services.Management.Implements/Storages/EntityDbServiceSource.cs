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
using System;
using System.Linq;
using System.Data.Common;
using SF.Sys.Collections.Generic;
using SF.Sys.Reflection;
using SF.Sys.Data;
using SF.Sys.Services.Management.DataModels;
using System.Threading.Tasks;

namespace SF.Sys.Services.Storages
{
	public class EntityDbServiceSource : IServiceConfigLoader, IServiceInstanceLister
	{
		IScoped<IDataScope> ScopedDataScope { get; }
		IServiceMetadata Metadata { get; }
		public EntityDbServiceSource(IScoped<IDataScope> ScopedDataScope, IServiceMetadata Metadata)
		{
			this.ScopedDataScope= ScopedDataScope;
			this.Metadata = Metadata;
		}
		class Config : IServiceConfig
		{
			public long Id { get; set; }

			public long? ContainerId { get; set; }

			public string ServiceType { get; set; }

			public string ImplementType { get; set; }

			public string Setting { get; set; }
			public string Name { get; set; }
		}
		public IServiceConfig GetConfig(long Id)
		{
			return ScopedDataScope.Use(scope => scope.Use("查询配置", ctx =>
			{
				var re = (from ins in ctx.Queryable<DataServiceInstance>()
							where ins.Id == Id
							select new Config
							{
								Id = ins.Id,
								ServiceType = ins.ServiceType,
								ImplementType = ins.ImplementType,
								Setting = ins.Setting,
								ContainerId = ins.ContainerId
							}
						).SingleOrDefault();

				if (re == null)
					return null;
				var svcDef = Metadata.ServicesByTypeName.Get(re.ServiceType);
				if (svcDef == null)
					throw new InvalidOperationException($"找不到定义的服务{re.ServiceType}");

				var implType = re.ImplementType;
				var svcImpl = svcDef.Implements.SingleOrDefault(i => i.ImplementType.GetFullName() == implType);
				if (svcImpl == null)
					throw new InvalidOperationException($"找不到服务实现{implType}");
				re.ImplementType = implType;

				if (re.Setting == null) re.Setting = "{}";
				return Task.FromResult((IServiceConfig) re);
			})).Result;
		}

		ServiceReference[] IServiceInstanceLister.List(long? ScopeId, string ServiceType, int Limit)
		{
			return ScopedDataScope.Use(scope => scope.Use("查询配置", ctx =>
			{

				if (ScopeId.HasValue)
				{
					var re = (from ins in ctx.Queryable<DataServiceInstance>()
							  where ins.ContainerId == ScopeId && ins.ServiceType == ServiceType
							  orderby ins.ItemOrder
							  select new ServiceReference
							  {
								  Id = ins.Id,
								  ServiceIdent = ins.ServiceIdent
							  }).Take(Limit).ToArray();
					return Task.FromResult(re);
				}
				else
				{
					var re = (from ins in ctx.Queryable<DataServiceInstance>()
							  where ins.ContainerId == null && ins.ServiceType == ServiceType
							  orderby ins.ItemOrder
							  select new ServiceReference
							  {
								  Id = ins.Id,
								  ServiceIdent = ins.ServiceIdent
							  }).Take(Limit).ToArray();
					return Task.FromResult(re);
				}
			})).Result;
			
		}
	}

}
