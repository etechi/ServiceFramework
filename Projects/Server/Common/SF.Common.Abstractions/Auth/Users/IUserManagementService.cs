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

using SF.Auth;
using SF.Auth.Identities;
using SF.Auth.Identities.Models;
using SF.Entities;
using SF.Metadata;
using SF.Users.Members.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SF.Auth.Users
{
	public class UserQueryArgument : Entities.IQueryArgument<ObjectKey<long>>
	{
		[Comment("Id")]
		public ObjectKey<long> Id { get; set; }

		[Comment("名称")]
		public string Name { get; set; }

		[Comment("账户名")]
		public string AccountName { get; set; }

		//[Comment("来源")]
		//[EntityIdent(typeof(MemberSources.IMemberSourceManagementService))]
		//public long? MemberSourceId { get; set; }

		//[Comment("邀请人")]
		//[EntityIdent(typeof(IMemberManagementService))]
		//public long? InvitorId { get; set; }

	}

	public interface IUserManagementService<TUserInternal, TUserEditable,TUserQueryArgument> : 
		Entities.IEntitySource<ObjectKey<long>,TUserInternal, TUserQueryArgument>,
		Entities.IEntityManager<ObjectKey<long>, TUserEditable>
		where TUserInternal:Models.UserInternal
		where TUserEditable : Models.UserEditable
		where TUserQueryArgument:UserQueryArgument
	{
		Task<string> CreateUserAsync(
			CreateIdentityArgument Arg
			);
	}

}

