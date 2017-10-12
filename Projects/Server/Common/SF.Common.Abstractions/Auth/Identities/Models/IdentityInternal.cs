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

using SF.Entities;
using SF.Metadata;
using System;
using System.Collections.Generic;

namespace SF.Auth.Identities.Models
{
	public class IdentityInternal : Identity
	{
		[Comment("创建标识")]
		public string CreateCredential  { get; set; }

		[Comment("创建标识提供者")]
		public long CreateCredentialProviderId { get; set; }

		[Comment("创建时间")]
		public DateTime CreateTime { get; set; }
		[Comment("更新时间")]
		public DateTime UpdateTime { get; set; }

		[Comment("对象状态")]
		public EntityLogicState LogicState { get; set; }
	}
	public class IdentityEditable : IdentityInternal
	{
		public string PasswordHash { get; set; }
		public string SecurityStamp { get; set; }
		public ICollection<IdentityCredential> Credentials { get; set; }

		[Comment("注册附加参数")]
		public string SignupExtraArgument { get; set; }
	}
}

