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
using SF.Entities;
using SF.Entities.AutoEntityProvider;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace SF.Auth.IdentityServices.Models
{
	[EntityObject]
	public class UserInternal : ObjectEntityBase
	{
		[Comment("创建标识")]
		[Required]
		[MaxLength(100)]
		[TableVisible]
		public string MainCredential  { get; set; }

		[Comment("创建标识类型")]
		[MaxLength(100)]
		[Required]
		[EntityIdent(typeof(ClaimType),nameof(MainClaimTypeName))]
		public string MainClaimTypeId { get; set; }

		[MaxLength(100)]
		[Ignore]
		[Comment("创建标识类型")]
		public string MainClaimTypeName { get; set; }
	}

	public class UserEditable : UserInternal
	{
		[Comment("图标")]
		[MaxLength(100)]
		[Image]
		public string Icon { get; set; }

		[Comment("头像")]
		[MaxLength(100)]
		[Image]
		public string Image { get; set; }

		[SkipWhenDefault]
		[Comment("重置密码")]
		[MaxLength(100)]
		public string PasswordHash { get; set; }

		[SkipWhenDefault]
		[Comment("安全标识")]
		[Ignore]
		[MaxLength(100)]
		public string SecurityStamp { get; set; }

		[Comment("登录凭证")]
		[TableRows]
		public IEnumerable<UserCredential> Credentials { get; set; }

		[Comment("附加参数")]
		public Dictionary<string,string> SignupExtraArgument { get; set; }

		[Ignore]
		[Comment("申明")]
		[TableRows]
		public IEnumerable<ClaimValue> Claims { get; set; }

		[Ignore]
		[Comment("角色")]
		[TableRows]
		public IEnumerable<UserRole> Roles { get; set; }

		[Comment("客户端")]
		[EntityIdent(typeof(ClientInternal),nameof(SignupClientName))]
		public long? SignupClientId { get; set; }

		[Comment("客户端")]
		[Ignore]
		[TableVisible]
		public string SignupClientName { get; set; }
	}
}

