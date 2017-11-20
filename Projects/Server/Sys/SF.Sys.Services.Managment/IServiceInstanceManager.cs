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

using SF.Sys.Entities;
using SF.Sys.Entities.Annotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace SF.Sys.Services.Management
{
	public class ServiceInstanceQueryArgument : IQueryArgument<ObjectKey<long>>
	{
		/// <summary>
		/// ID
		/// </summary>
		public ObjectKey<long> Id { get; set; }

		/// <summary>
		/// 服务实例名称
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 服务定义
		/// </summary>
		[EntityIdent(typeof(Models.ServiceDeclaration))]
		public string ServiceId { get; set; }


		/// <summary>
		/// 服务类型
		/// </summary>
		public string ServiceType { get; set; }

		/// <summary>
		/// 服务实现
		/// </summary>
		[EntityIdent(typeof(Models.ServiceImplement))]
		public string ImplementId { get; set; }

		/// <summary>
		/// 服务类型类型
		/// </summary>
		public string ImplementType { get; set; }

		/// <summary>
		/// 父服务实现
		/// </summary>
		[EntityIdent(typeof(Models.ServiceInstance))]
		public long? ContainerId { get; set; }

		/// <summary>
		/// 服务标识
		/// </summary>
		[MaxLength(100)]
		public string ServiceIdent { get; set; }

		/// <summary>
		/// 是否为默认服务实例
		/// </summary>
		public bool? IsDefaultService { get; set; }
	}

	///<title>服务实例管理</title>
	/// <summary>
	/// 系统内置服务实例
	/// </summary>
	[EntityManager]
	[Authorize("sysadmin")]
	[NetworkService]
	[Category("系统管理","系统服务管理")]
	public interface IServiceInstanceManager :
		IEntityManager<ObjectKey<long>, Models.ServiceInstanceEditable>,
		IEntitySource<ObjectKey<long>, Models.ServiceInstanceInternal, ServiceInstanceQueryArgument>
	{

	}
}
