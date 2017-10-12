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
using SF.Data.Models;
using SF.Metadata;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using SF.Core.ServiceManagement.Models;

namespace SF.Management.MenuServices.Models
{
	[Comment("菜单项动作")]
	public enum MenuItemAction
	{
		[Comment("无")]
		None,
		[Comment("实体管理")]
		EntityManager,
		[Comment("显示表单")]
		Form,
		[Comment("显示列表")]
		List,
		[Comment("显示内嵌网页")]
		IFrame,
		[Comment("打开链接")]
		Link
	}

	[Comment("菜单项")]
	public class MenuItem : UIObjectEntityBase<long>
	{

		[Comment("字体图标", "字体图标", GroupName = "前端展示")]
		[MaxLength(100)]
		public virtual string FontIcon { get; set; }


		[Comment("动作")]
		public MenuItemAction Action { get; set; }

		[Comment("动作参数")]
		public string ActionArgument { get; set; }

		[Comment("服务")]
		[EntityIdent(typeof(ServiceInstance))]
		public long? ServiceId { get; set; }

		[Comment("子菜单")]
		[TreeNodes]
		public IEnumerable<MenuItem> Children { get; set; }

		[Ignore]
		public long? ParentId { get; set; }

	}

	public class MenuEditable : Menu
    {
		
		[TreeNodes]
		[Comment("菜单项")]
		public IEnumerable<MenuItem> Items { get; set; }
	}
}

