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

using SF.Data;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Auth.IdentityServices.DataModels
{
	[Table(nameof(ResourceRequiredClaim))]
	public class ResourceRequiredClaim
	{
		[Column(Order =1)]
		[Key]
		[Comment("资源Id")]
		public long ResourceId { get; set; }

		[ForeignKey(nameof(ResourceId))]
		public Resource Resource { get; set; }

		[Column(Order = 2)]
		[Key]
		[Comment("申明类型Id")]
		[Index]
		public long ClaimTypeId { get; set; }

		[ForeignKey(nameof(ClaimTypeId))]
		public ClaimType ClaimType { get; set; }
	}
}
