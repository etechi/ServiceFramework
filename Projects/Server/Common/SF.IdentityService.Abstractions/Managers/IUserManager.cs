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
using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities;
using SF.Sys.NetworkService;

namespace SF.Auth.IdentityServices.Managers
{
	public class UserQueryArgument : ObjectQueryArgument<ObjectKey<long>>
	{
		/// <summary>
		/// 注册账号
		/// </summary>
		public string MainCredential { get; set; }

		/// <summary>
		/// 注册类型
		/// </summary>
		[EntityIdent(typeof(ClaimType))]
		public string MainClaimTypeId { get; set; }

		/// <summary>
		/// 姓名
		/// </summary>
		[StringContains]
		public override string Name { get; set; }
	}

	/// <summary>
	/// 用户管理
	/// </summary>
	/// <typeparam name="TInternal"></typeparam>
	/// <typeparam name="TEditable"></typeparam>
	/// <typeparam name="TQueryArgument"></typeparam>
	[EntityManager]
	[Authorize("admin")]
	[NetworkService]
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

