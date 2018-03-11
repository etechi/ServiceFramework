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

using SF.Sys.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Auth.IdentityServices.DataModels
{
	public class DataUserClaimValue: 
		DataUserClaimValue<DataUser, DataUserCredential, DataUserClaimValue,DataUserRole>
	{

	}

	[Table("UserClaimValue")]
	public class DataUserClaimValue<TUser, TUserCredential, TUserClaimValue, TUserRole> :
		DataBaseClaimValue
		where TUser : DataUser<TUser, TUserCredential, TUserClaimValue, TUserRole>
		where TUserCredential : DataUserCredential<TUser, TUserCredential, TUserClaimValue, TUserRole>
		where TUserClaimValue : DataUserClaimValue<TUser, TUserCredential, TUserClaimValue, TUserRole>
		where TUserRole : DataUserRole<TUser, TUserCredential, TUserClaimValue, TUserRole>

	{

		/// <summary>
		/// 身份标识ID
		/// </summary>
		[Index]
		public long UserId { get; set; }

		[ForeignKey(nameof(UserId))]
		public TUser User { get; set; }
		
	}
}
