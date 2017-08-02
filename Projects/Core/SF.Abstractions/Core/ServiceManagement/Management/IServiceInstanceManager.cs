using SF.Auth;
using SF.Data.Entity;
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


		[EntityIdent("ϵͳ������")]
		[Comment("������")]
		public string ServiceType { get; set; }

		[EntityIdent("ϵͳ����ʵ��")]
		[Comment("����ʵ��")]
		public string ImplementId { get; set; }

		[EntityIdent("ϵͳ����ʵ��")]
		[Comment("������ʵ��")]
		public long? ParentId { get; set; }

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
		IEntityManager<long, Models.ServiceInstanceEditable>,
		IEntitySource<long, Models.ServiceInstanceInternal, ServiceInstanceQueryArgument>
	{

	}
}
