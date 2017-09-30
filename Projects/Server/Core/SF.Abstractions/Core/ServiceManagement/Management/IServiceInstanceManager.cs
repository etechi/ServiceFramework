using SF.Auth;
using SF.Entities;
using SF.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SF.Core.ServiceManagement.Management
{
	public class ServiceInstanceQueryArgument : IQueryArgument<ObjectKey<long>>
	{
		[Comment("ID")]
		public ObjectKey<long> Id { get; set; }

		[Comment("服务实例名称")]
		public string Name { get; set; }


		[EntityIdent(typeof(Models.ServiceDeclaration))]
		[Comment("服务定义")]
		public string ServiceType { get; set; }

		[EntityIdent(typeof(Models.ServiceImplement))]
		[Comment("服务实现")]
		public string ImplementId { get; set; }

		[EntityIdent(typeof(Models.ServiceInstance))]
		[Comment("父服务实现")]
		public long? ContainerId { get; set; }

		[Comment("服务标识")]
		[MaxLength(100)]
		public string ServiceIdent { get; set; }

		[Comment("是否为默认服务实例")]
		public bool? IsDefaultService { get; set; }
	}

	[EntityManager("系统服务实例")]
	[Authorize("sysadmin")]
	[NetworkService]
	[Comment("服务实例管理", "系统内置服务实例")]
	[Category("系统管理","系统服务管理")]
	public interface IServiceInstanceManager :
		IEntityManager<ObjectKey<long>, Models.ServiceInstanceEditable>,
		IEntitySource<ObjectKey<long>, Models.ServiceInstanceInternal, ServiceInstanceQueryArgument>
	{

	}
}
