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

using SF.Sys.Annotations;
using SF.Sys.Entities.Models;
using System;
using System.ComponentModel;

namespace SF.Auth.IdentityServices.Models
{
	/// <summary>
	/// 凭证类型
	/// </summary>
	[EntityObject]
	public class ClaimType : ObjectEntityBase<string>
	{
		[ReadOnly(false)]
		public override string Id { get; set; }
	}



	/// <summary>
	/// 凭证参数值
	/// </summary>
	public class ClaimValue
	{
		/// <summary>
		/// 类型ID
		/// </summary>
		[EntityIdent(typeof(ClaimType), nameof(TypeName))]
		[TableVisible]
		public string TypeId { get; set; }

		/// <summary>
		/// 类型
		/// </summary>
		[Ignore]
		[TableVisible]
		public string TypeName { get; set; }


		/// <summary>
		/// 凭证值
		/// </summary>
		[TableVisible]
		public string Value { get; set; }

		/// <summary>
		/// 发行时间
		/// </summary>
		public DateTime IssueTime { get; set; }
	}

}

