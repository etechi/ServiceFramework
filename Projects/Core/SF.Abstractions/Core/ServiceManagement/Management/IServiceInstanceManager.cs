using SF.Auth;
using SF.Entities;
using SF.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SF.Core.ServiceManagement.Management
{
	public class ServiceInstanceQueryArgument : IQueryArgument<long>
	{
		[Comment("ID")]
		public Option<long> Id { get; set; }

		[Comment("����ʵ������")]
		public string Name { get; set; }


		[EntityIdent(typeof(IServiceDeclarationManager))]
		[Comment("������")]
		public string ServiceType { get; set; }

		[EntityIdent(typeof(IServiceImplementManager))]
		[Comment("����ʵ��")]
		public string ImplementId { get; set; }

		[EntityIdent(typeof(IServiceInstanceManager))]
		[Comment("������ʵ��")]
		public long? ContainerId { get; set; }

		[Comment("�����ʶ")]
		[MaxLength(100)]
		public string ServiceIdent { get; set; }

		[Comment("�Ƿ�ΪĬ�Ϸ���ʵ��")]
		public bool? IsDefaultService { get; set; }
	}

	[EntityManager]
	[Authorize("sysadmin")]
	[NetworkService]
	[Comment("����ʵ������", "ϵͳ���÷���ʵ��")]
	[Category("ϵͳ����","ϵͳ�������")]
	public interface IServiceInstanceManager :
		IEntityManager<long, Models.ServiceInstanceEditable>,
		IEntitySource<long, Models.ServiceInstanceInternal, ServiceInstanceQueryArgument>
	{

	}
}