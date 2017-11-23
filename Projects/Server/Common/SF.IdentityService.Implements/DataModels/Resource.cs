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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Auth.IdentityServices.DataModels
{
	[Table(nameof(Resource))]
	public class Resource : SF.Sys.Entities.DataModels.UIObjectEntityBase<string>
	{
		[MaxLength(100)]
		[Required]
		public override string Id { get; set; }

		/// <summary>
		/// 是否是标识资源
		/// </summary>
		public bool IsIdentityResource { get; set; }

		[InverseProperty(nameof(ResourceRequiredClaim.Resource))]
		public ICollection<ResourceRequiredClaim> RequiredClaims { get; set; }

		[InverseProperty(nameof(ResourceSupportedOperation.Resource))]
		public ICollection<ResourceSupportedOperation> SupportedOperations { get; set; }

		[InverseProperty(nameof(ScopeResource.Resource))]
		public ICollection<ScopeResource> Scopes { get; set; }
	}



}
