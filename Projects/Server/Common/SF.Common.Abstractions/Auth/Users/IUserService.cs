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

using SF.Metadata;
using SF.Auth;
using SF.Auth.Identities;
using SF.Users.Members.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Data.Models;
using SF.Core.Events;

namespace SF.Auth.Users
{
	public class UserRegisted  : IEvent
	{
		public long ServiceId { get; set; }
		public long UserId { get; set; }
		public DateTime Time { get; set; }

		long? IEvent.IdentityId => UserId;
		long? IEvent.ServiceId => ServiceId;
	}
	
	public interface IUserService<TCreateUserArgument,TUserDesc>
		where TCreateUserArgument: CreateUserArgument
		where TUserDesc:Models.UserDesc
	{
		Task<string> Signup(TCreateUserArgument Arg);
		Task<TUserDesc> GetCurrentUser();
	}

}

