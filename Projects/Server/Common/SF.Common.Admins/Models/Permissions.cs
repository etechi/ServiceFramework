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


using SF.Auth.IdentityServices.Models;
using SF.Sys.Annotations;
using SF.Sys.Entities;
using SF.Sys.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Common.Admins.Models
{
	
	public class ResourceGrantInternal : IEntityWithId<string>
	{
		/// <summary>
		/// Id
		/// </summary>
		[Layout(10)]
		[Key]
		[Ignore]
		public string Id { get; set; }

		/// <summary>
		/// 资源分组
		/// </summary>
		[ReadOnly(true)]
		[Layout(15)]
		public string Group { get; set; }

		/// <summary>
		/// 资源项目
		/// </summary>
		[ReadOnly(true)]
		[Layout(20)]
		public string Name { get; set; }

		/// <summary>
		/// 授权
		/// </summary>
		[Layout(25)]
		[EntityIdent(typeof(ResourceInternal), ScopeField = nameof(Id))]
		public string[] OperationIds { get; set; }

		/// <summary>
		/// 说明
		/// </summary>
		[ReadOnly(true)]
		[Layout(30)]
		[Ignore]
		public string Description { get; set; }

	}
}

