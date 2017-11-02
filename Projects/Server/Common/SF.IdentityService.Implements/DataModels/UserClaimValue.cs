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

	[Table(nameof(UserClaimValue))]
	public class UserClaimValue<TUser, TUserCredential, TUserClaimValue, TUserRole> :
		BaseClaimValue
		where TUser : User<TUser, TUserCredential, TUserClaimValue, TUserRole>
		where TUserCredential : UserCredential<TUser, TUserCredential, TUserClaimValue, TUserRole>
		where TUserClaimValue : UserClaimValue<TUser, TUserCredential, TUserClaimValue, TUserRole>
		where TUserRole : UserRole<TUser, TUserCredential, TUserClaimValue, TUserRole>

	{

		[Index]
		[Comment("身份标识ID")]
		public long UserId { get; set; }

		[ForeignKey(nameof(UserId))]
		public TUser User { get; set; }
		
	}
}