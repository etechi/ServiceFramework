#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

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
		public string ServiceId { get; set; }


		[Comment("��������")]
		public string ServiceType { get; set; }

		[EntityIdent(typeof(Models.ServiceImplement))]
		[Comment("����ʵ��")]
		public string ImplementId { get; set; }

		[Comment("������������")]
		public string ImplementType { get; set; }

		[EntityIdent(typeof(Models.ServiceInstance))]
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
		IEntityManager<ObjectKey<long>, Models.ServiceInstanceEditable>,
		IEntitySource<ObjectKey<long>, Models.ServiceInstanceInternal, ServiceInstanceQueryArgument>
	{

	}
}
