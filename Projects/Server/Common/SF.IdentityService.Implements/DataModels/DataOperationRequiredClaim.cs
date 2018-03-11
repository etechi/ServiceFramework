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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Auth.IdentityServices.DataModels
{
	[Table("OperationRequiredClaim")]
	public class DataOperationRequiredClaim
	{
		/// <summary>
		/// 操作Id
		/// </summary>
		[Column(Order =1)]
		[Key]
		public string OperationId { get; set; }

		[ForeignKey(nameof(OperationId))]
		public DataResource Resource { get; set; }

		/// <summary>
		/// 申明类型Id
		/// </summary>
		[Column(Order = 2)]
		[Key]
		[Index]
		public string ClaimTypeId { get; set; }

		[ForeignKey(nameof(ClaimTypeId))]
		public DataClaimType ClaimType { get; set; }
	}
}
