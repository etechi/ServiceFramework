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
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SF.Auth.IdentityServices.Internals
{
	public class UserCreateArgument
	{
		[Comment("用户信息")]
		[Required]
		public User User { get; set; }

		[Required]
		[Comment("密码哈希")]
		public string PasswordHash { get; set; }

		[Required]
		[Comment("安全戳")]
		public byte[] SecurityStamp { get; set; }

		[Required]
		[Comment("访问源")]
		public Clients.IUserAgent AccessSource { get; set; }

		[Required]
		[Comment("登录凭证")]
		[MaxLength(100)]
		public string CredentialValue { get; set; }

		[Comment("登录凭证提供者")]
		public string ClaimTypeId { get; set; }

		[Comment("用户角色")]
		public string[] Roles { get; set; }

		[Comment("用户申明")]
		public ClaimValue[] ClaimValues { get; set; }

		[Comment("注册附加参数")]
		[MaxLength(200)]
		public Dictionary<string,string> ExtraArgument { get; set; }
		
	}

	[Comment("用户身份数据")]
	public class UserData
	{
		public long Id { get; set; }
		public byte[] SecurityStamp { get; set; }
		public string PasswordHash { get; set; }
		public bool IsEnabled { get; set; }
		public string Name { get; set; }
		public string Icon { get; set; }
		public IEnumerable<ClaimValue> Claims { get; set; }
		public IEnumerable<string> Roles { get; set; }
	}
	
	public interface IUserStorage
	{
		Task<long> Create(UserCreateArgument Arg);
		Task<UserData> Load(long Id);
		Task UpdateDescription(User Identity);
		Task UpdateSecurity(long Id, string PasswordHash,byte[] SecurityStamp);
	}

}

