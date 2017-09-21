using System;
using SF.Entities;
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

		[Comment("���������")]
		public string Group { get; set; }
	}

	[EntityManager("ϵͳ������")]
	[Authorize("sysadmin")]
	[NetworkService]
	[Comment("���������", "����ϵͳ���÷���")]
	[Category("ϵͳ����", "ϵͳ�������")]
	public interface IServiceDeclarationManager:
		IEntitySource<string,Models.ServiceDeclaration, ServiceDeclarationQueryArgument>
	{

	}
}
