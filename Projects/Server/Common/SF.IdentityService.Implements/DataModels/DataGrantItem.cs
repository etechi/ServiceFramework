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

using SF.Sys.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Auth.IdentityServices.DataModels
{
	[Table("GrantItem")]
	public class DataGrantItem
	{
		/// <summary>
		/// 授权ID
		/// </summary>
		[Key]
		[Column(Order = 1)]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public long GrantId { get; set; }

		[ForeignKey(nameof(GrantId))]
		public DataGrant Grant { get; set; }

		/// <summary>
		/// 服务
		/// </summary>
		[Key]
		[Column(Order = 2)]
		[MaxLength(200)]
		[Required]
		public string ServiceId { get; set; }

		/// <summary>
		/// 服务方法，方法为空为授权所有方法
		/// </summary>
		[Key]
		[Column(Order = 3)]
		[MaxLength(200)]
		public string ServiceMethodId { get; set; }

	}
}
