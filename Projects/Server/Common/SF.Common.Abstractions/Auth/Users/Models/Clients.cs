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

using SF.Data.Models;
using SF.Entities;
using SF.Entities.AutoEntityProvider;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace SF.Auth.Users.Models
{

	[Comment("认证客户端")]
	public class Client : UIObjectEntityBase<long>
	{
		[Comment("标识")]
		public string Ident { get; set; }

		[Comment("默认角色")]
		[EntityIdent(typeof(Role))]
		public IEnumerable<long> Roles {get;set;}
	}
	
}

