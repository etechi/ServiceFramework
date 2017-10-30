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
using SF.Metadata;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Auth.IdentityServices.DataModels
{
	public class UserClaimValue: 
		UserClaimValue<User, UserCredential, UserClaimValue,UserRole>
	{

	}

	[Table("SysAuthClaimValue")]
	public class UserClaimValue<TUser, TUserCredential, TUserClaimValue, TUserRole>
		where TUser : User<TUser, TUserCredential, TUserClaimValue, TUserRole>
		where TUserCredential : UserCredential<TUser, TUserCredential, TUserClaimValue, TUserRole>
		where TUserClaimValue : UserClaimValue<TUser, TUserCredential, TUserClaimValue, TUserRole>
		where TUserRole : UserRole<TUser, TUserCredential, TUserClaimValue, TUserRole>

	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[Key]
		[Comment("Id")]
		public long Id { get; set; }

		[Index]
		[Comment("身份标识ID")]
		public long UserId { get; set; }

		[Index]
		[Comment("类型ID")]
		public long TypeId { get; set; }

		[ForeignKey(nameof(TypeId))]
		public ClaimType Type { get; set; }


		[ForeignKey(nameof(UserId))]
		public TUser User { get; set; }

		[Index]
		public long? ScopeId { get; set; }

		[Comment("凭证值")]
		public string Value { get; set; }

		[Comment("创建时间")]
		public DateTime CreateTime { get; set; }

		[Comment("修改时间")]
		public DateTime UpdateTime { get; set; }
	}
}
