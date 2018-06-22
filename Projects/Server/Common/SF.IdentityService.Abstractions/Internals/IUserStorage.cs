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
using SF.Sys.Auth;
using SF.Sys.Clients;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SF.Auth.IdentityServices.Internals
{
	public class UserCreateArgument
	{
		/// <summary>
		/// 用户信息
		/// </summary>
		[Required]
		public User User { get; set; }

		/// <summary>
		/// 密码哈希
		/// </summary>
		[Required]
		public string PasswordHash { get; set; }

		/// <summary>
		/// 安全戳
		/// </summary>
		[Required]
		public byte[] SecurityStamp { get; set; }

		/// <summary>
		/// 访问源
		/// </summary>
		[Required]
		public IUserAgent UserAgent { get; set; }

		/// <summary>
		/// 登录凭证
		/// </summary>
		[Required]
		[MaxLength(100)]
		public string CredentialValue { get; set; }

		/// <summary>
		/// 登录凭证提供者
		/// </summary>
		public string ClaimTypeId { get; set; }

		/// <summary>
		/// 用户角色
		/// </summary>
		public string[] Roles { get; set; }

		/// <summary>
		/// 客户端ID
		/// </summary>
		public string ClientId { get; set; }

		/// <summary>
		/// 用户申明
		/// </summary>
		public ClaimValue[] ClaimValues { get; set; }

		/// <summary>
		/// 注册附加参数
		/// </summary>
		[MaxLength(200)]
		public Dictionary<string,string> ExtraArgument { get; set; }
		
	}

	/// <summary>
	/// 用户身份数据
	/// </summary>
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

