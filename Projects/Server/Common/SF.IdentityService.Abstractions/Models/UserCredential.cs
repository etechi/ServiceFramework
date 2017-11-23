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

using SF.Sys.Annotations;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Auth.IdentityServices.Models
{
	public class UserCredential
	{
		/// <summary>
		/// 类型
		/// </summary>
		[Key]
		[Column(Order=1)]
		[EntityIdent(typeof(ClaimType))]
		[TableVisible]
		public string ClaimTypeId { get; set; }
		[TableVisible]
		[Ignore]
		public long UserId { get; set; }

		/// <summary>
		/// 凭证值
		/// </summary>
		[Key]
		[Column(Order = 2)]
		public string Credential { get; set; }
		/// <summary>
		/// 创建时间
		/// </summary>
		[ReadOnly(true)]
		public DateTime CreatedTime { get; set; }
		/// <summary>
		/// 确认时间
		/// </summary>
		public DateTime? ConfirmedTime { get; set; }
	}

}

