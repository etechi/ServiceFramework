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

using SF.Core.ServiceManagement.Management;
using SF.Entities;
using SF.Metadata;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Sys.Services.Management.Models
{
	[EntityObject]
	public class ServiceImplement: IEntityWithId<string>, IEntityWithName
	{
		[Key]
		[Comment("ID")]
		[ReadOnly(true)]
		[TableVisible]
		public string Id { get; set; }

		[Comment("名称")]
		[ReadOnly(true)]
		[TableVisible]
		public string Type { get; set; }

		[Comment("名称")]
		[ReadOnly(true)]
		[TableVisible]
		public string Name { get; set; }

		[Comment("描述")]
		[ReadOnly(true)]
		[TableVisible]
		public string Description { get; set; }

		[Comment("分组")]
		[ReadOnly(true)]
		[TableVisible]
		public string Group { get; set; }

		[Comment("是否禁用")]
		[TableVisible]
		public bool Disabled { get; set; }

		[Comment("服务定义")]
		[EntityIdent(typeof(ServiceDeclaration), nameof(DeclarationName))]
		[ReadOnly(true)]
		public string DeclarationId { get; set; }

		[Ignore]
		[TableVisible]
		[Comment("服务定义")]
		public string DeclarationName { get; set; }
	}
}
