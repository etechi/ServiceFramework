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
	/// 授权范围
	/// </summary>
	[EntityObject]
	public class ScopeInternal: ObjectEntityBase<string>
	{

	}
	public class ScopeEditable : ScopeInternal
	{
		//[Comment("授权资源")]
		//[TableRows]
		//public IEnumerable<ScopeResource> Resources { get; set; }

		/// <summary>
		/// 授权资源
		/// </summary>
		[EntityIdent(typeof(ResourceInternal))] 
		public IEnumerable<string> Resources { get; set; }
	}
	public class ScopeResource
	{
		/// <summary>
		/// 资源
		/// </summary>
		[EntityIdent(typeof(ResourceInternal), nameof(ResouceName))]
		[Key]
		public string ResourceId { get; set; }

		/// <summary>
		/// 资源名称
		/// </summary>
		[Ignore]
		public string ResouceName { get; set; }


	}
	
}

