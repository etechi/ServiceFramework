using SF.Auth;
using SF.Entities;
using SF.Metadata;
using System;
namespace SF.Core.ServiceManagement.Management
{
	public class ServiceImplementQueryArgument : IQueryArgument<string>
	{
		[Comment("ID")]
		public Option<string> Id { get; set; }

		[Comment("����ʵ������")]
		public string Name { get; set; }

		[Comment("����ʵ�ַ���")]
		public string Group { get; set; }

		[EntityIdent(typeof(IServiceDeclarationManager))]
		[Comment("������")]
		public string DeclarationId { get; set; }
	}

	[EntityManager]
	[Authorize("sysadmin")]
	[NetworkService]
	[Comment("����ʵ�ֹ���", "ϵͳ���÷���ʵ��")]
	[Category("ϵͳ����", "ϵͳ�������")]
	public interface IServiceImplementManager :
		IEntitySource<string, Models.ServiceImplement, ServiceImplementQueryArgument>
	{

	}
}
