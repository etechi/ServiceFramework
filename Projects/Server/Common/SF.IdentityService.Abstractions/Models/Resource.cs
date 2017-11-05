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

using SF.Data.Models;
using SF.Entities;
using SF.Entities.AutoEntityProvider;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace SF.Auth.IdentityServices.Models
{

	[Comment("资源")]
	[EntityObject]
	public class ResourceInternal : UIObjectEntityBase<string>
	{
		[ReadOnly(false)]
		public override string Id { get; set; }
		[Comment("标识资源")]
		public bool IsIdentityResource { get; set; }
	}

	public class ResourceRequiredClaim
	{
		[Key]
		[Comment("申明类型")]
		[EntityIdent(typeof(ClaimType),nameof(ClaimTypeName))]
		[Required]
		[TableVisible]
		public string ClaimTypeId { get; set; }

		[Comment("申明类型")]
		[TableVisible]
		[Ignore]
		public string ClaimTypeName { get; set; }
	}
	[Comment("资源")]
	public class ResourceEditable : ResourceInternal
	{ 
		[Comment("可用操作")]
		[TableRows]
		public IEnumerable<ResourceOperationInternal> SupportedOperations { get; set; }

		[Comment("所需申明")]
		[TableRows]
		public IEnumerable<ResourceRequiredClaim> RequiredClaims { get; set; }
	}

	[Comment("资源操作")]
	public class ResourceOperationInternal
	{
		[EntityIdent(typeof(OperationInternal),nameof(OperationName))]
		[Comment("操作")]
		[Key]
		public string OperationId { get; set; }

		[Comment("操作名称")]
		[Ignore]
		public string OperationName { get; set; }

	}

	[Comment("操作范围")]
	[EntityObject]
	public class OperationInternal : UIObjectEntityBase<string>
	{
		
	}
}

