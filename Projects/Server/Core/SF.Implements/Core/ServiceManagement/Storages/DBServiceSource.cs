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

using Newtonsoft.Json;
using SF.Core.ManagedServices.Runtime;
using SF.Core.ServiceManagement.Internals;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using System.Data.Common;
using Dapper;
namespace SF.Core.ServiceManagement.Storages
{
	public class DBServiceSource : IServiceConfigLoader, IServiceInstanceLister
	{
		DbConnection Connection { get; }
		IServiceMetadata Metadata { get; }
		public DBServiceSource(DbConnection Connection, IServiceMetadata Metadata)
		{
			this.Connection = Connection;
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
			var re = Connection.Query<Config>(
				"select Id,ServiceType,ImplementType,Setting,ContainerId " +
				"from SysServiceInstance " +
				"where Id=@Id", 
				new { Id = Id }
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
			return re;
		}

		ServiceReference[] IServiceInstanceLister.List(long? ScopeId, string ServiceType, int Limit)
		{
			if (ScopeId.HasValue)
			{
				var re = Connection.Query<ServiceReference>(
				"select top " + Limit + " Id,ServiceIdent " +
				"from SysServiceInstance " +
				"where ContainerId=@ContainerId and ServiceType=@ServiceType " +
				"order by ItemOrder", new { ContainerId = ScopeId, ServiceType = ServiceType }
				).ToArray();
				return re;
			}
			else
			{
				var re = Connection.Query<ServiceReference>(
				"select top " + Limit + " Id,ServiceIdent " +
				"from SysServiceInstance " +
				"where ContainerId is null and ServiceType=@ServiceType " +
				"order by ItemOrder", new { ServiceType = ServiceType }
				).ToArray();
				return re;
			}
		}
	}

}
