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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Auth.IdentityServices.DataModels
{
	[Table("RoleGrant")]
	public class DataRoleGrant 
	{
		/// <summary>
		/// 客户端ID
		/// </summary>
		[Key]
		[Column(Order =1)]
		public string RoleId { get; set; }

		[ForeignKey(nameof(RoleId))]
		public DataRole Role { get; set; }

		/// <summary>
		/// 授权ID
		/// </summary>
		[Key]
		[Column(Order = 2)]
		[MaxLength(100)]
		[Required]
		public long DstGrantId { get; set; }

		[ForeignKey(nameof(DstGrantId))]
		public DataGrant DstGrant { get; set; }

	}
}
