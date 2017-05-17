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

		[Comment("����������")]
		public string Name { get; set; }

		[EntityIdent("ϵͳ���������")]
		[Comment("���������")]
		public string CategoryId { get; set; }
	}

	[EntityManager("ϵͳ������")]
	[Authorize("sysadmin")]
	[NetworkService]
	[Comment("ϵͳ������", "����ϵͳ���÷���",GroupName ="ϵͳ�������")]
	public interface IServiceDeclarationManager:
		IEntitySource<string,Models.ServiceDeclaration, ServiceDeclarationQueryArgument>
	{

	}
}
