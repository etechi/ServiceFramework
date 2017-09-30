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

		[Comment("����ʵ������")]
		public string Name { get; set; }

		[Comment("����ʵ�ַ���")]
		public string Group { get; set; }

		[EntityIdent(typeof(ServiceDeclaration))]
		[Comment("������")]
		public string DeclarationId { get; set; }
	}

	[EntityManager("ϵͳ����ʵ��")]
	[Authorize("sysadmin")]
	[NetworkService]
	[Comment("����ʵ�ֹ���", "ϵͳ���÷���ʵ��")]
	[Category("ϵͳ����", "ϵͳ�������")]
	public interface IServiceImplementManager :
		IEntitySource<string, Models.ServiceImplement, ServiceImplementQueryArgument>
	{

	}
}
