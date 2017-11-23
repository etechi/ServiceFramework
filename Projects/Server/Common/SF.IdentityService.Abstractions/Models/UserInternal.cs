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

using SF.Sys.Annotations;
using SF.Sys.Entities.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SF.Auth.IdentityServices.Models
{
	[EntityObject]
	public class UserInternal : ObjectEntityBase
	{
		/// <summary>
		/// 创建标识
		/// </summary>
		[Required]
		[MaxLength(100)]
		[TableVisible]
		public string MainCredential  { get; set; }

		/// <summary>
		/// 创建标识类型
		/// </summary>
		[MaxLength(100)]
		[Required]
		[EntityIdent(typeof(ClaimType),nameof(MainClaimTypeName))]
		public string MainClaimTypeId { get; set; }

		/// <summary>
		/// 创建标识类型
		/// </summary>
		[MaxLength(100)]
		[Ignore]
		public string MainClaimTypeName { get; set; }
	}

	public class UserEditable : UserInternal
	{
		/// <summary>
		/// 图标
		/// </summary>
		[MaxLength(100)]
		[Image]
		public string Icon { get; set; }

		/// <summary>
		/// 头像
		/// </summary>
		[MaxLength(100)]
		[Image]
		public string Image { get; set; }

		/// <summary>
		/// 密码
		/// </summary>
		[SkipWhenDefault]
		[MaxLength(100)]
		public string PasswordHash { get; set; }

		/// <summary>
		/// 安全标识
		/// </summary>
		[SkipWhenDefault]
		[Ignore]
		[MaxLength(100)]
		public string SecurityStamp { get; set; }

		/// <summary>
		/// 登录凭证
		/// </summary>
		[TableRows]
		public IEnumerable<UserCredential> Credentials { get; set; }

		/// <summary>
		/// 附加参数
		/// </summary>
		public Dictionary<string,string> SignupExtraArgument { get; set; }

		/// <summary>
		/// 申明
		/// </summary>
		[Ignore]
		[TableRows]
		public IEnumerable<ClaimValue> Claims { get; set; }

		/// <summary>
		/// 角色
		/// </summary>
		[Ignore]
		[TableRows]
		public IEnumerable<UserRole> Roles { get; set; }

		/// <summary>
		/// 客户端
		/// </summary>
		[EntityIdent(typeof(ClientInternal),nameof(SignupClientName))]
		public long? SignupClientId { get; set; }

		/// <summary>
		/// 客户端
		/// </summary>
		[Ignore]
		[TableVisible]
		public string SignupClientName { get; set; }
	}
}

