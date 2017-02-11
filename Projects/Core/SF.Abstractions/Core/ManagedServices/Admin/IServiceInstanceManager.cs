using SF.Auth;
using SF.Data.Entity;
using SF.Metadata;

namespace SF.Core.ManagedServices.Admin
{
	public class ServiceInstanceQueryArgument : IQueryArgument<string>
	{
		[Comment("ID")]
		public Option<string> Id { get; set; }

		[Comment("服务实例名称")]
		public string Name { get; set; }


		[EntityIdent("系统服务定义")]
		[Comment("服务定义")]
		public string DeclarationId { get; set; }

		[EntityIdent("系统服务实现")]
		[Comment("服务实现")]
		public string ImplementId { get; set; }
	}

	[EntityManager("系统服务实例")]
	[Authorize("sysadmin")]
	[NetworkService]
	[Comment("系统服务实例", "系统内置服务实例", GroupName = "系统服务管理")]
	public interface IServiceInstanceManager :
		IEntityManager<string, Models.ServiceInstanceEditable>,
		IEntitySource<string, Models.ServiceInstance, ServiceInstanceQueryArgument>
	{

	}
}
