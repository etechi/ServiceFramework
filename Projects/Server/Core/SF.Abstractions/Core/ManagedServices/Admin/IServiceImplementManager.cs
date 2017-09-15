using SF.Auth;
using SF.Data.Entity;
using SF.Metadata;

namespace SF.Core.ManagedServices.Admin
{
	public class ServiceImplementQueryArgument : IQueryArgument<string>
	{
		[Comment("ID")]
		public Option<string> Id { get; set; }

		[Comment("服务实现名称")]
		public string Name { get; set; }

		[EntityIdent("系统服务实现分类")]
		[Comment("服务实现分类")]
		public string CategoryId { get; set; }

		[EntityIdent("系统服务定义")]
		[Comment("服务定义")]
		public string DeclarationId { get; set; }
	}

	[EntityManager("系统服务实现")]
	[Authorize("sysadmin")]
	[NetworkService]
	[Comment("系统服务实现", "系统内置服务实现", GroupName = "系统服务管理")]
	public interface IServiceImplementManager :
		IEntitySource<string, Models.ServiceImplement, ServiceImplementQueryArgument>
	{

	}
}
