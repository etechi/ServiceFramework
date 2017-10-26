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

namespace SF.Auth.Users.Models
{
	public class UserInternal : User
	{
		[Comment("创建标识")]
		public string CreateCredential  { get; set; }

		[Comment("创建标识提供者")]
		public long CreateCredentialProviderId { get; set; }

		[Comment("创建时间")]
		public DateTime CreateTime { get; set; }
		[Comment("更新时间")]
		public DateTime UpdateTime { get; set; }

		[Comment("对象状态")]
		public EntityLogicState LogicState { get; set; }
	}
	[Comment("凭证类型")]
	public class ClaimType
	{
		[Comment("Id")]
		public long Id { get; set; }

		[Comment("类型名称")]
		[Required]
		[MaxLength(100)]
		public string Name { get; set; }
	}

	
	[Comment("凭证参数值")]
	public class ClaimValue
	{
		[Comment("Id")]
		public long Id { get; set; }

		[Comment("类型ID")]
		[EntityIdent(typeof(ClaimType), nameof(TypeName))]
		public long TypeId { get; set; }

		[Comment("类型")]
		[Ignore]
		public string TypeName { get; set; }


		[Comment("凭证值")]
		public string Value { get; set; }

		[Comment("发行时间")]
		public DateTime IssueTime { get; set; }
	}

	[Comment("角色")]
	public class Role: ObjectEntityBase<long>
	{

	}
	public class RoleEditable : Role
	{
		public IEnumerable<ClaimValue> Claims { get; set; }
	}
	public class IdentityRole
	{
		[Comment("Id")]
		public long Id { get; set; }

		[Comment("类型ID")]
		[EntityIdent(typeof(Role), nameof(RoleName))]
		public long RoleId { get; set; }

		[Comment("类型")]
		[Ignore]
		public string RoleName { get; set; }


	}
	public class UserEditable : UserInternal
	{
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
		public IEnumerable<IdentityRole> Roles { get; set; }
	}
}

