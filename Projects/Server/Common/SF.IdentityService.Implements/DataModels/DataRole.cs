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
	[Table("Role")]
	public class DataRole : SF.Sys.Entities.DataModels.DataObjectEntityBase<string>
	{
		[MaxLength(100)]
		[Required]
		public override string Id { get; set; }

		[Index(IsUnique = true)]
		public override string Name { get; set; }

		[InverseProperty(nameof(DataRoleClaimValue.Role))]
		public ICollection<DataRoleClaimValue> ClaimValues { get; set; }

		[InverseProperty(nameof(DataRoleGrant.Role))]
		public ICollection<DataRoleGrant> Grants { get; set; }

		/// <summary>
		/// 系统角色
		/// </summary>
		/// <remarks>用于支持系统业务,不能删除</remarks>
		public bool IsSysRole { get; set; }
	}
}
