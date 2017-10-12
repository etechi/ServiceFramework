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

using SF.Entities;
using SF.Metadata;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Auth.Identities.Models
{
	[EntityObject]
	[Comment("身份标识")]
	public class Identity : IEntityWithId<long>
    {
		[Key]
		[ReadOnly(true)]
		[TableVisible]
		[Comment("ID")]
		public long Id { get; set; }

		[Comment("名称")]
		[MaxLength(100)]
		[Required]
		[TableVisible]
		public string Name { get; set; }

		[Comment("图标")]
		[MaxLength(100)]
		public string Icon { get; set; }

		[Comment("所属对象")]
		[MaxLength(100)]
		[Required]
		[EntityIdent]
		public string OwnerId { get; set; }

	}
}

