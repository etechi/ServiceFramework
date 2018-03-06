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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Common.Admins.Models
{
	public class AdminEditable : AdminInternal
	{
		/// <summary>
		/// 密码
		/// </summary>
		///<remarks>密码为空时，不修改旧密码, 密码不少于8位，注意密码强度</remarks>
		[MinLength(8)]
		[MaxLength(50)]
		public string Password { get; set; }

		/// <summary>
		/// 角色
		/// </summary>
		/// <remarks>管理员的角色，重新登录后生效</remarks>
		[EntityIdent(typeof(Role))]
		[TableVisible]
		public IEnumerable<string> Roles { get; set; }
	}
}

