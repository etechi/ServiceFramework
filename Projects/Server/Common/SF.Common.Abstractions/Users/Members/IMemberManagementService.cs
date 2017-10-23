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

namespace SF.Users.Members
{
	public class MemberQueryArgument :SF.Auth.Users.UserQueryArgument
	{

	}

	public class MemberRegisted : SF.Auth.Users.UserRegisted
	{
	}

	[EntityManager]
	[Authorize("admin")]
	[NetworkService]
	[Comment("会员")]
	[Category("用户管理", "会员管理")]
	public interface IMemberManagementService : 
		SF.Auth.Users.IUserManagementService<Models.MemberInternal,MemberEditable,MemberQueryArgument>
    {
	}

}

