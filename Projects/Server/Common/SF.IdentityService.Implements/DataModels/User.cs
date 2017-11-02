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

using SF.Data;
using SF.Entities;
using SF.Entities.AutoEntityProvider;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Auth.IdentityServices.DataModels
{
	[Table(nameof(User))]
	public class User<TUser,TUserCredential, TClaimValue, TUserRole> :
		SF.Entities.DataModels.ObjectEntityBase
		where TUser: User<TUser, TUserCredential, TClaimValue, TUserRole>
		where TUserCredential : UserCredential<TUser, TUserCredential, TClaimValue, TUserRole>
		where TClaimValue : UserClaimValue<TUser, TUserCredential, TClaimValue, TUserRole>
		where TUserRole : UserRole<TUser, TUserCredential, TClaimValue, TUserRole>
	{

		[MaxLength(30)]
		[Comment("电话")]
		public virtual string PhoneNumber { get; set; }

		[MaxLength(100)]
		[Comment("密码哈希")]
		[Required]
		public virtual string PasswordHash { get; set; }

		[Comment("图标")]
		[MaxLength(100)]
		public virtual string Icon { get; set; }

		[Comment("头像")]
		[MaxLength(100)]
		public virtual string Image { get; set; }

		[MaxLength(100)]
		[Required]
		[Comment("安全标识")]
		public virtual string SecurityStamp { get; set; }

		[Index(Name="MainCredential",Order = 1,IsUnique =true)]
		[Comment("注册标识类型")]
		[Required]
		[MaxLength(100)]
		public virtual string MainClaimTypeId { get; set; }

		[MaxLength(200)]
		[Index(Name = "MainCredential", Order = 2, IsUnique = true)]
		[Comment("注册标识值")]
		[Required]
		public virtual string MainCredential { get; set; }

		[MaxLength(200)]
		[Comment("注册附加参数")]
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
