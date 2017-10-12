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

		[Comment("服务实例名称")]
		public string Name { get; set; }


		[EntityIdent(typeof(Models.ServiceDeclaration))]
		[Comment("服务定义")]
		public string ServiceId { get; set; }


		[Comment("服务类型")]
		public string ServiceType { get; set; }

		[EntityIdent(typeof(Models.ServiceImplement))]
		[Comment("服务实现")]
		public string ImplementId { get; set; }

		[Comment("服务类型类型")]
		public string ImplementType { get; set; }

		[EntityIdent(typeof(Models.ServiceInstance))]
		[Comment("父服务实现")]
		public long? ContainerId { get; set; }

		[Comment("服务标识")]
		[MaxLength(100)]
		public string ServiceIdent { get; set; }

		[Comment("是否为默认服务实例")]
		public bool? IsDefaultService { get; set; }
	}

	[EntityManager]
	[Authorize("sysadmin")]
	[NetworkService]
	[Comment("服务实例管理", "系统内置服务实例")]
	[Category("系统管理","系统服务管理")]
	public interface IServiceInstanceManager :
		IEntityManager<ObjectKey<long>, Models.ServiceInstanceEditable>,
		IEntitySource<ObjectKey<long>, Models.ServiceInstanceInternal, ServiceInstanceQueryArgument>
	{

	}
}
