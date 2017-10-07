using System;
using SF.Entities;
using SF.Metadata;
using SF.Auth;

namespace SF.Core.ServiceManagement.Management
{
	public class ServiceDeclarationQueryArgument : IQueryArgument<ObjectKey<string>>
	{
		[Comment("ID")]
		public ObjectKey<string> Id { get; set; }

		[Comment("服务定义名称")]
		public string Name { get; set; }

		[Comment("服务定义分类")]
		public string Group { get; set; }
	}

	[EntityManager]
	[Authorize("sysadmin")]
	[NetworkService]
	[Comment("服务定义管理", "定义系统内置服务")]
	[Category("系统管理", "系统服务管理")]
	public interface IServiceDeclarationManager:
		IEntitySource<string,Models.ServiceDeclaration, ServiceDeclarationQueryArgument>
	{

	}
}
