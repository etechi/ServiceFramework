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
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Auth.Permissions.Models
{

	[EntityObject]
	public class ResourceGrantInternal : IEntityWithId<string>
	{
		[Comment(Name = "资源ID")]
		[ReadOnly(true)]
		[Layout(10)]
		public string Id { get; set; }

		[Display(Name = "资源分组")]
        [ReadOnly(true)]
        [Layout(15)]
        public string Group { get; set; }

        [Comment(Name = "资源项目")]
        [ReadOnly(true)]
        [Layout(20)]
        public string Name { get; set; }

        [Display(Name = "授权")]
        [Layout(25)]
        [EntityIdent(typeof(OperationInternal),ScopeField=nameof(Id))]
        public string[] OperationIds { get; set; }


    }
}
