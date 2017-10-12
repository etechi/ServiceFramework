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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Auth.Permissions.Models
{

	[EntityObject]
	public class RoleInternal : ObjectEntityBase<string>
    {
        [Comment(Name="Id",Description ="角色创建以后，ID不能再修改")]
        public override string Id { get; set; }

        [Display(Name="名称",Description ="角色的名称")]
		
        public override string Name { get; set; }

		[Display(Name = "说明", Description = "角色的说明")]
		[MaxLength(100)]
		public string Description { get; set; }

		[TableVisible]
        [Comment(Name = "角色状态",Description ="当角色无效时，角色相关的用户不会得到授权")]
        public override EntityLogicState LogicState { get; set; }

        [Comment(Name = "授权清单",Description ="注：对权限的修改在用户下次登录时生效")]
        [TableRows]
        [ReadOnly(true)]
        public ResourceGrantInternal[] Grants { get; set; }

        
    }

   
}
