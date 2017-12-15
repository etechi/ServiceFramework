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
using SF.Sys.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Auth.IdentityServices.DataModels
{
	[Table(nameof(User))]
	public class User<TUser,TUserCredential, TClaimValue, TUserRole> :
		SF.Sys.Entities.DataModels.ObjectEntityBase
		where TUser: User<TUser, TUserCredential, TClaimValue, TUserRole>
		where TUserCredential : UserCredential<TUser, TUserCredential, TClaimValue, TUserRole>
		where TClaimValue : UserClaimValue<TUser, TUserCredential, TClaimValue, TUserRole>
		where TUserRole : UserRole<TUser, TUserCredential, TClaimValue, TUserRole>
	{

		/// <summary>
		/// 电话
		/// </summary>
		[MaxLength(30)]
		public virtual string PhoneNumber { get; set; }
		///<title>密码哈希</title>
		/// <summary>
		/// 可以为空，为空时，不能通过密码验证登录
		/// </summary>
		[MaxLength(100)]
		public virtual string PasswordHash { get; set; }

		/// <summary>
		/// 图标
		/// </summary>
		[MaxLength(100)]
		public virtual string Icon { get; set; }

		/// <summary>
		/// 头像
		/// </summary>
		[MaxLength(100)]
		public virtual string Image { get; set; }

		/// <summary>
		/// 安全标识
		/// </summary>
		[MaxLength(100)]
		[Required]
		public virtual string SecurityStamp { get; set; }

		/// <summary>
		/// 注册标识类型
		/// </summary>
		[Index(Name="MainCredential",Order = 1,IsUnique =true)]
		[Required]
		[MaxLength(100)]
		public virtual string MainClaimTypeId { get; set; }

		/// <summary>
		/// 注册标识值
		/// </summary>
		[MaxLength(200)]
		[Index(Name = "MainCredential", Order = 2, IsUnique = true)]
		[Required]
		public virtual string MainCredential { get; set; }

		/// <summary>
		/// 注册附加参数
		/// </summary>
		[MaxLength(200)]
		[JsonData(typeof(Dictionary<string,string>))]
		public virtual string SignupExtraArgument { get; set; }


		[InverseProperty(nameof(UserCredential<TUser,TUserCredential, TClaimValue, TUserRole>.User))]
		public ICollection<TUserCredential> Credentials { get; set; }

		[InverseProperty(nameof(UserClaimValue<TUser, TUserCredential, TClaimValue, TUserRole>.User))]
		public ICollection<TClaimValue> ClaimValues { get; set; }

		[InverseProperty(nameof(UserRole<TUser, TUserCredential, TClaimValue, TUserRole>.User))]
		public ICollection<TUserRole> Roles { get; set; }

		[Index]
		[MaxLength(100)]
		public string SignupClientId { get; set; }

		[ForeignKey(nameof(SignupClientId))]
		public Client SignupClient { get; set; }

	}
	public class User : User<User, UserCredential, UserClaimValue, UserRole>
	{ }
}
