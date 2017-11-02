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
	public class UserInternal : ObjectEntityBase
	{
		[Comment("创建标识")]
		public string MainCredential  { get; set; }

		[Comment("创建标识提供者")]
		public string MainClaimTypeId { get; set; }
	}

	public class UserEditable : UserInternal
	{
		[Comment("图标")]
		[MaxLength(100)]
		public string Icon { get; set; }

		[Comment("头像")]
		[MaxLength(100)]
		public string Image { get; set; }


		[SkipWhenDefault]
		public string PasswordHash { get; set; }

		[SkipWhenDefault]
		public string SecurityStamp { get; set; }
		public IEnumerable<UserCredential> Credentials { get; set; }

		[Comment("注册附加参数")]
		public Dictionary<string,string> SignupExtraArgument { get; set; }

		[Ignore]
		public IEnumerable<ClaimValue> Claims { get; set; }

		[Ignore]
		public IEnumerable<UserRole> Roles { get; set; }

		public long? SignupClientId { get; set; }
	}
}
