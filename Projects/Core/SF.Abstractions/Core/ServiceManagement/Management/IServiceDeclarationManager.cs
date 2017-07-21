using System;
using SF.Data.Entity;
using SF.Metadata;
using SF.Auth;

namespace SF.Core.ServiceManagement.Management
{
	public class ServiceDeclarationQueryArgument : IQueryArgument<string>
	{
		[Comment("ID")]
		public Option<string> Id { get; set; }

		[Comment("服务定义名称")]
		public string Name { get; set; }

		[EntityIdent("系统服务定义分类")]
		[Comment("服务定义分类")]
		public string CategoryId { get; set; }
	}

	[EntityManager("系统服务定义")]
	[Authorize("sysadmin")]
	[NetworkService]
	[Comment("系统服务定义", "定义系统内置服务",GroupName ="系统服务管理")]
	public interface IServiceDeclarationManager:
		IEntitySource<string,Models.ServiceDeclaration, ServiceDeclarationQueryArgument>
	{

	}
}
