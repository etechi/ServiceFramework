using SF.Auth;
using SF.Data.Entity;
using SF.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SF.Core.ServiceManagement.Management
{
	public class ServiceInstanceQueryArgument : IQueryArgument<long>
	{
		[Comment("ID")]
		public Option<long> Id { get; set; }

		[Comment("服务实例名称")]
		public string Name { get; set; }


		[EntityIdent("系统服务定义")]
		[Comment("服务定义")]
		public string ServiceType { get; set; }

		[EntityIdent("系统服务实现")]
		[Comment("服务实现")]
		public string ImplementId { get; set; }

		[EntityIdent("系统服务实现")]
		[Comment("父服务实现")]
		public long? ParentId { get; set; }

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
		IEntityManager<long, Models.ServiceInstanceEditable>,
		IEntitySource<long, Models.ServiceInstanceInternal, ServiceInstanceQueryArgument>
	{

	}
}
