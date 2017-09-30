using SF.Auth;
using SF.Core.ServiceManagement.Models;
using SF.Entities;
using SF.Metadata;
using System;
namespace SF.Core.ServiceManagement.Management
{
	public class ServiceImplementQueryArgument : IQueryArgument<ObjectKey<string>>
	{
		[Comment("ID")]
		public ObjectKey<string> Id { get; set; }

		[Comment("服务实现名称")]
		public string Name { get; set; }

		[Comment("服务实现分类")]
		public string Group { get; set; }

		[EntityIdent(typeof(ServiceDeclaration))]
		[Comment("服务定义")]
		public string DeclarationId { get; set; }
	}

	[EntityManager("系统服务实现")]
	[Authorize("sysadmin")]
	[NetworkService]
	[Comment("服务实现管理", "系统内置服务实现")]
	[Category("系统管理", "系统服务管理")]
	public interface IServiceImplementManager :
		IEntitySource<string, Models.ServiceImplement, ServiceImplementQueryArgument>
	{

	}
}
