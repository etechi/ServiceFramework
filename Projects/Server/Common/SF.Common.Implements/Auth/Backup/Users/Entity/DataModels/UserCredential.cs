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

namespace SF.Auth.Users.Entity.DataModels
{

	[Table("SysAuthIdentityCredential")]
	public class UserCredential<TUser, TUserCredential, TUserClaimValue, TUserRole>
		where TUser : User<TUser, TUserCredential, TUserClaimValue, TUserRole>
		where TUserCredential : UserCredential<TUser, TUserCredential, TUserClaimValue, TUserRole>
		where TUserClaimValue : UserClaimValue<TUser, TUserCredential, TUserClaimValue, TUserRole>
		where TUserRole : UserRole<TUser, TUserCredential, TUserClaimValue, TUserRole>
	{

		[Key]
		[Column(Order = 1)]
		[Comment("分区ID")]
		[Index("union", Order = 1)]
		public long ScopeId { get; set; }

		[Key]
		[Column(Order =2)]
		[Index("union",Order=2)]
		public long ProviderId { get; set; }

		[Key]
		[Column(Order = 3)]
		[MaxLength(100)]
		public string Credential { get; set; }


		[Index]
		[Comment("标识ID")]
		public long UserId { get; set; }

		[Index("union", Order = 3)]
		[MaxLength(100)]
		public string UnionIdent { get; set; }

		public DateTime CreatedTime { get; set; }
		public DateTime? ConfirmedTime { get; set; }

		[ForeignKey(nameof(UserId))]
		public TUser User { get; set; }

	}
	public class UserCredential : UserCredential<User, UserCredential,UserClaimValue,UserRole>
	{ }
}
