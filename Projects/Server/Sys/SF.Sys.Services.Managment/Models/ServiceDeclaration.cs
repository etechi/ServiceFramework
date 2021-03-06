﻿#region Apache License Version 2.0
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

using SF.Sys.Annotations;
using SF.Sys.Entities;
using SF.Sys.Entities.Annotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Sys.Services.Management.Models
{
	[EntityObject]
	public class ServiceDeclaration : 
		IEntityWithId<string>,
		IEntityWithName
	{
		/// <summary>
		/// ID
		/// </summary>
		[Key]
		[ReadOnly(true)]
		[TableVisible]
		public string Id { get; set; }

		/// <summary>
		/// 类型
		/// </summary>
		[ReadOnly(true)]
		[TableVisible]
		public string Type { get; set; }

		/// <summary>
		/// 名称
		/// </summary>
		[ReadOnly(true)]
		[TableVisible]
		public string Name { get; set; }

		/// <summary>
		/// 描述
		/// </summary>
		[ReadOnly(true)]
		[TableVisible]
		public string Description { get; set; }

		/// <summary>
		/// 分组
		/// </summary>
		[ReadOnly(true)]
		[TableVisible]
		public string Group { get; set; }

		/// <summary>
		/// 是否禁用
		/// </summary>
		[TableVisible]
		public bool Disabled { get; set; }
	}
}
