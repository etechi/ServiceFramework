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

namespace SF.Auth.IdentityServices.Models
{

	/// <summary>
	/// 角色
	/// </summary>
	[EntityObject]
	public class Role: ObjectEntityBase<string>
	{
		/// <summary>
		/// 系统角色
		/// </summary>
		/// <remarks>用于支持系统业务,不能删除</remarks>
		[TableVisible]
		public bool IsSysRole { get; set; }
	}
	public class Grant
	{
		/// <summary>
		/// 操作资源ID
		/// </summary>
		[Key]
		[EntityIdent(typeof(ResourceInternal))]
		public string ResourceId { get; set; }


		/// <summary>
		/// 操作区域ID
		/// </summary>
		[Key]		
		[EntityIdent(typeof(OperationInternal))]
		public string OperationId { get; set; }

	}
	public class GrantEditable
	{
		/// <summary>
		/// 资源
		/// </summary>
		[Key]
		[EntityIdent(typeof(ResourceInternal))]
		[Uneditable]
		public string ResourceId { get; set; }

		/// <summary>
		/// 操作
		/// </summary>
		[EntityIdent(typeof(OperationInternal))]
		public IEnumerable<string> OperationIds { get; set; }

	}
	public class RoleEditable : Role
	{
		/// <summary>
		/// 凭证
		/// </summary>
		public IEnumerable<ClaimValue> Claims { get; set; }
		
		[Ignore]
		public IEnumerable<Grant> Grants { get; set; }

		/// <summary>
		/// 授权
		/// </summary>
		[TableRows]
		public IEnumerable<GrantEditable> GrantEditables { get; set; }
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

