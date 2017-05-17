using SF.Auth;
using SF.Data.Entity;
using SF.Metadata;
using System;
namespace SF.Core.ServiceManagement.Management
{
	public class ServiceInstanceQueryArgument : IQueryArgument<string>
	{
		[Comment("ID")]
		public Option<string> Id { get; set; }

		[Comment("����ʵ������")]
		public string Name { get; set; }


		[EntityIdent("ϵͳ������")]
		[Comment("������")]
		public string DeclarationId { get; set; }

		[EntityIdent("ϵͳ����ʵ��")]
		[Comment("����ʵ��")]
		public string ImplementId { get; set; }

		[Comment("�Ƿ�ΪĬ�Ϸ���ʵ��")]
		public bool? IsDefaultService { get; set; }
	}

	[EntityManager("ϵͳ����ʵ��")]
	[Authorize("sysadmin")]
	[NetworkService]
	[Comment("ϵͳ����ʵ��", "ϵͳ���÷���ʵ��", GroupName = "ϵͳ�������")]
	public interface IServiceInstanceManager :
		IEntityManager<string, Models.ServiceInstanceEditable>,
		IEntitySource<string, Models.ServiceInstanceInternal, ServiceInstanceQueryArgument>
	{

	}
}
