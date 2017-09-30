using SF.Auth;
using SF.Entities;
using SF.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SF.Core.ServiceManagement.Management
{
	public class ServiceInstanceQueryArgument : IQueryArgument<ObjectKey<long>>
	{
		[Comment("ID")]
		public ObjectKey<long> Id { get; set; }

		[Comment("����ʵ������")]
		public string Name { get; set; }


		[EntityIdent(typeof(Models.ServiceDeclaration))]
		[Comment("������")]
		public string ServiceType { get; set; }

		[EntityIdent(typeof(Models.ServiceImplement))]
		[Comment("����ʵ��")]
		public string ImplementId { get; set; }

		[EntityIdent(typeof(Models.ServiceInstance))]
		[Comment("������ʵ��")]
		public long? ContainerId { get; set; }

		[Comment("�����ʶ")]
		[MaxLength(100)]
		public string ServiceIdent { get; set; }

		[Comment("�Ƿ�ΪĬ�Ϸ���ʵ��")]
		public bool? IsDefaultService { get; set; }
	}

	[EntityManager("ϵͳ����ʵ��")]
	[Authorize("sysadmin")]
	[NetworkService]
	[Comment("����ʵ������", "ϵͳ���÷���ʵ��")]
	[Category("ϵͳ����","ϵͳ�������")]
	public interface IServiceInstanceManager :
		IEntityManager<ObjectKey<long>, Models.ServiceInstanceEditable>,
		IEntitySource<ObjectKey<long>, Models.ServiceInstanceInternal, ServiceInstanceQueryArgument>
	{

	}
}
