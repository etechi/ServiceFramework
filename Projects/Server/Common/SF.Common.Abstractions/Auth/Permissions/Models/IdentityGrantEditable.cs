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

using SF.Data.Models;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Auth.Permissions.Models
{

	[EntityObject]
	public class GrantEditable: ObjectEntityBase<long>
    {


		[EntityIdent(typeof(RoleInternal))]
		[Comment(Name = "角色", Description = "管理员的角色，重新登录后生效")]
		public IEnumerable<string> Roles { get; set; }


		[Comment(Name = "授权清单", Description = "注：1.为了方便管理，建议通过角色来设置权限， 2.新建管理员时需要保存以后才能设置权限， 2.权限修改后，下次用户登录时生效")]
		[TableRows]
		[ReadOnly(true)]
		[Ignore]
		public ResourceGrantInternal[] ResGrants { get; set; }


	}
}
