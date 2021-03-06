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

using SF.Sys.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Auth.IdentityServices.DataModels
{
	public class DataUserRole: DataUserRole<DataUser, DataUserCredential, DataUserClaimValue, DataUserRole>
	{

	}
	[Table("UserRole")]
	public class DataUserRole<TUser, TUserCredential, TUserClaimValue, TUserRole>
		where TUser : DataUser<TUser, TUserCredential, TUserClaimValue, TUserRole>
		where TUserCredential : DataUserCredential<TUser, TUserCredential, TUserClaimValue, TUserRole>
		where TUserClaimValue : DataUserClaimValue<TUser, TUserCredential, TUserClaimValue, TUserRole>
		where TUserRole : DataUserRole<TUser, TUserCredential, TUserClaimValue, TUserRole>
	{
		[Key]
		[Column(Order =1)]
		public long UserId { get; set; }

		[ForeignKey(nameof(UserId))]
		public TUser User { get; set; }

		/// <summary>
		/// 角色
		/// </summary>
		[Key]
		[Column(Order = 1)]
		[Index]
		public string RoleId { get; set; }

		[ForeignKey(nameof(RoleId))]
		public DataRole Role { get; set; }
	}
}
