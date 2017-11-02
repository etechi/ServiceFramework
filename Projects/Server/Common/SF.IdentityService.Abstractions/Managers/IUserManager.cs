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

using SF.Auth.IdentityServices.Models;
using SF.Entities;
using SF.Metadata;
using System;
using System.Threading.Tasks;

namespace SF.Auth.IdentityServices.Managers
{
	public class UserQueryArgument : QueryArgument<long>
	{	
		[Comment("注册账号")]
		public string MainCredential { get; set; }

		[Comment("注册类型")]
		public string MainClaimTypeId { get; set; }

		[Comment("姓名")]
		[StringContains]
		public string Name { get; set; }
	}

	[EntityManager]
	[Authorize("admin")]
	[NetworkService]
	[Comment("用户管理")]
	public interface IUserManager<TInternal,TEditable,TQueryArgument> :
		IEntityManager<ObjectKey<long>,TEditable>,
		IEntitySource<ObjectKey<long>, TInternal, TQueryArgument>,
		Internals.IUserStorage
		where TInternal:Models.UserInternal
		where TEditable : Models.UserEditable
		where TQueryArgument : UserQueryArgument
	{
	}
	public interface IUserManager: IUserManager<UserInternal, UserEditable, UserQueryArgument>
	{
	}
}
