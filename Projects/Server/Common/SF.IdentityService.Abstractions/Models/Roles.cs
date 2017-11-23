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
using System.ComponentModel.DataAnnotations;

namespace SF.Auth.IdentityServices.Models
{

	/// <summary>
	/// 角色
	/// </summary>
	[EntityObject]
	public class Role: ObjectEntityBase<string>
	{

	}
	public class Grant
	{
		/// <summary>
		/// 操作资源ID
		/// </summary>
		[Key]
		[EntityIdent(typeof(ResourceInternal), nameof(ResourceName))]
		public string ResourceId { get; set; }

		[TableVisible]
		[Ignore]
		public string ResourceName { get; set; }

		/// <summary>
		/// 操作区域ID
		/// </summary>
		[Key]		
		[EntityIdent(typeof(OperationInternal), nameof(OperationName))]
		public string OperationId { get; set; }

		[TableVisible]
		[Ignore]
		public string OperationName { get; set; }
	}
	public class RoleEditable : Role
	{
		public IEnumerable<ClaimValue> Claims { get; set; }
		[TableRows]
		public IEnumerable<Grant> Grants { get; set; }
	}
	public class UserRole
	{

		/// <summary>
		/// 类型ID
		/// </summary>
		[EntityIdent(typeof(Role), nameof(RoleName))]
		[Key]
		public string RoleId { get; set; }

		/// <summary>
		/// 类型
		/// </summary>
		[Ignore]
		public string RoleName { get; set; }


	}
	
}

