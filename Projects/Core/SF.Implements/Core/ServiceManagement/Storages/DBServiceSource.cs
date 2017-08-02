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
	[UnmanagedService]
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

			public long? ParentId { get; set; }

			public string ServiceType { get; set; }

			public string ImplementType { get; set; }

			public string Setting { get; set; }
			public string Name { get; set; }
		}
		public IServiceConfig GetConfig(long Id)
		{
			var re = Connection.Query<Config>(
				"select Id,ServiceType,ImplementType,Setting,ParentId " +
				"from SysServiceInstance " +
				"where Id=@Id", 
				new { Id = Id }
				).SingleOrDefault();
			if (re == null)
				return null;
			var svcDef = Metadata.ServicesByTypeName.Get(re.ServiceType);
			if (svcDef == null)
				throw new InvalidOperationException($"找不到定义的服务{re.ServiceType}");

			var (implType, _) = re.ImplementType.Split2('@');
			var svcImpl = svcDef.Implements.SingleOrDefault(i => i.ImplementType.GetFullName() == implType);
			if (svcImpl == null)
				throw new InvalidOperationException($"找不到服务实现{implType}");
			re.ImplementType = implType;
			return re;
		}

		ServiceReference[] IServiceInstanceLister.List(long? ScopeId, string ServiceType, int Limit)
		{
			if (ScopeId.HasValue)
			{
				var re = Connection.Query<ServiceReference>(
				"select top " + Limit + " Id,ServiceIdent " +
				"from SysServiceInstance " +
				"where ParentId=@ParentId and ServiceType=@ServiceType " +
				"order by Priority", new { ParentId = ScopeId, ServiceType = ServiceType }
				).ToArray();
				return re;
			}
			else
			{
				var re = Connection.Query<ServiceReference>(
				"select top " + Limit + " Id,ServiceIdent " +
				"from SysServiceInstance " +
				"where ParentId is null and ServiceType=@ServiceType " +
				"order by Priority", new { ServiceType = ServiceType }
				).ToArray();
				return re;
			}
		}
	}

}
