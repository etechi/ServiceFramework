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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Auth.IdentityServices.Models
{
	/// <summary>
	/// 授权
	/// </summary>
	[EntityObject]
	public class Grant : ObjectEntityBase<long>
	{

	}
	public class GrantEditable : Grant
	{
		/// <summary>
		/// 授权方法
		/// </summary>
		//[EntityIdent(typeof(GrantItem))]
		[TableRows]
		public IEnumerable<GrantItem> Items { get; set; }
	}
	public class GrantItem
	{
		/// <summary>
		/// 服务
		/// </summary>
		[Required]
		[MaxLength(200)]
		[Key]
		[Column(Order=1)]
		public string ServiceId { get; set; }

		/// <summary>
		/// 方法
		/// </summary>
		[Required]
		[MaxLength(100)]
		[Key]
		[Column(Order = 2)]
		public string ServiceMethodId { get; set; }

	}
	
}

