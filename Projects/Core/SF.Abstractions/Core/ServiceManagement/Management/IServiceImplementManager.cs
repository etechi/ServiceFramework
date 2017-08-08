using SF.Auth;
using SF.Data.Entity;
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

		[EntityIdent("ϵͳ����ʵ�ַ���")]
		[Comment("����ʵ�ַ���")]
		public string CategoryId { get; set; }

		[EntityIdent("ϵͳ������")]
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
