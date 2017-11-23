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
	/// 资源
	/// </summary>
	[EntityObject]
	public class ResourceInternal : UIObjectEntityBase<string>
	{
		[ReadOnly(false)]
		public override string Id { get; set; }
		/// <summary>
		/// 标识资源
		/// </summary>
		public bool IsIdentityResource { get; set; }
	}

	public class ResourceRequiredClaim
	{
		/// <summary>
		/// 申明类型
		/// </summary>
		[Key]
		[EntityIdent(typeof(ClaimType),nameof(ClaimTypeName))]
		[Required]
		[TableVisible]
		public string ClaimTypeId { get; set; }

		/// <summary>
		/// 申明类型
		/// </summary>
		[TableVisible]
		[Ignore]
		public string ClaimTypeName { get; set; }
	}
	/// <summary>
	/// 资源
	/// </summary>
	public class ResourceEditable : ResourceInternal
	{
		/// <summary>
		/// 可用操作
		/// </summary>
		[TableRows]
		public IEnumerable<ResourceOperationInternal> SupportedOperations { get; set; }

		/// <summary>
		/// 所需申明
		/// </summary>
		[TableRows]
		public IEnumerable<ResourceRequiredClaim> RequiredClaims { get; set; }
	}

	/// <summary>
	/// 资源操作
	/// </summary>
	public class ResourceOperationInternal
	{
		/// <summary>
		/// 操作
		/// </summary>
		[EntityIdent(typeof(OperationInternal),nameof(OperationName))]
		[Key]
		public string OperationId { get; set; }

		/// <summary>
		/// 操作名称
		/// </summary>
		[Ignore]
		public string OperationName { get; set; }

	}

	/// <summary>
	/// 操作范围
	/// </summary>
	[EntityObject]
	public class OperationInternal : UIObjectEntityBase<string>
	{
		
	}
}

