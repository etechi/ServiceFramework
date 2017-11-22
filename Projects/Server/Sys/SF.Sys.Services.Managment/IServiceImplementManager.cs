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
using System;
using SF.Sys.Entities.Annotations;
using SF.Sys.Services.Management.Models;
using SF.Sys.Auth;
using SF.Sys.NetworkService;
using SF.Sys.Annotations;

namespace SF.Sys.Services.Management
{
	public class ServiceImplementQueryArgument : IQueryArgument<ObjectKey<string>>
	{
		/// <summary>
		/// ID
		/// </summary>
		public ObjectKey<string> Id { get; set; }

		/// <summary>
		/// 服务实现名称
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 服务实现分类
		/// </summary>
		public string Group { get; set; }

		/// <summary>
		/// 服务定义
		/// </summary>
		[EntityIdent(typeof(ServiceDeclaration))]
		public string DeclarationId { get; set; }
	}

	///<title>服务实现管理</title>
	/// <summary>
	/// 系统内置服务实现
	/// </summary>
	[EntityManager]
	[Authorize("sysadmin")]
	[NetworkService]
	[Category("系统管理", "系统服务管理")]
	public interface IServiceImplementManager :
		IEntitySource<ObjectKey<string>, Models.ServiceImplement, ServiceImplementQueryArgument>
	{

	}
}
